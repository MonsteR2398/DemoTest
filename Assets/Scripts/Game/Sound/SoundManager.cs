using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Runtime / debug")]
    public bool debugLogs = true;

    [Header("Global 2D loop source (assign in inspector)")]
    public AudioSource loopSource; // spatialBlend = 0

    [Header("Tempo & scheduling")]
    public double bpm = 120.0;
    public double lookAheadSec = 0.75;
    public double safetyLeadSec = 0.03;

    private double SecPerBeat => 60.0 / bpm;

    private double _sessionStartDsp;
    private bool _sessionActive = false;

    private readonly Dictionary<SoundContainer, List<SoundEmitter>> _zoneEmitters = new();
    private readonly Dictionary<SoundEmitter, EmitterPlan> _plans = new();
    private readonly Dictionary<SoundEmitter, SoundContainer> _ownerZone = new();
    private readonly Dictionary<SoundContainer, List<AudioSource>> _scheduledByZone = new();
    private readonly Dictionary<SoundContainer, double> _zoneCycle = new();
    private readonly Dictionary<SoundContainer, HashSet<AudioClip>> _zoneClipSet = new();

    private SoundContainer _loopOwnerZone;

    private double _lastScheduledBeat = 0.0;
    private readonly HashSet<string> _newScheduledKeys = new();
    private readonly HashSet<SoundContainer> ActiveZones = new();

    public SoundContainer LastActiveZone = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // -------------------- Zone API --------------------
    public void EnterZone(SoundContainer zone, List<SoundEmitter> emitters, double zoneBpm, bool startNewSession, double cycleLen)
    {
        if (zone == null) return;

        if (!ActiveZones.Contains(zone))
        {
            ActiveZones.Add(zone);
            _zoneCycle[zone] = (cycleLen > 0.001) ? cycleLen : 16.0;
            _zoneEmitters[zone] = new List<SoundEmitter>();
            _zoneClipSet[zone] = new HashSet<AudioClip>();
        }
        LastActiveZone = zone;
        if (debugLogs) Debug.Log($"[SM] EnterZone: {zone.name} | emitters={emitters?.Count ?? 0} | startNew={startNewSession} | zoneBpm={zoneBpm} | cycle={_zoneCycle[zone]}");

        // если луп в списке и просили начать новую сессию — стартуем с ним
        var loopEmitter = emitters?.FirstOrDefault(emitter => emitter != null && emitter.isLoop && emitter.clip != null);

        if (!_sessionActive)
        {
            if (startNewSession && loopEmitter != null)
                StartNewSession(zoneBpm > 0 ? zoneBpm : bpm, loopEmitter);
            else
                StartSessionWithoutLoop(zoneBpm > 0 ? zoneBpm : bpm);
        }
        else
        {
            if (loopEmitter != null && (_loopOwnerZone == null || !loopSource.isPlaying))
                StartLoopAlignedToNextWholeBeat(loopEmitter);
        }

        // зарегистрировать переданные эмиттеры
        if (emitters != null)
        {
            foreach (var e in emitters)
                RegisterEmitterInZone(zone, e, zoneBpm, startNewSession, _zoneCycle[zone]);
        }
    }

    public void RegisterEmitterInZone(SoundContainer zone, SoundEmitter emitter, double zoneBpm, bool startNewSession, double cycleLen)
    {
        if (zone == null || emitter == null || emitter.clip == null) return;

        if (!_zoneEmitters.ContainsKey(zone)) _zoneEmitters[zone] = new List<SoundEmitter>();
        if (!_zoneClipSet.ContainsKey(zone)) _zoneClipSet[zone] = new HashSet<AudioClip>();
        _zoneCycle[zone] = (cycleLen > 0.001) ? cycleLen : (_zoneCycle.TryGetValue(zone, out var cycle) ? cycle : 16.0);

        // отсекаем дубль по самому AudioClip
        if (_zoneClipSet[zone].Contains(emitter.clip))
        {
            if (debugLogs) Debug.Log($"[SM] Skip duplicate clip '{emitter.clip.name}' in zone {zone.name}");
            return;
        }

        _zoneClipSet[zone].Add(emitter.clip);
        _zoneEmitters[zone].Add(emitter);
        _ownerZone[emitter] = zone;

        // если сессии нет — запустим
        if (!_sessionActive)
        {
            if (emitter.isLoop && startNewSession)
                StartNewSession(zoneBpm > 0 ? zoneBpm : bpm, emitter);
            else
                StartSessionWithoutLoop(zoneBpm > 0 ? zoneBpm : bpm);
        }

        // если это луп и сейчас ничего не играет — стартуем
        if (emitter.isLoop && (_loopOwnerZone == null || !loopSource.isPlaying))
            StartLoopAlignedToNextWholeBeat(emitter);

        // создать план (шаги паттерна повторяются по cycleLen)
        if (!_plans.ContainsKey(emitter))
        {
            var plan = EmitterPlan.Build(emitter, CurrentBeat(), _zoneCycle[zone]);
            _plans[emitter] = plan;
            if (debugLogs) Debug.Log($"[SM] Registered plan '{emitter.clip.name}' in zone {zone.name} (loop={emitter.isLoop})");
        }
    }

    public void UnregisterEmitterInZone(SoundContainer zone, SoundEmitter emitter)
    {
        if (zone == null || emitter == null) return;

        if (_zoneEmitters.TryGetValue(zone, out var list))
        {
            list.Remove(emitter);
        }

        _zoneClipSet.TryGetValue(zone, out var set);
        set?.Remove(emitter.clip);

        _plans.Remove(emitter);
        _ownerZone.Remove(emitter);
    }

    public void ExitZone(SoundContainer zone)
    {
        if (zone == null) return;
        if (!ActiveZones.Contains(zone)) return;

        if (debugLogs) Debug.Log($"[SM] ExitZone: {zone.name}");

        CancelScheduledForZone(zone);

        if (_zoneEmitters.TryGetValue(zone, out var list))
        {
            foreach (var emitter in list)
            {
                _plans.Remove(emitter);
                _ownerZone.Remove(emitter);
            }
        }

        _zoneEmitters.Remove(zone);
        _zoneClipSet.Remove(zone);
        _zoneCycle.Remove(zone);
        ActiveZones.Remove(zone);

        if (_loopOwnerZone == zone)
        {
            loopSource.Stop();
            _loopOwnerZone = null;

            // попробовать найти другой луп
            var another = _plans.Keys.FirstOrDefault(key => key.isLoop);
            if (another != null)
                StartLoopAlignedToNextWholeBeat(another);
            else if (_plans.Count == 0)
                _sessionActive = false;
        }
    }

    // -------------------- Session / loop --------------------
    private void StartNewSession(double newBpm, SoundEmitter loopEmitter)
    {
        bpm = newBpm > 0 ? newBpm : bpm;
        _lastScheduledBeat = 0.0;
        _newScheduledKeys.Clear();

        double dspNow = AudioSettings.dspTime;
        _sessionStartDsp = dspNow + safetyLeadSec;
        _sessionActive = true;

        if (loopSource == null) { Debug.LogError("[SM] loopSource not assigned!"); return; }
        loopSource.Stop();
        loopSource.clip = loopEmitter.clip;
        loopSource.loop = true;
        loopSource.spatialBlend = 0f;
        loopSource.PlayScheduled(_sessionStartDsp);
        _loopOwnerZone = _ownerZone.ContainsKey(loopEmitter) ? _ownerZone[loopEmitter] : null;

        // перестроить все планы относительно начала (beat 0)
        var keys = _plans.Keys.ToArray();
        foreach (var key in keys) _plans[key] = EmitterPlan.Build(key, 0.0, GetZoneCycleFor(key));

        if (debugLogs) Debug.Log($"[SM] StartNewSession: bpm={bpm}, sessionStartDsp={_sessionStartDsp:F4}, loop='{loopEmitter.clip.name}'");
    }

    private void StartSessionWithoutLoop(double newBpm)
    {
        bpm = newBpm > 0 ? newBpm : bpm;
        double dspNow = AudioSettings.dspTime;
        _sessionStartDsp = dspNow + safetyLeadSec;
        _sessionActive = true;
        _lastScheduledBeat = 0.0;
        _newScheduledKeys.Clear();

        if (debugLogs) Debug.Log($"[SM] StartSessionWithoutLoop: bpm={bpm}, sessionStartDsp={_sessionStartDsp:F4}");
    }

    private void StartLoopAlignedToNextWholeBeat(SoundEmitter loopEmitter)
    {
        if (!_sessionActive) StartSessionWithoutLoop(bpm);

        double dspNow = AudioSettings.dspTime;
        double nowBeat = CurrentBeat(dspNow);
        double nextWhole = System.Math.Ceiling(nowBeat);
        double startTime = BeatToDsp(nextWhole) + safetyLeadSec;

        if (loopSource == null) { Debug.LogError("[SM] loopSource not assigned!"); return; }
        loopSource.Stop();
        loopSource.clip = loopEmitter.clip;
        loopSource.loop = true;
        loopSource.spatialBlend = 0f;
        loopSource.PlayScheduled(startTime);
        _loopOwnerZone = _ownerZone.ContainsKey(loopEmitter) ? _ownerZone[loopEmitter] : null;

        if (debugLogs) Debug.Log($"[SM] StartLoopAlignedToNextWholeBeat: loop='{loopEmitter.clip.name}' startAtBeat={nextWhole} dsp={startTime:F4}");
    }

    // -------------------- Update / scheduling --------------------
    private void Update()
    {
        if (!_sessionActive) return;

        double dspNow = AudioSettings.dspTime;
        double currentBeat = CurrentBeat(dspNow);
        double scheduleUntil = currentBeat + lookAheadSec / SecPerBeat;

        _newScheduledKeys.Clear();
        int scheduledCount = 0;

        foreach (var planKeyValue in _plans)
        {
            var emitter = planKeyValue.Key;
            var plan = planKeyValue.Value;

            if (emitter == null || emitter.clip == null) continue;
            if (emitter.isLoop) continue; // луп играет через loopSource

            // 1) шаги паттерна — повторяются каждые plan.cycleLen
            if (plan.stepBeats != null)
            {
                for (int i = 0; i < plan.nextStepDue.Length; i++)
                {
                    while (plan.nextStepDue[i] <= scheduleUntil)
                    {
                        if (plan.nextStepDue[i] > _lastScheduledBeat)
                        {
                            if (TryScheduleOneShot(emitter, plan.nextStepDue[i], out _)) scheduledCount++;
                        }
                        plan.nextStepDue[i] += plan.cycleLen;
                    }
                }
            }

            // 2) интервалы
            if (plan.intervals != null)
            {
                for (int i = 0; i < plan.intervals.Length; i++)
                {
                    double step = plan.intervals[i];
                    if (step <= 1e-9) continue;

                    double next = plan.nextDue[i];
                    while (next <= scheduleUntil)
                    {
                        if (next > _lastScheduledBeat)
                        {
                            if (TryScheduleOneShot(emitter, next, out _)) scheduledCount++;
                        }
                        next += step;
                    }
                    plan.nextDue[i] = next;
                }
            }
        }

        if (debugLogs && scheduledCount > 0)
            Debug.Log($"[SM] Scheduled {scheduledCount} events up to beat {scheduleUntil:F3}");

        _lastScheduledBeat = scheduleUntil;
    }

    // -------------------- Plan one shot --------------------
    private bool TryScheduleOneShot(SoundEmitter emitter, double beat, out double dspAt)
    {
        dspAt = BeatToDsp(beat);

        // дедупликация: один и тот же AudioClip на одном и том же бите — один раз
        string key = $"{emitter.clip.GetInstanceID()}@{System.Math.Round(beat, 6)}";
        if (_newScheduledKeys.Contains(key)) return false;
        _newScheduledKeys.Add(key);

        var source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        source.clip = emitter.clip;
        source.spatialBlend = 0f;
        source.PlayScheduled(dspAt);

        var zone = _ownerZone.TryGetValue(emitter, out var z) ? z : null;
        if (zone != null)
        {
            if (!_scheduledByZone.TryGetValue(zone, out var list))
            {
                list = new List<AudioSource>();
                _scheduledByZone[zone] = list;
            }
            list.Add(source);
        }

        Destroy(source, (float)(emitter.clip.length + 3.0));

        if (debugLogs)
            Debug.Log($"[SM] OneShot '{emitter.clip.name}' beat={beat:F3} dspAt={dspAt:F4}");

        return true;
    }

    private void CancelScheduledForZone(SoundContainer zone)
    {
        if (zone == null) return;
        if (!_scheduledByZone.TryGetValue(zone, out var list)) return;

        if (debugLogs) Debug.Log($"[SM] CancelScheduledForZone: {zone.name} cancelCount={list.Count}");
        foreach (var s in list)
        {
            if (s == null) continue;
            s.Stop();
            Destroy(s);
        }
        list.Clear();
        _scheduledByZone.Remove(zone);
    }

    // -------------------- Helpers --------------------
    private double CurrentBeat(double dspNow) => (dspNow - _sessionStartDsp) / SecPerBeat;
    private double CurrentBeat() => CurrentBeat(AudioSettings.dspTime);
    private double BeatToDsp(double beat) => _sessionStartDsp + beat * SecPerBeat;

    private double GetZoneCycleFor(SoundEmitter e)
    {
        if (e != null && _ownerZone.TryGetValue(e, out var z) && _zoneCycle.TryGetValue(z, out var c)) return c;
        return 16.0;
    }

    // -------------------- Emitter plan --------------------
    private class EmitterPlan
    {
        // pattern steps (repeating every cycleLen)
        public double[] stepBeats;       // e.oneShotBeats (sorted, unique)
        public double cycleLen;          // zone cycle length
        public double[] nextStepDue;     // next time for each step

        // intervals (optional)
        public double[] intervals;
        public double[] nextDue;

        public static EmitterPlan Build(SoundEmitter e, double currentBeat, double cycleLen)
        {
            var p = new EmitterPlan();
            p.cycleLen = (cycleLen > 0.001) ? cycleLen : 16.0;

            // Шаги паттерна: повторяем их каждые cycleLen
            if (e.oneShotBeats != null && e.oneShotBeats.Length > 0)
            {
                var list = new List<double>(e.oneShotBeats.Where(b => b >= 0.0));
                list.Sort();

                // убрать дубли шагов (например 0 и 0.0)
                var uniq = new List<double>();
                double? prev = null;
                foreach (var b in list)
                {
                    if (prev == null || System.Math.Abs(b - prev.Value) > 1e-6)
                        uniq.Add(b);
                    prev = b;
                }

                p.stepBeats = uniq.ToArray();
                p.nextStepDue = new double[p.stepBeats.Length];

                for (int i = 0; i < p.stepBeats.Length; i++)
                {
                    double step = p.stepBeats[i];
                    double k = System.Math.Ceiling((currentBeat - step) / p.cycleLen);
                    if (double.IsInfinity(k) || double.IsNaN(k)) k = 0;
                    p.nextStepDue[i] = step + System.Math.Max(0, k) * p.cycleLen;
                    if (p.nextStepDue[i] < currentBeat) p.nextStepDue[i] += p.cycleLen;
                }
            }

            // интервалы
            if (e.useIntervals && e.intervals != null && e.intervals.Length > 0)
            {
                p.intervals = e.intervals.ToArray();
                p.nextDue = new double[p.intervals.Length];
                for (int i = 0; i < p.intervals.Length; i++)
                {
                    double step = System.Math.Max(1e-9, p.intervals[i]);
                    double x = (currentBeat - e.intervalOffsetBeats) / step;
                    double k = System.Math.Ceiling(x);
                    p.nextDue[i] = e.intervalOffsetBeats + k * step;
                }
            }

            return p;
        }
    }
}

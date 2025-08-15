using UnityEngine;

/// Точный счётчик тактов на базе AudioSettings.dspTime.
/// Позволяет безопасно менять BPM "на лету" без скачков.
public sealed class BeatClock
{
    public double BPM { get; private set; }

    // Точка, от которой считаем "прирост" битов с текущим BPM
    private double _anchorDsp;
    // Сколько битов накоплено к моменту _anchorDsp
    private double _anchorBeat;

    public BeatClock(double bpm)
    {
        Reset(bpm, AudioSettings.dspTime);
    }

    public void Reset(double bpm, double dspNow)
    {
        BPM = Mathf.Max(1f, (float)bpm);
        _anchorDsp = dspNow;
        _anchorBeat = 0.0;
    }

    /// Текущий бит с учётом времени и темпа
    public double GetBeat(double dspNow)
    {
        double secPerBeat = 60.0 / BPM;
        return _anchorBeat + (dspNow - _anchorDsp) / secPerBeat;
    }

    /// Сменить BPM на лету без рассинхрона.
    public void SetBPM(double newBpm, double dspNow)
    {
        // Зафиксировать текущий бит, "переякорить" и сменить темп
        _anchorBeat = GetBeat(dspNow);
        BPM = Mathf.Max(1f, (float)newBpm);
        _anchorDsp = dspNow;
    }

    public double BeatToDsp(double beat, double dspNow)
    {
        double currentBeat = GetBeat(dspNow);
        double deltaBeats = beat - currentBeat;
        return dspNow + deltaBeats * (60.0 / BPM);
    }

    public double SecPerBeat => 60.0 / BPM;
}

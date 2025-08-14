using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private AudioSource _audioSource;
    private List<ISoundEmitter> _registeredSounds = new List<ISoundEmitter>();
    private Dictionary<ISoundEmitter, float> _nextPlayTimes = new Dictionary<ISoundEmitter, float>();
    private ISoundEmitter _currentLoopedSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        HandleTimedSounds();
    }

    public void RegisterSound(ISoundEmitter soundEmitter)
    {
        if (_registeredSounds.Contains(soundEmitter)) return;

        _registeredSounds.Add(soundEmitter);
        
        if (soundEmitter.Interval <= 0 && soundEmitter.phaseOffset <= 0)
        {
            PlayLoopedSound(soundEmitter);
            return;
        }

        float firstPlayTime = Time.time + soundEmitter.phaseOffset;
        _nextPlayTimes[soundEmitter] = firstPlayTime;

        if (soundEmitter.phaseOffset <= 0)
        {
            float playTime = Time.time;
            _audioSource.PlayOneShot(soundEmitter.Clip);
            _nextPlayTimes[soundEmitter] = playTime + soundEmitter.Interval;
        }
    }

    public void UnregisterSound(ISoundEmitter soundEmitter)
    {
        if (!_registeredSounds.Contains(soundEmitter)) return;

        _registeredSounds.Remove(soundEmitter);
        _nextPlayTimes.Remove(soundEmitter);

        if (_currentLoopedSound == soundEmitter)
        {
            _audioSource.Stop();
            _currentLoopedSound = null;
        }
    }

    private void PlayLoopedSound(ISoundEmitter soundEmitter)
    {
        if (_currentLoopedSound != null) return;

        _currentLoopedSound = soundEmitter;
        _audioSource.clip = soundEmitter.Clip;
        _audioSource.loop = true;
        _nextPlayTimes[soundEmitter] = Time.time;
        _audioSource.Play();
    }

    private void HandleTimedSounds()
    {
        float currentTime = Time.time;
        List<ISoundEmitter> toRemove = new List<ISoundEmitter>();

        foreach (var emitter in _registeredSounds)
        {
            if (!_nextPlayTimes.TryGetValue(emitter, out var nextTime)) continue;

            if (currentTime >= nextTime)
            {
                float playTime = Time.time;
                _audioSource.PlayOneShot(emitter.Clip);
                
                if (emitter.Interval > 0)
                    _nextPlayTimes[emitter] = playTime + emitter.Interval;
                else
                    toRemove.Add(emitter);
            }
        }

        foreach (var emitter in toRemove)
            UnregisterSound(emitter);
    }
}

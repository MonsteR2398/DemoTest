using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private int BrainrotBPM = 60;
    private Dictionary<AudioClip, ISoundEmitter> _activeSounds = new Dictionary<AudioClip, ISoundEmitter>();
    private AudioSource _source;
    private int _globalBeatCounter;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        _source = GetComponent<AudioSource>();
        StartCoroutine(BeatRoutine());
    }

    public void RegisterSound(ISoundEmitter audio)
    {
        if (audio.SoundInterval <= 0)
        {
            audio.SoundInterval = 1;
            //Debug.LogWarning($"Interval for {audio.Clip.name} was corrected to 1.");
        }
        
        if (!_activeSounds.ContainsKey(audio.Clip))
            _activeSounds.Add(audio.Clip, audio);
    }

    public void UnregisterSound(ISoundEmitter audio)
    {
        if (_activeSounds.ContainsKey(audio.Clip))
            _activeSounds.Remove(audio.Clip);
    }

    private IEnumerator BeatRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f / BrainrotBPM);
            _globalBeatCounter++;
            foreach (var pair in _activeSounds)
            {
                var audio = pair.Value;
                if (_globalBeatCounter % audio.SoundInterval == 0)
                { 
                    _source.PlayOneShot(audio.Clip);
                }
            }
        }
    }
}
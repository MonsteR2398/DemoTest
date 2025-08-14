using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundContainer : MonoBehaviour, ITriggerEnterHandler, ITriggerExitHandler
{
    private List<ISoundEmitter> _audios = new List<ISoundEmitter>();
    private bool _isActive;


    public void HandleTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ISoundEmitter>(out var audio))
        {
            RegisterDynamicAudio(audio);
        }
        else
        { 
            _isActive = true;
            UpdateSoundRegistration();
        }
    }

    public void HandleTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ISoundEmitter>(out var audio))
        { 
            UnregisterDynamicAudio(audio);
        }
        else
        { 
            _isActive = false;
            UpdateSoundRegistration();
        }
    }

    public void SetActive(bool isActive)
    {
        _isActive = isActive;
        UpdateSoundRegistration();
    }

    public void RegisterDynamicAudio(ISoundEmitter audio)
    {
        if (!_audios.Contains(audio))
        {
            _audios.Add(audio);
            if (_isActive)
                SoundManager.Instance.RegisterSound(audio);
        }
    }

    public void UnregisterDynamicAudio(ISoundEmitter audio)
    {
        if (_audios.Remove(audio) && _isActive)
            SoundManager.Instance.UnregisterSound(audio);
    }

    private void UpdateSoundRegistration()
    {
        var allAudios = new List<ISoundEmitter>();
        allAudios.AddRange(_audios);

        foreach (var audio in allAudios)
        {
            if (_isActive)
                SoundManager.Instance.RegisterSound(audio);
            else
                SoundManager.Instance.UnregisterSound(audio);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SoundContainer : MonoBehaviour, ITriggerEnterHandler, ITriggerExitHandler
{
    [Header("Настройки зоны")]
    public double zoneBpm = 120.0;
    public bool startNewSessionOnEnter = true;

    [Header("Длина цикла зоны (в битах)")]
    public double cycleLengthBeats = 16.0;

    private readonly HashSet<SoundEmitter> _emitters = new();
    private bool _isActive;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    private void Awake()
    {
        var list = new List<SoundEmitter>();
        GetComponentsInChildren(includeInactive: true, result: list);
        foreach (var e in list) _emitters.Add(e);
    }

    // private void OnTriggerEnter(Collider other) => InternalEnter(other);
    // private void OnTriggerExit(Collider other)  => InternalExit(other);
    public void HandleTriggerEnter(Collider other) => InternalEnter(other);
    public void HandleTriggerExit(Collider other)
    {
        Debug.Log("EXIT");
        InternalExit(other);
    }

    private void InternalEnter(Collider other)
    {
        if (other.TryGetComponent(out SoundEmitter emitter))
        {
            RegisterDynamicAudio(emitter);
            return;
        }
        else if(LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            _isActive = true;
            SoundManager.Instance.EnterZone(this, _emitters.ToList(), zoneBpm, startNewSessionOnEnter, cycleLengthBeats);
        }
    }

    private void InternalExit(Collider other)
    {
        if (other.TryGetComponent(out SoundEmitter emitter))
        {
            UnregisterDynamicAudio(emitter);
            return;
        }
        else
        {
            _isActive = false;
            SoundManager.Instance.ExitZone(this);
        }
    }

    public void RegisterDynamicAudio(SoundEmitter emitter)
    {
        if (emitter == null) return;
        _emitters.Add(emitter);

        if (_isActive)
        {
            SoundManager.Instance.RegisterEmitterInZone(this, emitter, zoneBpm, startNewSessionOnEnter, cycleLengthBeats);
        }
    }

    public void UnregisterDynamicAudio(SoundEmitter emitter)
    {
        if (emitter == null) return;
        if (_emitters.Remove(emitter))
        {
            SoundManager.Instance.UnregisterEmitterInZone(this, emitter);
        }
    }
}

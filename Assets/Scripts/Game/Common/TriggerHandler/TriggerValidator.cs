using UnityEngine;

public class TriggerValidator : MonoBehaviour
{
    [SerializeField] private LayerMask validLayers;
    private ITriggerHandler[] _handlers;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _handlers = GetComponents<ITriggerHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & validLayers) == 0) return;
        foreach (var handler in _handlers)
            handler?.HandleTrigger(other);
    }
}
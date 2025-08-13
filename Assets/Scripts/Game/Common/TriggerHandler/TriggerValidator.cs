using UnityEngine;

public class TriggerValidator : MonoBehaviour
{
    [SerializeField] private LayerMask validLayers;
    
    // Кешированные компоненты (опционально для оптимизации)
    private ITriggerEnterHandler[] _enterHandlers;
    private ITriggerStayHandler[] _stayHandlers;
    private ITriggerExitHandler[] _exitHandlers;
    private bool _isInitialized;

    private void Awake() => Initialize();


    private void Initialize()
    {
        if (_isInitialized) return;
        
        _enterHandlers = GetComponents<ITriggerEnterHandler>();
        _stayHandlers = GetComponents<ITriggerStayHandler>();
        _exitHandlers = GetComponents<ITriggerExitHandler>();
        _isInitialized = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsLayerValid(other.gameObject.layer)) return;
        foreach (var handler in _enterHandlers)
            handler.HandleTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsLayerValid(other.gameObject.layer)) return;
        
        foreach (var handler in _stayHandlers)
            handler.HandleTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsLayerValid(other.gameObject.layer)) return;
        
        foreach (var handler in _exitHandlers)
            handler.HandleTriggerExit(other);
    }

    private bool IsLayerValid(int layer) => ((1 << layer) & validLayers) != 0;


    // динамическое обновление обработчиков
    public void RefreshHandlers()
    {
        _isInitialized = false;
        Initialize();
    }
}
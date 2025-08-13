using UnityEngine;

public abstract class InteractableDisplay : MonoBehaviour, IInteractable
{
    [Header("Display Settings")]
    [SerializeField] protected Vector3 _displayOffset = new Vector3(0, 1.5f, 0);
    [SerializeField] protected bool _alwaysVisible = false;
    
    public Vector3 GetPosition() => transform.position + _displayOffset;
    public bool IsAlwaysVisible() => _alwaysVisible;
    
    public abstract DisplayData GetDisplayData();
    
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //         UIManager.Instance.RegisterDisplay(this);
    // }
    
    // private void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //         UIManager.Instance.UnregisterDisplay(this);
    // }
    
    // private void OnDestroy()
    // {
    //     UIManager.Instance?.UnregisterDisplay(this);
    // }
}
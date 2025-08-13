using UnityEngine;
using UnityEngine.UI;

public abstract class BaseInteractionZone : MonoBehaviour, ITriggerEnterHandler, ITriggerStayHandler, ITriggerExitHandler
{
    [SerializeField] protected BuyDisplay _display;
    [SerializeField] protected Vector3 _displayOffset = new Vector3(0, 1f, 0);

    protected Renderer[] zoneRenderers;
    public Color[] OriginalColors;
    
    protected bool isActive = false;
    protected static BaseInteractionZone currentActiveZone;
    private Hammer currentHammer;

    protected virtual void Start()
    {
        if (_display != null)
        {
            _display.Initialize(null, _displayOffset);
            _display.SetActive(false);
        }

        zoneRenderers = GetComponentsInChildren<Renderer>();
        OriginalColors = new Color[zoneRenderers.Length];
        for (int i = 0; i < zoneRenderers.Length; i++)
            OriginalColors[i] = zoneRenderers[i].material.color;
    }

    protected virtual void OnEnable()
    {
        if (_display != null)
        {
            _display.Initialize(null, _displayOffset);
            _display.SetActive(false);
        }
    }

    private void OnDisable()
    { 
        if(currentHammer != null)
            currentHammer.HammerDeactivate -= DeactivateZone;
    } 

    public void HandleTriggerEnter(Collider other)
    {
        ActivateZone(other.transform);
    }

    public void HandleTriggerStay(Collider other)
    {
        ActivateZone(other.transform);
    }

    public void HandleTriggerExit(Collider other)
    {
        DeactivateZone();
    }
    
    protected virtual void ActivateZone(Transform playerTransform)
    {
        if (InventorySystem.Instance.itemInHand == null || isActive)
            return;

        bool currentZoneValid = currentActiveZone != null && currentActiveZone.gameObject != null;
        
        if (!currentZoneValid || IsCloserToPlayerThan(playerTransform, currentActiveZone))
        {
            if (currentZoneValid && currentActiveZone != this)
                currentActiveZone.DeactivateZone();
            currentActiveZone = this;
            _display.SetActive(true);
            SetZoneColor(new Color(1, 0, 0, 0.75f));
            _display.GetButton.onClick.AddListener(OnActionButtonClick);
            isActive = true;

            if (currentHammer != null) return;
            if (InventorySystem.Instance.itemInHand is Hammer hammer)
            { 
                currentHammer = hammer;
                currentHammer.HammerDeactivate += DeactivateZone;
            }
        }
    }
    
    
    private bool IsCloserToPlayerThan(Transform target, BaseInteractionZone otherZone)
    {
        float thisDistance = Vector3.Distance(target.position, transform.position);
        float otherDistance = Vector3.Distance(target.position, otherZone.transform.position);
        return thisDistance < otherDistance;
    }
    
    protected virtual void DeactivateZone()
    {
        if (!isActive) return;
        
        _display.SetActive(false);
        ResetZoneColor();
        _display.GetButton.onClick.RemoveListener(OnActionButtonClick);
        isActive = false;
        currentHammer = null;
        
        if (currentActiveZone == this)
            currentActiveZone = null;
    }

    protected virtual void OnDestroy()
    {
        if(currentHammer != null)
            currentHammer.HammerDeactivate -= DeactivateZone;
        if (currentActiveZone == this)
            currentActiveZone = null;
    }
    
    private void SetZoneColor(Color color)
    {
        foreach (var renderer in zoneRenderers)
            renderer.material.color = color;
    }

    private void ResetZoneColor()
    {
        for (int i = 0; i < zoneRenderers.Length; i++)
            zoneRenderers[i].material.color = OriginalColors[i];
    }

    public abstract void OnActionButtonClick();
}

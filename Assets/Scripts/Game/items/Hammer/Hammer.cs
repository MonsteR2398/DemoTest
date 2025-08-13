using System;
using UnityEngine;

public class Hammer : Item
{
    public override string GetItemID() => "hammer";
    [SerializeField] private GameObject _triggerObj;
    public Action HammerDeactivate;

    void Start()
    {
        InventorySystem.Instance.AddItem(this, 1);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _triggerObj.SetActive(true);
    }
    private void OnDisable()
    {
        _triggerObj.SetActive(false);
        HammerDeactivate?.Invoke();
    }

    private bool isDestroying = false;

    // public void UseInPlacementZone(BaseInteractionZone zone)
    // {
    //     if (zone != null && zone.gameObject != null)
    //         Destroy(zone.gameObject);
    // }

    // public void UseInItem(Item item)
    // {
    //     Debug.Log("Hammer used in egg/brainrot: " + item.name);
    //     InventorySystem.Instance.AddItem(item, 1);
    // }
}

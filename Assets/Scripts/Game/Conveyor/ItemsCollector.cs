using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCollector : MonoBehaviour, ITriggerHandler
{
    public void HandleTrigger(Collider other)
    {
        if (other.TryGetComponent<Item>(out var item))
            item.ReturnToPool();
    }
}

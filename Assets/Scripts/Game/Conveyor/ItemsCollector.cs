using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCollector : ITriggerEnterHandler
{
    public void HandleTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Item>(out var item))
            item.ReturnToPool();
    }
}

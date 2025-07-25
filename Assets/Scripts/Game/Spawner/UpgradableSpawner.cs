using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradableSpawner  : MonoBehaviour
{
    [SerializeField] protected SpawnerConfig config;
    [SerializeField] protected Transform spawnPoint;
    
    protected int CurrentLevel = 1;
    
    private readonly Dictionary<int, ObjectPoolManager<Item>> _poolMap = new();


    private ObjectPoolManager<Item> GetPoolForCurrentItem()
    {
        var egg = config.GetRandomEgg();
        foreach (var poolItem in _poolMap)
            if (poolItem.Key == egg.Id)
                return poolItem.Value;
        var newPool = new ObjectPoolManager<Item>(egg, 10, 50);
        _poolMap.Add(egg.Id, newPool);
        return newPool;
    }

    protected Item SpawnToPoint(Quaternion rotation = default)
    {
        var pool = GetPoolForCurrentItem();
        if (pool == null) return null;

        Egg item = (Egg)pool.Get();

        item.Rarity = config.GetRandomRarity();
        item.Variant = config.GetRandomVariant();

        item.transform.SetPositionAndRotation(spawnPoint.position, rotation);
        item.transform.gameObject.SetActive(true);
        return item;
    }
}

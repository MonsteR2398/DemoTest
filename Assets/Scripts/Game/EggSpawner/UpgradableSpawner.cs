using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradableSpawner  : MonoBehaviour
{
    [SerializeField] protected SpawnerConfig config;
    [SerializeField] protected Transform spawnPoint;
    [SerializeField] private ConveyorManager conveyorManager;

    [SerializeField] private bool _nowEnemy;
    
    protected int CurrentLevel = 1;
    
    private readonly Dictionary<string, Queue<Egg>> _poolMap = new();

    private bool canLegendarySpawned = false;
    private bool canMythicSpawned = false;

    private void OnEnable()
    {
        TextDisplayEvents.LegendaryEggCanBeSpawned += CanLegendarySpawned;
        TextDisplayEvents.MythicEggCanBeSpawned += CanMythicSpawned;
    }

    private void OnDisable()
    {
        TextDisplayEvents.LegendaryEggCanBeSpawned -= CanLegendarySpawned;
        TextDisplayEvents.MythicEggCanBeSpawned -= CanMythicSpawned;
    }

    private void CanLegendarySpawned(bool ready = true) => canLegendarySpawned = ready;

    private void CanMythicSpawned(bool ready = true) => canMythicSpawned = ready;



    protected Egg SpawnToPoint(Quaternion rotation = default)
    {
        Item eggItem = null;
        
        // Handle special egg spawns
        if (canLegendarySpawned)
        {
            eggItem = config.GetEggByType(EggType.Legendary);
            canLegendarySpawned = false;
        }
        else if (canMythicSpawned)
        {
            eggItem = config.GetEggByType(EggType.Mythical);
            canMythicSpawned = false;
        }
        else
        {
            eggItem = config.GetRandomItem();
        }
        
        if (eggItem == null) 
        {
            Debug.LogError("Egg not found in config!");
            return null;
        }
        
        string itemID = eggItem.GetItemID();
        Egg egg;
        
        if (_poolMap.TryGetValue(itemID, out Queue<Egg> pool) && pool.Count > 0)
        {
            egg = pool.Dequeue();
        }
        else
        {
            egg = Instantiate(eggItem) as Egg;
            if (egg == null) 
            {
                Debug.LogError($"Failed to create egg of type {eggItem.GetType()}");
                return null;
            }
        }
        egg.SetNowEnemy(_nowEnemy);
        egg.Data.Rarity = config.GetRandomRarity();
        egg.Data.Variant = config.GetRandomVariant();
        egg.transform.SetPositionAndRotation(spawnPoint.position, rotation);
        egg.gameObject.SetActive(true);

        TextDisplayEvents.RaiseDisplayEnabled(egg);

        if (conveyorManager != null)
            conveyorManager.AddMovable(egg);

        return egg;
    }
    
    public void ReturnEggToPool(Egg egg)
    {
        string itemID = egg.GetItemID();
        
        if (!_poolMap.TryGetValue(itemID, out Queue<Egg> pool))
        {
            pool = new Queue<Egg>();
            _poolMap[itemID] = pool;
        }
        TextDisplayEvents.RaiseDisplayDisabled(egg);
        egg.gameObject.SetActive(false);
        pool.Enqueue(egg);
    }
}

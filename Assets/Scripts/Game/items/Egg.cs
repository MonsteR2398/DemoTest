using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class EggData
{
    public string UniqueId { get; private set; }
    public int Id;
    public EggType EggType;
    public int OpenPrice;
    public bool HasActiveTimer;
    public float SpawnTimer;
    [HideInInspector] public bool HasSpawned;
    [HideInInspector] public Vector3 Position;
    [HideInInspector] public Rarity Rarity;
    [HideInInspector] public Variant Variant;

    [HideInInspector] public string LastSaveTimeString;

    public void UpdateLastSaveTimeString() => LastSaveTimeString = DateTime.Now.ToBinary().ToString();
    public string SetUniqueId(string value) => UniqueId = value;
    public void AddUniqueId()
    {
        if (string.IsNullOrEmpty(UniqueId))
            SetUniqueId(System.Guid.NewGuid().ToString());
    }
}

public class Egg : Item, IConveyorMovable, ITimer, IBuyable, ISpawned
{
    public EggData Data;
    public bool IsOnConveyor { get; set; }
    public bool HasSpawned 
    {
        get => Data.HasSpawned;
        set => Data.HasSpawned = value;
    }

    private IConveyor _currentConveyor;


    // public void OnSpawn()
    // {
    //     Data.SpawnTimer = 0;
    //     Data.HasActiveTimer = false;
    //     IsOnConveyor = false;
    //     _currentConveyor = null;
    //     TextDisplayEvents.RaiseDisplayEnabled(this);
    // }


    public override void OnSpawnToGround()
    {
        Data.HasSpawned = true;
        base.OnSpawnToGround();
        Data.Position = transform.position;
    }

    public override Variant GetVariant() => Data.Variant;
    // IBuyable implementation

    public string GetItemName() => _itemName;
    public override int GetPrice() => Data.HasSpawned ? Data.OpenPrice : _price;
    public bool CanBuy() => _canBuy;
    public bool SetCanBuy(bool value) => _canBuy = value;
    public bool HasActiveTimer() => Data.HasActiveTimer;

    public void Buy()
    {
        if (!CanBuy()) return;

        if (NowEnemy)
        {
            Debug.Log($"Украдено {Data.EggType} яйцо!");
            return;
        }

        if (!Data.HasSpawned)
            Debug.Log($"Куплено {Data.EggType} яйцо за {_price} монет");
        else
            OpenEgg();
    }

    public override string GetTextOnDisplay()
    {
        if (Data.HasActiveTimer)
            return Data.SpawnTimer > 0 ? $"{Data.SpawnTimer:F0} секунд" : "!";
        else
            if (Data.HasSpawned) return "";

        
        var variant = $"<style=Variant{Data.Variant}>{Data.Variant}</style>\n";
        if (Data.Variant == Variant.Default) variant = "";
        var rarity = $"<style=Rarity{Data.Rarity}>{Data.Rarity}</style>\n";
        var name = $"{Data.EggType} egg\n";
        var price = $"<style=Price>${GetPrice()}</style>\n";

        return variant + rarity + name + price;
    }

    public void ActivateTimer()
    {
        Data.HasActiveTimer = true;
        TextDisplayEvents.RaiseDisplayEnabled(this);
        StartCoroutine(Timer());
    }

    public void GetOfflineTime()
    {
        if (!string.IsNullOrEmpty(Data.LastSaveTimeString))
        {
            DateTime lastSaveTime = DateTime.FromBinary(Convert.ToInt64(Data.LastSaveTimeString));
            TimeSpan offlineTime = DateTime.Now - lastSaveTime;
            Data.SpawnTimer -= (long)offlineTime.TotalSeconds;
        }
        Data.UpdateLastSaveTimeString();
    }

    private IEnumerator Timer()
    {
        while (Data.SpawnTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            Data.SpawnTimer--;
        }
        Data.HasActiveTimer = false;
    }


    public void OpenEgg()
    {
        if (!string.IsNullOrEmpty(Data.UniqueId))
            LoadInitializer.Instance.SaveLoadManager.RemoveEggByUniqueId(Data.UniqueId);
        
        this.GetComponent<BrainrotSpawner>().Spawn(Data.Variant);
        Debug.Log($"Реализация открытия {Data.Rarity} яйца");
    }

    public override string GetItemID() => $"{Data.EggType}{Data.Rarity}{Data.Variant}";

    public void MoveToTargetOnConveyor(Vector3 target, float speed)
    {
        float step = speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }

    public void SetConveyor(IConveyor conveyor) => _currentConveyor = conveyor;

    public override void OnReturnToPool()
    {
        IsOnConveyor = false;
        _currentConveyor?.RemoveMovable(this);
        _currentConveyor = null;
            if (Data.HasSpawned)
        _canBuy = false;
    }
}

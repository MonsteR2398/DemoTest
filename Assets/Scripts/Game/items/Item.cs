using System;
using UnityEngine;

public class Item : PoolableMono<Item>, IObjectTextDisplay
{
    [SerializeField] protected string _itemName;
    [SerializeField] protected int _price;

    [SerializeField] protected Vector3 _displayOffset;

    [SerializeField] protected Sprite _icon;
    protected bool _canBuy = true;
    public bool NowEnemy = false;


    public virtual void OnSpawnToGround()
    {
    }

    public override void OnGetFromPool()
    {
    }
    
    public void SetNowEnemy(bool value) => NowEnemy = value; 
    public virtual Sprite GetIcon() => _icon;
    public virtual string GetName() => _itemName;
    public virtual int GetPrice() => _price;



    // IObjectTextDisplay implementation
    public Vector3 GetPosition() => transform.position;
    public virtual string GetTextOnDisplay() => _itemName;
    public virtual bool IsAlwaysVisible() => false;
    public virtual bool ShouldOrientToCamera() => true;

    // Added for Inventory System
    public virtual string GetItemID() => _itemName;

}

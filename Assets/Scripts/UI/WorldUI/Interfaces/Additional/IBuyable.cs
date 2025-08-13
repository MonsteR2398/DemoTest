using UnityEngine;

public interface IBuyable
{
    string GetItemName();
    int GetPrice();
    bool CanBuy();
    bool SetCanBuy(bool value);
    void Buy();
    Vector3 GetPosition();
}

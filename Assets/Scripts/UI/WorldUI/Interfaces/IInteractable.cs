using UnityEngine;
public interface IInteractable
{
    Vector3 GetPosition();
    DisplayData GetDisplayData();
    bool IsAlwaysVisible();
}

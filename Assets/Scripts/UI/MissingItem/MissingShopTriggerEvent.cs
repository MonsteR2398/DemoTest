using System;
using UnityEngine;

public class MissingShopTriggerEvent : MonoBehaviour, ITriggerEnterHandler, ITriggerExitHandler
{
    public static event Action OnMissingShopTriggerEnter;
    public static event Action OnMissingShopTriggerExit;

    public void HandleTriggerEnter(Collider other)
    {
        OnMissingShopTriggerEnter.Invoke();
    }
    
    public void HandleTriggerExit(Collider other)
    {
        OnMissingShopTriggerExit.Invoke();
    }
}

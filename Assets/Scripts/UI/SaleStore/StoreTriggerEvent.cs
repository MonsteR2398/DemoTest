using System;
using UnityEngine;

public class StoreTriggerEvent : MonoBehaviour, ITriggerEnterHandler, ITriggerExitHandler
{
    public static event Action OnTriggerEnter;
    public static event Action OnTriggerExit;

    public void HandleTriggerEnter(Collider other)
    {
        OnTriggerEnter.Invoke();
    }
    
    public void HandleTriggerExit(Collider other)
    {
        OnTriggerExit.Invoke();
    }
}

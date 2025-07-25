using System;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorManager : MonoBehaviour, IConveyor, ITriggerHandler
{
    [SerializeField] private Transform moveTarget;
    [SerializeField] private float speed = 5f;
    private readonly HashSet<IConveyorMovable> _movables = new();
    
    
    private void FixedUpdate()
    {
        foreach (var movable in _movables) movable.MoveToTargetOnConveyor(moveTarget.position, speed);
    }
    
    public void HandleTrigger(Collider other)
    {
        if (other.TryGetComponent<IConveyorMovable>(out var movable))
        {
            AddMovable(movable);
            movable.SetConveyor(this);
        }
    }
    public void AddMovable(IConveyorMovable movable)
    {
        movable.IsOnConveyor = true;
        _movables.Add(movable);
    }
    public void RemoveMovable(IConveyorMovable movable)
    {
        movable.IsOnConveyor = false;
        _movables.Remove(movable);
    }
    private void OnDestroy()
    {
        foreach (var movable in _movables)
            movable.SetConveyor(null);
    }
}

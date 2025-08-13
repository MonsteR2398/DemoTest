using System;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorManager : MonoBehaviour, IConveyor
{
    [SerializeField] private Transform moveTarget;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float arrivalThreshold = 0.1f;
    private readonly HashSet<IConveyorMovable> _movables = new();
    private readonly List<IConveyorMovable> _reachedMovables = new();

    [SerializeField] private MissingItemShop _missingItemShop;
    [SerializeField] private UpgradableSpawner _spawner;


    private void FixedUpdate()
    {
        _reachedMovables.Clear();
        
        foreach (var movable in _movables)
        {
            if ((movable as MonoBehaviour) == null) continue;
            
            movable.MoveToTargetOnConveyor(moveTarget.position, speed);
            
            if (Vector3.Distance((movable as MonoBehaviour).transform.position, moveTarget.position) < arrivalThreshold)
                _reachedMovables.Add(movable);
        }
        
        foreach (var movable in _reachedMovables)
            HandleMovableReachedTarget(movable);
    }
    
    private void HandleMovableReachedTarget(IConveyorMovable movable)
    {
        RemoveMovable(movable);
        
        if (movable is Egg egg)
        {
            if (_missingItemShop != null && !egg.NowEnemy)
                _missingItemShop.AddItemInShop(egg);
            
            if (_spawner != null)
                _spawner.ReturnEggToPool(egg);
        }
    }
    
    public void AddMovable(IConveyorMovable movable)
    {
        movable.IsOnConveyor = true;
        _movables.Add(movable);
        movable.SetConveyor(this);
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

using UnityEngine;
public interface IConveyorMovable 
{
    void MoveToTargetOnConveyor(Vector3 target, float speed);
    void SetConveyor(IConveyor conveyor);
    bool IsOnConveyor { get; set; }
    bool CanCollect { get; set; }
}

public interface IConveyor 
{
    void AddMovable(IConveyorMovable movable);
    void RemoveMovable(IConveyorMovable movable);
}
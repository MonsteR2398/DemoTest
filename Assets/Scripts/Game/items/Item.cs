using UnityEngine;

public abstract class Item : PoolableMono<Item>, IConveyorMovable, IObjectTextDisplay
{
    public int Price;
    private IConveyor _currentConveyor;
    private Vector3 _verticalVelocity;
    private Transform _transform;

    public bool IsOnConveyor { get; set; }
    public bool CanCollect { get; set; } = true;
    public bool IsGrounded { get; private set; }

    protected void Awake()
    {
        _transform = transform;
    }
    public int GetPrice() => Price;

    public void SetPrice(int price) => Price = price;

    public void ApplyGravity(Vector3 direction, float force, float deltaTime)
    {
        if (!IsOnConveyor)
        {
            var gravityThisFrame = force * deltaTime;
            _verticalVelocity.y += gravityThisFrame;
            _transform.position -= _verticalVelocity;
        }
        else
        {
            _verticalVelocity = Vector3.zero;
        }
    }
    public void SetConveyor(IConveyor conveyor) => _currentConveyor = conveyor;

    public override void OnReturnToPool()
    {
        _verticalVelocity = Vector3.zero;
        IsGrounded = false;
        IsOnConveyor = false;

        _currentConveyor?.RemoveMovable(this);
        _currentConveyor = null;
    }

    public void MoveToTargetOnConveyor(Vector3 target, float speed)
    {
        float step = speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }

    //IObjectTextDisplay
    public virtual string GetTextOnDisplay() => Price.ToString();
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythical
}

public enum Variant
{
    Default,
    Golden,
    Diamond
}


using UnityEngine;
using Assets.Scripts.Game.Player;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(MovementHandler))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementHandler _movementHandler;
    private IMovementInput _inputHandler;

    private void Awake()
    {
        // Ищем джойстик на канвасе, а не на персонаже
        _inputHandler = FindObjectOfType<JoystickInput>();
        
        if (_inputHandler == null)
            _inputHandler = gameObject.AddComponent<JoystickInput>();
    }

    private void FixedUpdate()
    {
        Vector2 input = _inputHandler.GetMovementDirection();
        _movementHandler.Move(input);
    }
}

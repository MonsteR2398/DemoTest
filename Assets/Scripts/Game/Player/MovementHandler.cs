using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementHandler : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private Transform _cameraTransform;
    
    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isGrounded;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void Move(Vector2 inputDirection)
    {
        _isGrounded = _controller.isGrounded;
        
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        Vector3 moveDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
        Vector3 cameraForward = Vector3.Scale(_cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        moveDirection = moveDirection.x * _cameraTransform.right + moveDirection.z * cameraForward;
        _controller.Move(moveDirection * _moveSpeed * Time.deltaTime);

        if (inputDirection != Vector2.zero && _cameraTransform != null)
        {
            Vector3 joystickDirection = new Vector3(inputDirection.x, 0, inputDirection.y);
            Quaternion cameraRotation = Quaternion.LookRotation(cameraForward);
            Vector3 worldDirection = cameraRotation * joystickDirection;

            Quaternion targetRotation = Quaternion.LookRotation(worldDirection);
            transform.rotation = targetRotation;
        }

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
        
    }
}

using System;
using Interfaces;
using Player;
using States;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, 
    IMovementController,
    IRotationController,
    IAnimationController,
    IInputProvider,
    ICalculator
{
    [SerializeField] private PlayerSettings settings = new PlayerSettings();
    [SerializeField] private Camera pCamera;
    [SerializeField] private Animator pAnimator;
    
    private PlayerMovementFsm _fsm;
    private CharacterController _characterController;
    private Vector3 _verticalVelocity;
    private float _currentRotationVelocity;
    private float _currentMoveSpeed;
    private bool _isGrounded;

    public ICharacterData Data => settings;
    
    private void Awake()
    {
        if (settings == null)
        {
            Debug.LogError("PlayerSettings not assigned");
            enabled = false;
            return;
        }
        InitializeComponents();
        InitializeStateMachine();
    }
    private void FixedUpdate()
    {
        _isGrounded = _characterController.isGrounded;
        _fsm?.FixedUpdate();
    }

    private void Update()
    {
        _fsm?.Update();
    }
    
    #region Initialization
    private void InitializeComponents()
    {
        _characterController = GetComponent<CharacterController>();
        
        if (TryGetComponent(out Camera playerCamera))
            pCamera = playerCamera;
        if (pCamera != null) return;
        pCamera = Camera.main;
        Debug.LogError("Camera not found");
        enabled = false;
    }
    private void InitializeStateMachine()
    {
        var context = new PlayerContext(
            movement: this,
            rotation: this,
            animation: this,
            input: this,
            calculator: this,
            data: Data
        );
        
        var fsmInitializer = new PlayerFsmInitializer();
        _fsm = fsmInitializer.CreateFsm(context);
    }
    
    #endregion

    #region IMovementController
    public void Move(Vector3 direction) => _characterController.Move(direction * (_currentMoveSpeed * Time.fixedDeltaTime));
    
    public void SetMoveSpeed(float speed = 1) => _currentMoveSpeed = speed;
    
    public void ApplyGravity(float multiplier)
    {
        if (!_isGrounded)
        {
            float gravityThisFrame = Physics.gravity.y * multiplier * Time.fixedDeltaTime;
            _verticalVelocity.y += gravityThisFrame;
        }
        _characterController.Move(_verticalVelocity * Time.fixedDeltaTime);
    }

    public bool IsGrounded => _isGrounded;
    public Vector3 Velocity { get; set; }
    #endregion

    #region IRotationController
    public void Rotate(Quaternion rotation) => transform.rotation = rotation;
    #endregion

    #region IInputProvider
    public Vector3 GetMoveInputRaw() => new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    public bool JumpPressed => Input.GetButtonDown("Jump");
    public bool RunPressed => Input.GetKey(KeyCode.LeftShift);
    #endregion

    #region ICalculator
    public Vector3 CalculateMoveDirection(Vector3 input, float speedMultiplier = 1f)
    {
        return Quaternion.Euler(0, GetCameraRelativeDirection(input), 0) * Vector3.forward;
    }

    public Quaternion CalculateRotateDirection(Vector3 input, float speedMultiplier = 1f)
    {
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, GetCameraRelativeDirection(input),
            ref _currentRotationVelocity, 0.1f, Data.RotationSpeed
        );
        return Quaternion.Euler(0f, angle, 0f);
    }

    public float GetCameraRelativeDirection(Vector3 input)
    {
        var targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + pCamera.transform.eulerAngles.y;
        return targetAngle;
    }

    public Vector3 CalculateJumpVelocity(Vector3 input, float speedMultiplier = 1f)
    {
        return Quaternion.Euler(0f, GetCameraRelativeDirection(input), 0f) * Vector3.forward * Data.JumpForce;
    }
    #endregion

    #region IAnimationController
     public void SetBool(string param, bool value) => pAnimator.SetBool(param, value);
     public void SetTrigger(string param) => pAnimator.SetTrigger(param);
     public void SetFloat(string param, float value) => pAnimator.SetFloat(param, value);
     public float GetFloat(string param) => pAnimator.GetFloat(param);

    #endregion
}
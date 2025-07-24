using Interfaces;
using UnityEngine;

namespace States
{
    public class PlayerJumpState : PlayerMovementBaseState
    {

        private Vector3 _moveDirection;        
        private float _verticalVelocity;
        private Vector3 _moveInputRaw;

        public PlayerJumpState(
            PlayerMovementFsm fsm, PlayerContext context) 
            : base(fsm, context) { }
        
        public override void EnterState()
        {
            _moveDirection = InputProvider.GetMoveInputRaw().magnitude > 0.1f 
                ? Calculator.CalculateMoveDirection(InputProvider.GetMoveInputRaw())
                : Vector3.zero;
            _verticalVelocity = Data.JumpForce;
            Animation.SetBool("OnJump", true);
        }
        public override void ExitState()
        {
            Animation.SetBool("OnJump", false);
        }

        public override void FixedUpdateState()
        {
            _moveDirection.y = _verticalVelocity;
           Movement.Move(_moveDirection);
           
           //rotate
           var rotateDirection = Calculator.CalculateRotateDirection(_moveInputRaw);
           Rotation.Rotate(rotateDirection);
        }

        public override void UpdateState()
        {
            _moveInputRaw = InputProvider.GetMoveInputRaw();
            var rotateDirection = Calculator.CalculateRotateDirection(_moveInputRaw);
            Rotation.Rotate(rotateDirection);
        }

        protected override void CheckTransitions()
        {
            if (_verticalVelocity > 0.1) return;
            if (!Movement.IsGrounded) return;
            if (InputProvider.GetMoveInputRaw().magnitude >= 0.1f)
            {
                if (InputProvider.RunPressed)
                    Fsm.SetState<PlayerRunState>();
                else
                    Fsm.SetState<PlayerWalkState>();
            }
            else
            {
                Fsm.SetState<PlayerIdleState>();
            }
        }
        protected override void HandleGravity() => _verticalVelocity += Physics.gravity.y * Time.fixedDeltaTime;
    }
}
using UnityEngine;

namespace States
{
    public class PlayerRunState : PlayerMovementBaseState
    {
        private readonly float _runSpeed;

        private Vector3 _moveInputRaw;

        public PlayerRunState(
            PlayerMovementFsm fsm, PlayerContext context)
            : base(fsm, context)
        {
            _runSpeed = Data.RunSpeed;
        }

        public override void EnterState()
        {
            Animation.SetFloat("Speed", 1.5f);
            Animation.SetBool("OnRun", true);
        }

        public override void ExitState()
        {
            Animation.SetBool("OnRun", false);
            Animation.SetFloat("Speed", 0f);
        }

        public override void FixedUpdateState()
        {
            var moveDirection = Calculator.CalculateMoveDirection(_moveInputRaw);
            Movement.SetMoveSpeed(_runSpeed);
            Movement.Move(moveDirection);
        }
        public override void UpdateState()
        {
            _moveInputRaw = InputProvider.GetMoveInputRaw();
            var rotateDirection = Calculator.CalculateRotateDirection(_moveInputRaw);
            Rotation.Rotate(rotateDirection);
        }
        protected override void CheckTransitions()
        {
            if (!InputProvider.RunPressed)
            {
                if (_moveInputRaw.magnitude >= 0.1f)
                    Fsm.SetState<PlayerWalkState>();
                else
                    Fsm.SetState<PlayerIdleState>();
            }
            
            if (Movement.IsGrounded && InputProvider.JumpPressed)
                Fsm.SetState<PlayerJumpState>();


        }
    }
}
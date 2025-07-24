using UnityEngine;

namespace States
{
    public class PlayerWalkState : PlayerMovementBaseState
    {
        private readonly float _walkSpeed;
        private Vector3 _moveInputRaw;

        public PlayerWalkState(
            PlayerMovementFsm fsm, PlayerContext context)
            : base(fsm, context)
        {
            _walkSpeed = Data.WalkSpeed;
        }

        public override void EnterState()
        {
            Animation.SetFloat("Speed", 1f);
            Animation.SetBool("OnWalk", true);
        }

        public override void ExitState()
        {
            Animation.SetBool("OnWalk", false);
            Animation.SetFloat("Speed", 0f);
        }

        public override void FixedUpdateState()
        {
            Movement.SetMoveSpeed(_walkSpeed);
            Movement.Move(Calculator.CalculateMoveDirection(_moveInputRaw));
        }
        public override void UpdateState()
        {
            _moveInputRaw = InputProvider.GetMoveInputRaw();
            var rotateDirection = Calculator.CalculateRotateDirection(_moveInputRaw);
            Rotation.Rotate(rotateDirection);
        }
        protected override void CheckTransitions()
        {
            if (!Movement.IsGrounded) return;
            if (_moveInputRaw.magnitude < 0.1f)
                Fsm.SetState<PlayerIdleState>();
            else
                if (InputProvider.RunPressed)
                    Fsm.SetState<PlayerRunState>();

            if (InputProvider.JumpPressed)
                Fsm.SetState<PlayerJumpState>();
        }
    }
}
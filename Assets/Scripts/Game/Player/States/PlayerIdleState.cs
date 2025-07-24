using UnityEngine;

namespace States
{
    public class PlayerIdleState : PlayerMovementBaseState
    {
        private readonly PlayerContext _context;
        public PlayerIdleState(PlayerMovementFsm fsm, PlayerContext context) 
            : base(fsm, context) { }

        public override void EnterState()
        {
            Animation.SetBool("OnIdle", true);
        }

        public override void ExitState()
        {
            Animation.SetBool("OnIdle", false);
        }

        protected override void CheckTransitions()
        {
            if (InputProvider.GetMoveInputRaw().magnitude >= 0.1f)
            {
                if (InputProvider.RunPressed)
                    Fsm.SetState<PlayerRunState>();
                else
                    Fsm.SetState<PlayerWalkState>();
            }
            else if (InputProvider.JumpPressed && Movement.IsGrounded)
                Fsm.SetState<PlayerJumpState>();
        }
    }
}
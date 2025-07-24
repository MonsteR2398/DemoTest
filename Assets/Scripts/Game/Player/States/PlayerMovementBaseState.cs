using Interfaces;
using UnityEngine;

namespace States
{
    public abstract class PlayerMovementBaseState : IState
    {
        protected readonly IMovementController Movement;
        protected readonly IRotationController Rotation;
        protected readonly IAnimationController Animation;
        protected readonly IInputProvider InputProvider;
        protected readonly ICalculator Calculator;
        protected readonly ICharacterData Data;
        protected readonly PlayerMovementFsm Fsm;

        // Commit

        protected PlayerMovementBaseState(PlayerMovementFsm fsm, PlayerContext context)
        {
            Fsm = fsm;
            Movement = context.Movement;
            Rotation = context.Rotation;
            Animation = context.Animation;
            InputProvider = context.Input;
            Calculator = context.Calculator;
            Data = context.Data;
        }
        
        protected virtual void HandleGravity()
        {
            Movement.ApplyGravity(1f);
        }

        public virtual void EnterState() { }
        public virtual void ExitState() { }
        public virtual void UpdateState()
        {
            CheckTransitions();
        }
        public virtual void FixedUpdateState()
        { 
            HandleGravity();
        }
        protected abstract void CheckTransitions();
    }

}

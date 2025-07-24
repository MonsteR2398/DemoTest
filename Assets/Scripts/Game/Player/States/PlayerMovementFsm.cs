using System;
using System.Collections.Generic;

namespace States
{
    public class PlayerMovementFsm
    {
        private IState currentState;
        private Dictionary<Type, IState> _states = new Dictionary<Type, IState>();

        public void AddState(IState state)
        {
            _states.Add(state.GetType(), state);
        }
        
        public IState GetCurrentState() => currentState;

        public void SetState<T>() where T : IState
        {
            if (_states.TryGetValue(typeof(T), out var newState))
            {
                currentState?.ExitState();
                currentState = newState;
                currentState.EnterState();
            }
        }

        public void FixedUpdate() => currentState?.FixedUpdateState();
        public void Update() => currentState?.UpdateState();
    }
}
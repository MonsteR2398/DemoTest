using System;
using System.Collections.Generic;

public class ModeFSM
{
    private IState currentState;
    private Dictionary<Type, IState> _states = new Dictionary<Type, IState>();

    public void AddState(IState state) => _states.Add(state.GetType(), state);
    public IState GetCurrentState() => currentState;
    
    public List<IState> GetStates() => new List<IState>(_states.Values);

    public void SetState<T>() where T : IState => SetState(typeof(T));
    
    public void SetState(Type stateType)
    {
        if (_states.TryGetValue(stateType, out var newState))
        {
            currentState?.ExitState();
            currentState = newState;
            currentState.EnterState();
        }
    }

    public void FixedUpdateState() => currentState?.FixedUpdateState();
    public void UpdateState() => currentState?.UpdateState();
}

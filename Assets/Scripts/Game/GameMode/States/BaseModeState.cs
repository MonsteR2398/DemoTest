using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseModeState : IState
{
    protected MonoBehaviour CoroutineRunner;
    protected List<Coroutine> ActiveCoroutines = new();
    public GameModeType ModeType { get; protected set; }
    protected ModeConfig _config;
    protected ObjectPoolManager<BaseObject> _objectPool;

    protected BaseModeState(ModeFSM fsm, ModeContext context)
    {
        CoroutineRunner = (MonoBehaviour)context.ModeController;
    }

    public virtual void EnterState()
    {
        ActiveCoroutines.Clear();
    }
    public virtual void ExitState()
    {
        foreach (var coroutine in ActiveCoroutines)
            if (coroutine != null)
                CoroutineRunner.StopCoroutine(coroutine);
    }
    public virtual void FixedUpdateState() { }
    public virtual void UpdateState()
    {
        CheckTransitions();
    }

    public virtual void Initialize(ModeConfig config)
    {
        _config = config;
        ModeType = config.ModeType;
    }

    protected abstract void CheckTransitions();
    
    protected Coroutine StartManagedCoroutine(IEnumerator routine)
    {
        var coroutine = CoroutineRunner.StartCoroutine(routine);
        ActiveCoroutines.Add(coroutine);
        return coroutine;
    }
}

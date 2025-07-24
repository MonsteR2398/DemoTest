using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeController : MonoBehaviour
{
    [SerializeField] private float _modeSwitchInterval = 10f;
    [SerializeField] private ModeConfigs _modeConfigs;
    private ModeFSM _fsm;
    private List<IState> _allStates;

    void Awake()
    {
        InitializeStateMachine();
    }

    private void Start()
    {
        SwitchToRandomState();
        StartCoroutine(ModeSwitchTimer());
    }

    private void Update()
    {
        _fsm?.UpdateState();
    }

    private void FixedUpdate()
    {
        _fsm?.FixedUpdateState();
    }

    private void InitializeStateMachine()
    {
        var context = new ModeContext(this);
        var fsmInitializer = new ModeFSMInitializer();
        _fsm = fsmInitializer.CreateFsm(context);
        _allStates = _fsm.GetStates();
    }

    public ModeConfigs GetConfigs() => _modeConfigs;
    private void SwitchToRandomState()
    {
        var randomState = _allStates[Random.Range(0, _allStates.Count)];
        _fsm.SetState(randomState.GetType());
    }

    private IEnumerator ModeSwitchTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(_modeSwitchInterval);
            SwitchToRandomState();
        }
    }
}

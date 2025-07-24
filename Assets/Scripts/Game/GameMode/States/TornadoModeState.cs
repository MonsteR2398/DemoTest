using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TornadoModeState : BaseModeState
{
    private TornadoConfig _data;
    private float _timeOffsetX, _timeOffsetZ;
    private BaseObject tornadoObj;
    private Renderer tornadoRenderer;

    public TornadoModeState(ModeFSM fsm, ModeContext context)
        : base(fsm, context)
    {
        _data = (TornadoConfig)context.ModeController.GetConfigs().GetConfig(GameModeType.Tornado);
        _objectPool = new ObjectPoolManager<BaseObject>(_data.prefab);
    }

    public override void EnterState()
    {
        base.EnterState();

        _timeOffsetX = Random.Range(-_data.range, _data.range);
        _timeOffsetZ = Random.Range(-_data.range, _data.range);

        tornadoObj = _objectPool.Get();
        tornadoRenderer = tornadoObj.transform.GetComponent<Renderer>();

        Debug.Log("Торнадо приблежается!");


    }

    public override void ExitState()
    {
        base.ExitState();
        tornadoObj.ReturnToPool();
        Debug.Log("Торнадо закончился.");
    }

    public override void FixedUpdateState()
    {
        float x = Mathf.PerlinNoise(Time.time * _data.noiseScale, _timeOffsetX) * 2 - 1;
        float z = Mathf.PerlinNoise(_timeOffsetZ, Time.time * _data.noiseScale) * 2 - 1;
        float y = Mathf.Sin(Time.time) * 0.2f;

        float currentRadius = 2f + Mathf.PerlinNoise(Time.time * 0.3f, 0) * 1f;

        Vector3 newPos = _data.position + new Vector3(x, y, z).normalized * currentRadius;
        tornadoObj.transform.position = Vector3.Lerp(tornadoObj.transform.position, newPos, Time.deltaTime * 2f);
        
        tornadoRenderer.material.color = Color.Lerp(Color.gray, Color.white, Mathf.PingPong(Time.time, 1f));
    }

    protected override void CheckTransitions()
    {
        // тут возможная реализация переходов между состояниями 
        // if (...)
        // Fsm.SetState<MeteorState>();
    }
}
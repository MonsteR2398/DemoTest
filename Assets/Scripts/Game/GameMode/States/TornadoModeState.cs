using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TornadoModeState : BaseModeState
{
    private TornadoConfig _data;
    private float _offsetX, _offsetZ;
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

        _offsetX = Random.Range(-_data.range, _data.range);
        _offsetZ = Random.Range(-_data.range, _data.range);

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
        float x = Mathf.PerlinNoise(Time.time * _data.noiseScale, _offsetX) * 2 - 1;
        float z = Mathf.PerlinNoise(_offsetZ, Time.time * _data.noiseScale) * 2 - 1;
        float y = Mathf.Sin(Time.time) * _data.heightVariation;

         Vector3 newPos = _data.position + new Vector3(x, y, z) * _data.moveSpeed;
            newPos.x = Mathf.Clamp(newPos.x, -_data.range, _data.range);
            newPos.z = Mathf.Clamp(newPos.z, -_data.range, _data.range);
        tornadoObj.transform.position = Vector3.Lerp(tornadoObj.transform.position, newPos, Time.deltaTime);
        
        tornadoRenderer.material.color = Color.Lerp(Color.gray, Color.white, Mathf.PingPong(Time.time, 1f));
    }

    protected override void CheckTransitions()
    {
        // тут возможная реализация переходов между состояниями 
        // if (...)
        // Fsm.SetState<MeteorState>();
    }
}
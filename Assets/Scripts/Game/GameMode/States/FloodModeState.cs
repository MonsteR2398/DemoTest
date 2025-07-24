using System.Collections;
using UnityEngine;

public class FloodModeState : BaseModeState
{
    private FloodConfig _data;
    private BaseObject floodObj;

    private float _elapsedTime = 0f;

    public FloodModeState(ModeFSM fsm, ModeContext context)
        : base(fsm, context)
    {
        _data = (FloodConfig)context.ModeController.GetConfigs().GetConfig(GameModeType.Flood);
        _objectPool = new ObjectPoolManager<BaseObject>(_data.prefab);
    }

    public override void EnterState()
    {
        base.EnterState();
        floodObj = _objectPool.Get();
        floodObj.transform.SetPositionAndRotation(_data.areaPosition, Quaternion.identity);
        floodObj.transform.localScale = Vector3.one * _data.rangeFlood / 10;
        StartManagedCoroutine(MoveFloodUp());
#if UNITY_EDITOR
        floodObj.GetComponent<Renderer>().material.mainTextureScale = Vector3.one * _data.rangeFlood;
#endif
    
        Debug.Log("Вода начала подниматься!");
    }

    public override void ExitState()
    {
        base.ExitState();
        floodObj.ReturnToPool();
        Debug.Log("Вода исчезла.");
    }

    private IEnumerator MoveFloodUp()
    {
        float time = 0f;
        Vector3 startPos = _data.areaPosition;
        Vector3 endPos = Vector3.up * _data.endYFlood;

        while (time < _data.fillingTime)
        {
            float progress = Mathf.SmoothStep(0f, 1f, time / _data.fillingTime);
            floodObj.transform.position = Vector3.Lerp(startPos, endPos, progress);
            time += Time.deltaTime;
            yield return null;
        }
        floodObj.transform.position = endPos;
        StartManagedCoroutine(MoveFloodDown(endPos));
    }

    private IEnumerator MoveFloodDown(Vector3 startPosition)
    {
        float time = 0f;
        Vector3 startPos = startPosition;
        Vector3 endPos = Vector3.down * _data.areaPosition.y;

        while (time < _data.fillingTime)
        {
            float progress = Mathf.SmoothStep(0f, 1f, time / _data.fillingTime);
            floodObj.transform.position = Vector3.Lerp(startPos, endPos, progress);
            time += Time.deltaTime;
            yield return null;
        }
        floodObj.transform.position = endPos;
    }

    protected override void CheckTransitions()
    {
        // тут возможная реализация переходов между состояниями 
        // if (...)
        // Fsm.SetState<MeteorState>();
    }
}

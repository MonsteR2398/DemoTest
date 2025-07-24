using System.Collections;
using UnityEngine;

public class MeteorModeState : BaseModeState
{
    private MeteorConfig _data;
    public MeteorModeState(ModeFSM fsm, ModeContext context)
        : base(fsm, context)
    {
        _data = (MeteorConfig)context.ModeController.GetConfigs().GetConfig(GameModeType.Meteor);
        _objectPool = new ObjectPoolManager<BaseObject>(_data.prefab);
    }

    public override void EnterState()
    {
        base.EnterState();
        StartManagedCoroutine(SpawnMeteors());
        Debug.Log("Метеориды начали падать!");
    }

    public override void ExitState()
    {
        base.ExitState();
    }


    protected override void CheckTransitions()
    {
        // тут возможная реализация переходов между состояниями 
        // if (...)
        // Fsm.SetState<MeteorState>();
    }

    private IEnumerator SpawnMeteors()
    {
        while (true)
        {
            var item = _objectPool.Get();
            Vector3 spawnPos = GetRandomSpawnPosition(_data.position, _data.range);
            item.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(_data.spawnInterval);
        }
    }

    Vector3 GetRandomSpawnPosition(Vector3 position, float range)
    {
    float randomX = Random.Range(position.x - range / 2f, position.x + range / 2f);
    float randomZ = Random.Range(position.z - range / 2f, position.z + range / 2f);

    return new Vector3(randomX, position.y, randomZ);
    }

}

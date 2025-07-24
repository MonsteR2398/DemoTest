using UnityEngine;

[CreateAssetMenu(fileName = "MeteorConfig", menuName = "Game/Meteor Config")]
public class MeteorConfig : ModeConfig
{
    [Header("Параметры метеоритов")]
    public BaseObject prefab;
    public float spawnInterval = 2f;
    public int maxMeteors = 10;
    public float meteorSpeed = 1f;
    public float meteorDamage = 25f;

    [Header("Спавн")]
    public Vector3 position;
    public float range;

#if UNITY_EDITOR
    public override void DrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            new Vector3(position.x, position.y, position.z), 
            new Vector3(range, 0, range));
    }
#endif
}

using UnityEngine;

[CreateAssetMenu(fileName = "TornadoConfig", menuName = "Game/Tornado Config")]
public class TornadoConfig : ModeConfig
{
    public BaseObject prefab;

    [Header("Настройки движения")]
    public float moveSpeed = 2f;
    public float noiseScale = 0.5f;
    public float heightVariation = 1f;

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

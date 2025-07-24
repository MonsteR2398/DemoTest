using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloodConfig", menuName = "Game/Flood Config")]
public class FloodConfig : ModeConfig
{
    [Header("Параметры наводнения")]
    public BaseObject prefab;
    public float fillingTime = 5f;
    public float damage = 25f;

    [Header("Спавн")]
    public Vector3 areaPosition;
    public float rangeFlood;
    public float endYFlood;

    #if UNITY_EDITOR
    public override void DrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(areaPosition.x, endYFlood/2 + areaPosition.y, areaPosition.z), new Vector3(rangeFlood, endYFlood, rangeFlood));
    }
#endif
}
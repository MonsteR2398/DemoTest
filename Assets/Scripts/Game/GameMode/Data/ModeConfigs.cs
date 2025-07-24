using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameModeType { Meteor, Flood, Tornado }

public abstract class ModeConfig : ScriptableObject
{
    public GameModeType ModeType;
    public string ModeName;
    public Sprite ModeIcon;
    [TextArea] public string ModeDescription;

#if UNITY_EDITOR
    public virtual void DrawGizmos() { }
#endif
}

[CreateAssetMenu(fileName = "ModeConfigs", menuName = "Game/Mode Configs")]
public class ModeConfigs : ScriptableObject
{
    [SerializeField] private List<ModeConfig> _configurations = new();

    public ModeConfig GetConfig(GameModeType modeType)
    {
        foreach (var config in _configurations)
            if (config.ModeType == modeType) return config;
        return null;
    }
}

#if UNITY_EDITOR
using UnityEngine;
public class ModeVisualizer : MonoBehaviour
{
    [SerializeField] private ModeConfigs _configs;
    [SerializeField] private GameModeType _selectedMode;
    
    private void OnDrawGizmos()
    {
        if (_configs == null) return;
        
        var config = _configs.GetConfig(_selectedMode);
        if (config != null)
            config.DrawGizmos();
    }
}
#endif
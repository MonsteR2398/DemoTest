using UnityEngine;

public class WorldObjectUI : MonoBehaviour
{
    [Header("Настройки отображения")]
    public Vector3 uiOffset = new Vector3(0, 1.5f, 0);
    public bool alwaysVisible;
    public bool showInfo = true;
    public bool showProgress;
    public bool showBuy;
    public string displayText = "Объект";

    void Start() => WorldUIManager.Instance.RegisterObject(this);
    void OnDestroy() => WorldUIManager.Instance?.UnregisterObject(this);

    public Vector3 GetUIPosition() => transform.position + uiOffset;
}
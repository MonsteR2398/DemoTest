
using UnityEngine;

public abstract class UIDisplay : MonoBehaviour
{
    private void LateUpdate()
    {
        if (gameObject.activeInHierarchy)
            transform.rotation = Camera.main.transform.rotation;
    }

    //public abstract void Init(object target);
    //public abstract void UpdateDisplay();
    
    public void SetActive(bool state) => gameObject.SetActive(state);
    
    public void SetPosition(Vector3 worldPosition)
    {
        // Конвертируем мировые координаты в позицию на Canvas
        var canvasRect = WorldUIManager.Instance.worldCanvas.GetComponent<RectTransform>();
        var viewportPos = Camera.main.WorldToViewportPoint(worldPosition);

        var canvasPos = new Vector2(
            (viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
            (viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));

        GetComponent<RectTransform>().anchoredPosition = canvasPos;
    }
}
using UnityEngine;
using TMPro;

public class TextDisplay : UIDisplay
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;

    public string Text
    {
        get => _textMeshPro.text;
        set => _textMeshPro.text = value;
    }

    public void SetPosition(Vector3 worldPosition, Vector3 offset) =>
        transform.position = worldPosition + offset;

    public GameObject GetPrefab() => gameObject;
}

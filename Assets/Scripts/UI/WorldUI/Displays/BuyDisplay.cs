using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Button _buyButton;
    private RectTransform _rectTransform;
    private Transform _target;
    private Vector3 _worldOffset;

    private System.Action _onBuy;
    
    public Button GetButton => _buyButton;

    public void Initialize(System.Action onBuyCallback, Vector3 worldOffset)
    {
        _worldOffset = worldOffset;
        _onBuy = onBuyCallback;
        _buyButton.onClick.RemoveAllListeners();
        _buyButton.onClick.AddListener(() => _onBuy?.Invoke());
    }
    public void SetActive(bool state) => gameObject.SetActive(state);

    public void SetPosition(Vector3 worldPosition, Vector3 offset)
    {
        transform.position = worldPosition + offset;
    }

    public void SetCanBuy(bool canBuy)
    {
        _buyButton.interactable = canBuy;
    }

    
    public void SetText(string text)
    {
        _titleText.text = text;
    }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
    
    void LateUpdate()
    {
        if (_target == null) return;

        Vector3 worldPos = _target.position + _worldOffset;
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
        screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height);

        _rectTransform.position = screenPos;
    }
}

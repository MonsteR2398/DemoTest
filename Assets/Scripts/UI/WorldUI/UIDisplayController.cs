using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplayController : MonoBehaviour
{
    [Header("Ссылки")]
    public TMP_Text textField;
    public Image progressBar;
    public Button buyButton;

    private WorldObjectUI linkedObject;
    private IBuyable buyable;
    private IProgressable progressable;

    public void Initialize(WorldObjectUI obj)
    {
        linkedObject = obj;
        buyable = obj.GetComponent<IBuyable>();
        progressable = obj.GetComponent<IProgressable>();

        SetupDisplay();
    }

    void SetupDisplay()
    {
        // Базовая информация
        if (textField != null)
            textField.text = linkedObject.displayText;

        // Прогресс бар
        if (progressBar != null && progressable != null)
        {
            progressBar.fillAmount = progressable.Progress / progressable.MaxProgress;
        }

        // Кнопка покупки
        if (buyButton != null && buyable != null)
        {
            buyButton.onClick.AddListener(OnBuyClick);
            buyButton.interactable = buyable.CanBuy();
        }
    }

    void OnBuyClick()
    {
        if (buyable != null && buyable.CanBuy())
        {
            Destroy(gameObject);
        }
    }
}
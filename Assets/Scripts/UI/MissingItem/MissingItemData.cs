using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissingItemData : MonoBehaviour
{
    public Image IconImage;
    public Image IconBackground;
    public TextMeshProUGUI TimerText;
    public Button Button;
    public Item Item { get; set; }
    [NonSerialized] public ItemViewerData Viewer;

    public int Timer = 10;
    public int RemainingTime { get; set; }

    public void Setup(Item item)
    {
        Item = item;
        IconImage.sprite = item.GetIcon();
    }


    void OnEnable()
    {
        if (transform.GetSiblingIndex() != transform.parent.childCount - 1)
            transform.SetAsLastSibling();
    }

}

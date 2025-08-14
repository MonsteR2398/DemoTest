using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaleItemData : MonoBehaviour
{
    public Image IconImage;
    public Image BackgroundImage;
    public TextMeshProUGUI ItemSizeText;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI PriceText;
    public Button SellButton;
    
    public InventorySlot Slot { get; set; }
    public string SlotKey { get; set; }
}

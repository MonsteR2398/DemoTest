using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SaleStoreSystem : MonoBehaviour
{
    [SerializeField] private GameObject _storePanel;
    [SerializeField] private GameObject _itemParent;
    [SerializeField] private SaleItemData _itemPrefab;
    [SerializeField] private TextMeshProUGUI _allPriceText;
    [SerializeField] private Button _sellAllButton;
    private int _allPrice;
    private Queue<SaleItemData> _itemPool = new Queue<SaleItemData>();

    void Awake()
    {
        if (_storePanel == null)
            _storePanel = transform.GetChild(0).gameObject;
    }
    void OnEnable()
    { 
        StoreTriggerEvent.OnTriggerEnter += OpenSaleStore;
        StoreTriggerEvent.OnTriggerExit += CloseSaleStore;
    }
    void OnDisable()
    {
        StoreTriggerEvent.OnTriggerEnter -= OpenSaleStore;
        StoreTriggerEvent.OnTriggerExit -= CloseSaleStore; 
    }
    void OnDestroy()
    {
        StoreTriggerEvent.OnTriggerEnter -= OpenSaleStore;
        StoreTriggerEvent.OnTriggerExit -= CloseSaleStore;
    }

    void OpenSaleStore()
    {
        _storePanel.SetActive(true);

        Dictionary<string, InventorySlot> slots = new();
        _allPrice = 0;
        
        foreach (var slot in InventorySystem.Instance.GetSlots())
        {
            if (slot.Value.GetItem() is Brainrot)
            {
                slots.Add(slot.Key, slot.Value);
                _allPrice += slot.Value.GetItem().GetPrice();
            }
        }
        
        _allPriceText.text = _allPrice.ToString();
        
        foreach (Transform child in _itemParent.transform)
        {
            child.gameObject.SetActive(false);
            _itemPool.Enqueue(child.GetComponent<SaleItemData>());
        }
        
        _sellAllButton.onClick.RemoveAllListeners();
        _sellAllButton.onClick.AddListener(SellAllItems);
        
        foreach (var kvp in slots)
        {
            string slotKey = kvp.Key;
            InventorySlot slot = kvp.Value;
            
            SaleItemData itemSlot;
            if (_itemPool.Count > 0)
            {
                itemSlot = _itemPool.Dequeue();
                itemSlot.gameObject.SetActive(true);
            }
            else
            {
                itemSlot = Instantiate(_itemPrefab, _itemParent.transform);
            }
            itemSlot.transform.gameObject.SetActive(true);
            itemSlot.IconImage.sprite = slot.ItemIcon.sprite;

            if (slot.BackgroundIcon.gameObject.activeSelf)
            {
                itemSlot.BackgroundImage.enabled = true;
                itemSlot.BackgroundImage.sprite = slot.BackgroundIcon.sprite;
                itemSlot.BackgroundImage.color = slot.BackgroundIcon.color;
            }
            else
            {
                itemSlot.BackgroundImage.enabled = false;
            }



            itemSlot.ItemSizeText.text = $"({slot.ItemSize} ft.)";
            itemSlot.NameText.text = slot.GetItem().GetName();
            itemSlot.PriceText.text = slot.GetItem().GetPrice().ToString();
            itemSlot.Slot = slot;
            itemSlot.SlotKey = slotKey;
            
            if (itemSlot.SellButton != null)
            {
                itemSlot.SellButton.onClick.RemoveAllListeners();
                itemSlot.SellButton.onClick.AddListener(() => SellItem(itemSlot));
            }
        }
    }
    void CloseSaleStore()
    {
        _storePanel.SetActive(false);
    }

    void SellItem(SaleItemData itemData)
    {
        if (itemData.Slot == null || string.IsNullOrEmpty(itemData.SlotKey))
            return;
            
        int itemPrice = itemData.Slot.GetItem().GetPrice();
        InventorySystem.Instance.RemoveSlot(itemData.SlotKey);
        
        Debug.Log($"Реализация продажи предмета, выдано: {itemPrice}");
        //CurrencySystem.Instance.AddMoney(itemPrice);
        
        itemData.gameObject.SetActive(false);
        _itemPool.Enqueue(itemData);
        _allPriceText.text = _allPrice.ToString();
    }

    void SellAllItems()
    {
        List<SaleItemData> activeItems = new List<SaleItemData>();
        foreach (Transform child in _itemParent.transform)
        {
            if (child.gameObject.activeSelf)
            {
                SaleItemData item = child.GetComponent<SaleItemData>();
                if (item != null) activeItems.Add(item);
            }
        }

        foreach (SaleItemData item in activeItems)
        {
            SellItem(item);
        }
        Debug.Log($"Реализация продажи всех предметов, выдано: {_allPrice}");
        _allPrice = 0;
        _allPriceText.text = _allPrice.ToString();
    }

    [Header("Technical")]
    [SerializeField] private GameObject SellPrefab;
}

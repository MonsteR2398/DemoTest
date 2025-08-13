using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissingItemShop : MonoBehaviour
{
    [SerializeField] private MissingItemData _missingItemPrefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private MissingItemViewer _viewer;
    private Queue<MissingItemData> _itemPool = new Queue<MissingItemData>();
    private List<MissingItemData> _activeItems = new List<MissingItemData>();

    void Start()
    {
        StartCoroutine(UpdateEverySecondCoroutine());
    }

    public void AddItemInShop(Item item)
    {
        MissingItemData newItem;
        if (_itemPool.Count > 0)
            newItem = _itemPool.Dequeue();
        else
            newItem = Instantiate(_missingItemPrefab, _parent);

        newItem.Setup(item);
        newItem.RemainingTime = newItem.Timer;
        newItem.TimerText.text = newItem.RemainingTime.ToString() + "s";
        newItem.Button.onClick.RemoveAllListeners();
        newItem.Button.onClick.AddListener(() => BuyItem(item));
        newItem.gameObject.SetActive(true);
        
        _activeItems.Add(newItem);
        
        if (_viewer != null)
            newItem.Viewer = _viewer.AddViewerToItem(item);
    }
    // public List<Item> GetActiveItems()
    // {
    //     List<Item> items = new List<Item>();
    //     foreach (var itemData in _activeItems)
    //     {
    //         items.Add(itemData.Item);
    //     }
    //     return items;
    // }

    private IEnumerator UpdateEverySecondCoroutine()
    {
        while (true)
        {
            for (int i = _activeItems.Count - 1; i >= 0; i--)
            {
                MissingItemData item = _activeItems[i];
                item.RemainingTime--;
                item.TimerText.text = item.RemainingTime.ToString() + "s";

                if (item.RemainingTime <= 0)
                {
                    if (_viewer != null)
                        _viewer.RemoveItemFromViewer(item.Viewer);
                    
                    item.gameObject.SetActive(false);
                    _itemPool.Enqueue(item);
                    _activeItems.RemoveAt(i);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void BuyItem(Item item)
    {
        Debug.Log($"Покупка пропущенного яйца: {item.GetName()}");
        InventorySystem.Instance.AddItem(item, 1);
        
        for (int i = _activeItems.Count - 1; i >= 0; i--)
        {
            if (_activeItems[i].IconImage.sprite == item.GetIcon())
            {
                var itemData = _activeItems[i];
                itemData.gameObject.SetActive(false);
                _itemPool.Enqueue(itemData);
                _activeItems.RemoveAt(i);
                
                if (_viewer != null)
                    _viewer.RemoveItemFromViewer(itemData.Viewer);
                break;
            }
        }
    }
}

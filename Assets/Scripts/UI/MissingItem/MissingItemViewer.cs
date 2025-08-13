using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissingItemViewer : MonoBehaviour
{
    [SerializeField] private ItemViewerData _prefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private int _maxCount;
    
    [SerializeField] private MissingItemShop _shop;
    private List<ItemViewerData> _activeIcons = new List<ItemViewerData>();
    private Queue<ItemViewerData> _pool = new Queue<ItemViewerData>();
    
    void Start()
    {
        if (_shop == null)
        {
            Debug.LogError("MissingItemShop not selected in inspector");
            return;
        }
    }

    public ItemViewerData AddViewerToItem(Item item)
    {
        ItemViewerData icon = GetPooledIcon();
        icon.Image.sprite = item.GetIcon();
        icon.gameObject.SetActive(true);

        _activeIcons.Add(icon);
        return icon;
    }
    
    public void RemoveItemFromViewer(ItemViewerData item)
    {
        item.gameObject.SetActive(false);
        _pool.Enqueue(item);
        _activeIcons.Remove(item);
    }
    
    private ItemViewerData GetPooledIcon()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();
        return Instantiate(_prefab, _parent);
    }
}

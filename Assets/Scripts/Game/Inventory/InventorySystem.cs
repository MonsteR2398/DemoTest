using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;
    
    [Header("Settings")]
    public Transform spawnPoint;
    
    [Header("References")]
    public Transform inventoryPanel;
    public GameObject slotPrefab;

    public Item itemInHand { get; private set; }
    private Dictionary<string, InventorySlot> _slots = new Dictionary<string, InventorySlot>();

    public void SetItemInHand(Item item) => itemInHand = item;

    public void ReturnItemToInventory(Item item)
    {
        if (item != null) //&& item != itemInHand)
        {
            AddItem(item);
            itemInHand = null;
            Destroy(item.gameObject);
        }
    }
    
    private void Awake()
    {
        Instance = this;
    }

    public Dictionary<string, InventorySlot> GetSlots() => _slots;
    
    public void AddItem(Item item, int count = 1, bool needSave = true)
    {
        Item itemToAdd = item;
        
        //излишнее копирование, нужно добавить метод для переданного item с нужными параметрами чтобы избежать удаления и использовать его
        if (item is Egg originalEgg)
        {
            Egg newEgg = Instantiate(originalEgg, spawnPoint.position, Quaternion.identity);
            newEgg.gameObject.SetActive(false);

            newEgg.Data.EggType = originalEgg.Data.EggType;
            newEgg.Data.HasSpawned = false;
            itemToAdd = newEgg;

            if (needSave)
            {
                LoadInitializer.Instance.SaveLoadManager.AddEggData(newEgg.Data);
                LoadInitializer.Instance.SaveLoadManager.SaveEggsData();
            }
        }
        if (item is Brainrot rot)
        {
            if (needSave)
            {
                rot.Data.HasSpawned = false;
                LoadInitializer.Instance.SaveLoadManager.AddBrainrotData(rot.Data);
                LoadInitializer.Instance.SaveLoadManager.SaveBrainrotsData();
            }
        }
        
        if (_slots.TryGetValue(itemToAdd.GetItemID(), out InventorySlot slot))
        {
            slot.AddCount(count);
        }
        else
        {
            GameObject slotObj = Instantiate(slotPrefab, inventoryPanel);
            InventorySlot newSlot = slotObj.GetComponent<InventorySlot>();
            newSlot.Initialize(itemToAdd, count);
            _slots.Add(itemToAdd.GetItemID(), newSlot);
        }
    }
    
    public void RemoveSlot(string itemID)
    {
        if (_slots.TryGetValue(itemID, out InventorySlot slot))
        {
            Destroy(slot.gameObject);
            _slots.Remove(itemID);
        }
    }
}

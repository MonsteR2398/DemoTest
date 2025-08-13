using System.Transactions;
using UnityEngine;

public class UIEquipZone : BaseInteractionZone
{
    private Item _item;
    protected override void Start()
    {
        _display = ItemCanvasSystem.GetEquipDisplay();
        _item = this.GetComponent<Item>();
        base.Start();
    }
    public override void OnActionButtonClick()
    {
        _display.GetButton.onClick.RemoveListener(OnActionButtonClick);

        DeactivateZone();
        if (_item != null)
        {
            if (_item is Brainrot brainrot)
            {
                Debug.Log(brainrot.Data.UniqueId);
                if (!string.IsNullOrEmpty(brainrot.Data.UniqueId))
                {
                    LoadInitializer.Instance.SaveLoadManager.RemoveBrainrotByUniqueId(brainrot.Data.UniqueId);
                    LoadInitializer.Instance.SaveLoadManager.SaveBrainrotsData();
                }
            }
            InventorySystem.Instance.AddItem(_item, 1);
            gameObject.SetActive(false);
        }
    }

    protected override void ActivateZone(Transform playerTransform)
    {
        base.ActivateZone(playerTransform);
        if (_item.NowEnemy)
            _display.SetText("Украсть");
        else
            _display.SetText("Подобрать");
    }
}

using System.Transactions;
using UnityEngine;

public class UIEquipZone : BaseInteractionZone
{
    private Item _item;
    protected override void Start()
    {
        _display = ItemCanvasSystem.Instance.GetEquipDisplay();
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
                if (!string.IsNullOrEmpty(brainrot.Data.UniqueId))
                    LoadInitializer.Instance.SaveLoadManager.RemoveBrainrotByUniqueId(brainrot.Data.UniqueId);
                SoundManager.Instance.LastActiveZone.UnregisterDynamicAudio(brainrot.gameObject.GetComponent<SoundEmitter>());
            }
                
            InventorySystem.Instance.AddItem(_item, 1);
            gameObject.SetActive(false);
        }
    }

    // protected override void ActivateZone(Transform playerTransform)
    // {
    //     Debug.Log(1);
    //     base.ActivateZone(playerTransform);
    //     Debug.Log(2);
    //     if (_item.NowEnemy)
    //         _display.SetText("Украсть");
    //     else
    //         _display.SetText("Подобрать");
    // }

    protected override void ActivateZone(Transform playerTransform)
    {
        base.ActivateZone(playerTransform);

            _display.SetTarget(gameObject.transform);
            
            if (_item.NowEnemy)
            _display.SetText("Украсть");
        else
            _display.SetText("Подобрать");
        // else
        // {
        //     _display.SetActive(false);
        // }
    }
}

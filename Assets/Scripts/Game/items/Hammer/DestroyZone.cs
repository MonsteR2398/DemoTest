using UnityEngine;
using YG;

public class DestroyZone : BaseInteractionZone
{
    [SerializeField] protected int _price;
    public int UniqueId;

    public int SetUniqueId(int value) => UniqueId = value;

    protected override void Start()
    {
        base.Start();
        LoadInitializer.Instance.InitDestroyZones(this);
    }


    protected override void ActivateZone(Transform playerTransform)
    {
        base.ActivateZone(playerTransform);
        _display.SetTarget(gameObject.transform);
        _display.SetText(_price.ToString());
    }
    public override void OnActionButtonClick()
    {
        _display.GetButton.onClick.RemoveListener(OnActionButtonClick);
        LoadInitializer.Instance.SaveLoadManager.AddDestroyZone(this);
        DeactivateZone();

        Destroy(gameObject);
    }
}

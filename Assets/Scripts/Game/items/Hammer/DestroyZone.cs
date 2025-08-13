using UnityEngine;

public class DestroyZone : BaseInteractionZone
{
    [SerializeField] protected int _price;

    protected override void ActivateZone(Transform playerTransform)
    {
        base.ActivateZone(playerTransform);
        _display.SetText(_price.ToString());
    }
    public override void OnActionButtonClick()
    {
        _display.GetButton.onClick.RemoveListener(OnActionButtonClick);

        DeactivateZone();

        Destroy(gameObject);
    }
}

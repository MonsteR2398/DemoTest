using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InfoDisplay : UIDisplay
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Image _icon;
    
    private IInteractable _target;
    
    // public override void Init(object target)
    // {
    //     _target = (IInteractable)target;
    //     UpdateDisplay();
    // }
    
    // public override void UpdateDisplay()
    // {
    //     var data = _target.GetDisplayData();
    //     _text.text = data.Text;
    //     // Дополнительная логика отображения
    // }
}
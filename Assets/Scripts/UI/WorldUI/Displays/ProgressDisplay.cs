using UnityEngine;
using UnityEngine.UI;

public class ProgressDisplay : UIDisplay
{
    [SerializeField] private Slider _progressBar;
    
    private IProgressable _target;
    
    // public override void Init(object target)
    // {
    //     _target = (IProgressable)target;
    //     _progressBar.maxValue = _target.MaxProgress;
    //     UpdateDisplay();
    // }
    
    // public override void UpdateDisplay()
    // {
    //     _progressBar.value = _target.Progress;
    // }
}
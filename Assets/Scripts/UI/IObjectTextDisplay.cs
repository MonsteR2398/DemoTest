using System;
using UnityEngine;

public interface IObjectTextDisplay
{
    public string GetTextOnDisplay();
    //public bool HasUpdatePerFrame() => false;
    public Vector3 GetPosition();
    public Vector3 DisplayOffset { get; }
    public bool IsAlwaysVisible() => false;
    public bool ShouldOrientToCamera() => true;
    
    //public event Action OnDisplayTextChanged;
}

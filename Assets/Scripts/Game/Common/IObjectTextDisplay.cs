using System;
using UnityEngine;

public interface IObjectTextDisplay
{
    public string GetTextOnDisplay();
    public bool HasUpdatePerFrame() => false;
}

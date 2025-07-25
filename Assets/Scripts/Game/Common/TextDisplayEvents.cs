using System;

public static class TextDisplayEvents 
{
    public static event Action<IObjectTextDisplay> OnDisplayEnabled;
    public static event Action<IObjectTextDisplay> OnDisplayDisabled;

    public static void RaiseDisplayEnabled(IObjectTextDisplay display) => OnDisplayEnabled?.Invoke(display);
    public static void RaiseDisplayDisabled(IObjectTextDisplay display) => OnDisplayDisabled?.Invoke(display);
}
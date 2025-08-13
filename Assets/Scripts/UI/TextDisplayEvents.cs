using System;

public static class TextDisplayEvents 
{
    public static event Action<IObjectTextDisplay> OnDisplayEnabled;
    public static event Action<IObjectTextDisplay> OnDisplayDisabled;

    public static event Action<bool> LegendaryEggCanBeSpawned;
    public static event Action<bool> MythicEggCanBeSpawned;


    public static void RaiseLegendaryEggCanBeSpawned(bool ready) => LegendaryEggCanBeSpawned.Invoke(ready);
    public static void RaiseMythicEggCanBeSpawned(bool ready) => MythicEggCanBeSpawned.Invoke(ready);


    public static void RaiseDisplayEnabled(IObjectTextDisplay display) => OnDisplayEnabled?.Invoke(display);
    public static void RaiseDisplayDisabled(IObjectTextDisplay display) => OnDisplayDisabled?.Invoke(display);
}
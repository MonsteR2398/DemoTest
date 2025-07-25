using System;
using System.Collections;
using UnityEngine;


public class Egg : Item
{
    public int Id;

    [SerializeField] private string _name;
    [NonSerialized] public Rarity Rarity;
    [NonSerialized] public Variant Variant;


    private void OnEnable() => TextDisplayEvents.RaiseDisplayEnabled(this);
    private void OnDisable() => TextDisplayEvents.RaiseDisplayDisabled(this);

    public override string GetTextOnDisplay()
    {
        var variant = $"<style=Variant{Variant}>{Variant}</style>\n";
        if (Variant == Variant.Default) variant = "";
        var rarity = $"<style=Rarity{Rarity}>{Rarity}</style>\n";
        var name = $"{_name}\n";
        var price = $"<style=Price>${Price}</style>\n";

        string displayText =
        variant +
        rarity +
        name +
        price;

        return displayText;
    } 

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string name;
    public string description;
    public bool isKeyItem;

    public Sprite icon;
    public Item(string iconName)
    {
        icon = InitializeIcon(iconName);
    }

    public virtual bool Use() { return false; }
    public virtual Item Combine(Item item) { return null; }
    public virtual void Examine() { }
    public Sprite InitializeIcon(string itemName)
    {
        Sprite icon = Resources.Load<Sprite>("Sprites/Icons/" + itemName);
        if (icon == null)
        {
            Debug.LogWarning($"Icon for {itemName} not found!");
        }
        return icon;
    }
}
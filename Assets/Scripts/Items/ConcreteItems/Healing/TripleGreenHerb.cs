using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleGreenHerb : Item
{
    protected List<Item> itemList;
    private PlayerDamage playerDamage;

    public TripleGreenHerb(PlayerInventory playerInventory) : base("TripleGreenHerb")
    {
        this.playerInventory = playerInventory;
        playerDamage = playerInventory.playerDamage;
        name = "Triple Green Herb";
        description = "A mix of three medicinal herbs used to vastly restore one's health";
        isKeyItem = false;
    }

    public override bool Use()
    {
        playerDamage.Heal(100);
        Debug.Log("You have restored a vast amount of health");
        playerInventory.SetMessageText("You have restored a vast amount of health");
        return true;
    }
    public override Item Combine(Item item)
    {
        Debug.Log("Cannot Combine with " + item.name);
        return null;
    }
    public override bool CanCombine(Item item)
    {
        Debug.Log("Cannot Combine with " + item.name);
        return false;
    }
}

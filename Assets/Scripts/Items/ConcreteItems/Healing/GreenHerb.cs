using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenHerb : Item
{
    protected PlayerInventory playerInventory;

    private PlayerDamage playerDamage;

    public GreenHerb(PlayerInventory playerInventory) : base("GreenHerb")
    {
        this.playerInventory = playerInventory;
        playerDamage = playerInventory.playerDamage;

        name = "Green Herb";
        description = "A medicinal herb used to restore one's health";
        isKeyItem = false;

        if (icon == null)
        {
            Debug.LogWarning("The icon didnt set");
        }
    }

    public override bool Use()
    {
        playerDamage.Heal(25);
        Debug.Log("You have restored a small amount of health");
        return true;
    }
    public override Item Combine(Item item)
    {
        switch(item.name)
        {
            case "Red Herb":
                return new RedGreenHerb(playerInventory);
            case "Green Herb":
                return new DoubleGreenHerb(playerInventory);
            case "Double Green Herb":
                return new TripleGreenHerb(playerInventory);
            default:
                break;
        }
        return null;
    }
    public override bool CanCombine(Item item)
    {
        switch (item.name)
        {
            case "Red Herb":
                return true;
            case "Green Herb":
                return true;
            case "Double Green Herb":
                return true;
            default:
                Debug.Log("Cannot Combine with " + item.name);
                break;
        }
        return false;
    }
    public override void Examine() 
    {
        Debug.Log(description);
    }
}

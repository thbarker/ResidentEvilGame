using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenHerb : Item
{
    protected PlayerInventory playerInventory;
    protected List<Item> itemList;

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
                Debug.Log("Can Combine with Red Herb");
                return new RedGreenHerb(playerInventory);
            case "Green Herb":
                Debug.Log("Can Combine with Green Herb");
                return new DoubleGreenHerb(playerInventory);
            case "Double Green Herb":
                Debug.Log("Can Combine with Double Green Herb");
                return new TripleGreenHerb(playerInventory);
            default:
                Debug.Log("Cannot Combine with " + item.name);
                break;
        }
        return null;
    }
    public override void Examine() 
    {
        Debug.Log(description);
    }
}

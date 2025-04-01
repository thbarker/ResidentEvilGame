using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleGreenHerb : Item
{
    protected PlayerInventory playerInventory;
    protected List<Item> itemList;
    private PlayerDamage playerDamage;

    public DoubleGreenHerb(PlayerInventory playerInventory) : base("DoubleGreenHerb")
    {
        this.playerInventory = playerInventory;
        playerDamage = playerInventory.playerDamage;
        name = "Double Green Herb";
        description = "A Mix of two medicinal herb used to greatly restore one's health";
        isKeyItem = false;
    }

    public override bool Use()
    {
        playerDamage.Heal(50);
        Debug.Log("You have restored a great amount of health");
        return true;
    }
    public override Item Combine(Item item)
    {
        switch (item.name)
        {
            case "Green Herb":
                Debug.Log("Can Combine with Green Herb");
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

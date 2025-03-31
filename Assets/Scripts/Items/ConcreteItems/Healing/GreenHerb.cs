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

    public override void Use()
    {
        playerDamage.Heal(25);
        Debug.Log("You have gained health");
    }
    public override void Combine(Item item) 
    {
        switch(item.name)
        {
            case "Red Herb":
                Debug.Log("Can Combine with Red Herb");
                break;
            case "Green Herb":
                Debug.Log("Can Combine with Green Herb");
                break;
            case "Double Green Herb":
                Debug.Log("Can Combine with Double Green Herb");
                break;
            default:
                Debug.Log("Cannot Combine with " + item.name);
                break;
        }
    }
    public override void Examine() 
    {
        Debug.Log(description);
    }
}

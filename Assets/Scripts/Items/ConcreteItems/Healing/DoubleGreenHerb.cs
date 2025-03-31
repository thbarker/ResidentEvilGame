using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleGreenHerb : Item
{
    protected PlayerInventory playerInventory;
    protected List<Item> itemList;


    public DoubleGreenHerb(PlayerInventory playerInventory) : base("GreenHerb")
    {
        this.playerInventory = playerInventory;
        name = "Double Green Herb";
        description = "A Mix of two medicinal herb used to greatly restore one's health";
        isKeyItem = false;
    }

    public override void Use()
    {
        Debug.Log("You have gained health");
    }
    public override void Combine(Item item) 
    {
        switch (item.name)
        {
            case "Red Herb":
                Debug.Log("Can Combine with Red Herb");
                break;
            case "Green Herb":
                Debug.Log("Can Combine with Green Herb");
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

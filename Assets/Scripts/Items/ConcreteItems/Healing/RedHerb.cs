using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedHerb : Item
{
    protected PlayerInventory playerInventory;
    protected List<Item> itemList;


    public RedHerb(PlayerInventory playerInventory) : base("RedHerb")
    {
        this.playerInventory = playerInventory;
        name = "Red Herb";
        description = "A medicinal herb used to increase potency";
        isKeyItem = false;
    }

    public override bool Use()
    {
        Debug.Log("You cannot use this item alone");
        return false;
    }
    public override Item Combine(Item item)
    {
        switch (item.name)
        {
            case "Green Herb":
                return new RedGreenHerb(playerInventory);
            default:
                Debug.Log("Cannot Combine with " + item.name);
                break;
        }
        return null;
    }
    public override bool CanCombine(Item item)
    {
        switch (item.name)
        {
            case "Green Herb":
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

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

    public override void Use()
    {
        Debug.Log("You cannot use this item alone");
    }
    public override void Combine(Item item)
    {
        switch (item.name)
        {
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

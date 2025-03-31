using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedGreenHerb : Item
{
    protected PlayerInventory playerInventory;
    protected List<Item> itemList;


    public RedGreenHerb(PlayerInventory playerInventory) : base("GreenHerb")
    {
        this.playerInventory = playerInventory;
        name = "Red Green Herb";
        description = "A mix of medicinal herbs used to vastly restore health";
        isKeyItem = false;
    }

    public override void Use()
    {
        Debug.Log("You have healed");
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

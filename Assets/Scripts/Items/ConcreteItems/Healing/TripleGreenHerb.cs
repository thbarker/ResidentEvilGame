using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleGreenHerb : Item
{
    protected PlayerInventory playerInventory;
    protected List<Item> itemList;


    public TripleGreenHerb(PlayerInventory playerInventory) : base("GreenHerb")
    {
        this.playerInventory = playerInventory;
        name = "Triple Green Herb";
        description = "A mix of three medicinal herbs used to vastly restore one's health";
        isKeyItem = false;
    }

    public override void Use()
    {
        Debug.Log("You have gained health");
    }
    public override void Combine(Item item) 
    {
        Debug.Log("Cannot Combine with " + item.name);
    }
    public override void Examine() 
    {
        Debug.Log(description);
    }
}

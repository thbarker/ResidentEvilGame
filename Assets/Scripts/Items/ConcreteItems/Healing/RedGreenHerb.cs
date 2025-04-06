using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedGreenHerb : Item
{
    protected PlayerInventory playerInventory;
    protected List<Item> itemList;
    private PlayerDamage playerDamage;

    public RedGreenHerb(PlayerInventory playerInventory) : base("RedGreenHerb")
    {
        this.playerInventory = playerInventory; 
        playerDamage = playerInventory.playerDamage;

        name = "Red Green Herb";
        description = "A mix of medicinal herbs used to vastly restore health";
        isKeyItem = false;
    }

    public override bool Use()
    {
        playerDamage.Heal(100);
        Debug.Log("You have restored a vast amount of health");
        return true;
    }
    public override Item Combine(Item item)
    {
        switch (item.name)
        {
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

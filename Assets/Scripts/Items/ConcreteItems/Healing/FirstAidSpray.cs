using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAidSpray : Item
{
    protected PlayerInventory playerInventory;
    protected List<Item> itemList;

    private PlayerDamage playerDamage;

    public FirstAidSpray(PlayerInventory playerInventory) : base("FirstAidSpray")
    {
        this.playerInventory = playerInventory;
        playerDamage = playerInventory.playerDamage;

        name = "First Aid Spray";
        description = "An effective medical spray used to heal a vast amount of health";
        isKeyItem = false;

        if (icon == null)
        {
            Debug.LogWarning("The icon didnt set");
        }
    }

    public override bool Use()
    {
        playerDamage.Heal(100);
        Debug.Log("You have restored a vast amount of health");
        return true;
    }
    public override Item Combine(Item item)
    {
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

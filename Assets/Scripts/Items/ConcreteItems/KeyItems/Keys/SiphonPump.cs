using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SiphonPump : Item
{
    protected PlayerInventory playerInventory;

    public SiphonPump(PlayerInventory playerInventory) : base("SiphonPump")
    {
        this.playerInventory = playerInventory;

        name = "Siphon Pump";
        description = "A device that draws liquid from one container to another.";
        isKeyItem = true;

        if (icon == null)
        {
            Debug.LogWarning("The icon didnt set");
        }
    }

    public override bool Use()
    {
        Debug.Log("It is not necessary to use this right now.");
        return false;
    }
    public override Item Combine(Item item)
    {
        switch (item.name)
        {
            case "Empty Fuel Can":
                return new EmptyCanAndSiphon(playerInventory, 1);
        }
        return null;
    }
    public override bool CanCombine(Item item)
    {
        switch (item.name)
        {
            case "Empty Fuel Can":
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

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class SiphonPump : Item
{
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
        playerInventory.SetMessageText("It is not necessary to use this right now.");
        return false;
    }
    public override Item Combine(Item item)
    {
        switch (item.name)
        {
            case "Empty Fuel Can":
                return new EmptyCanAndSiphon(playerInventory, 1);
            default:
                playerInventory.SetMessageText("Cannot Combine with " + item.name);
                break;
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
}

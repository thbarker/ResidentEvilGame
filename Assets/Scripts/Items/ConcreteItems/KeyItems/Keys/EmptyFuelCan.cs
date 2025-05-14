using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EmptyFuelCan : Item
{
    public EmptyFuelCan(PlayerInventory playerInventory) : base("FuelCan")
    {
        this.playerInventory = playerInventory;

        name = "Empty Fuel Can";
        description = "A fuel can without any gasoline.";
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
            case "Siphon Pump":
                return new EmptyCanAndSiphon(playerInventory, 1);
        }
        return null;
    }
    public override bool CanCombine(Item item)
    {
        switch (item.name)
        {
            case "Siphon Pump":
                return true;
            default:
                Debug.Log("Cannot Combine with " + item.name);
                break;
        }
        return false;
    }
}

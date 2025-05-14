using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTruck : Lockable
{
    bool used = false;
    private void Start()
    {
        instantlyUse = true;
    }
    public override void Use()
    {
        if (!used)
        {
            playerInventory.AddItem(new FuelCan(playerInventory, 1));
            messageHandler.QueueMessage("You have filled up your fuel can.");
        }
    }
}

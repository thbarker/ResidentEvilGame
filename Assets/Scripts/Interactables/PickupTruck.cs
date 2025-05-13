using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTruck : Lockable
{
    private void Start()
    {
        instantlyUse = true;
    }
    public override void Use()
    {
        playerInventory.AddItem(new FuelCan(playerInventory, 1));
        messageHandler.QueueMessage("You have filled up your fuel can.");
    }
}

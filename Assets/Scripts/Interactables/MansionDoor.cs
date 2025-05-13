using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MansionDoor : Lockable
{
    public override void Use()
    {
        messageHandler.QueueMessage("You Win.");
    }
}

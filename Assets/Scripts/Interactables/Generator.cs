using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class Generator : Lockable
{
    public ElectronicallyLockedSafe safe;

    public override void Use()
    {
        safe.Unlock(); 
        messageHandler.QueueMessage("You have turned on the power.");
    }
}

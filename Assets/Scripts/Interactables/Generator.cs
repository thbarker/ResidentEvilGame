using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : Lockable
{
    public ElectronicallyLockedSafe safe;

    public override void Use()
    {
        safe.Unlock();
    }
}

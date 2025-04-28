using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElectronicallyLockedSafe : Lockable
{

    public void Unlock()
    {
        locked = false;
    }
}

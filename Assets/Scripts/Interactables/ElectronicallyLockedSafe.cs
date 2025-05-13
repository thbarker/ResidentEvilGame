using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElectronicallyLockedSafe : Lockable
{
    public GameObject mansionKey;
    public void Unlock()
    {
        locked = false;
        mansionKey.SetActive(true);
    }
}

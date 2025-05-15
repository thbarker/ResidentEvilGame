using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElectronicallyLockedSafe : Lockable
{
    public GameObject mansionKey;
    public GameObject safeDoor;
    public void Unlock()
    {
        locked = false;
        safeDoor.transform.rotation = Quaternion.Euler(0, -150, 0);
        mansionKey.SetActive(true);
    }
}

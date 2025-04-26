using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElectronicallyLockedSafe : Interactable
{
    private bool locked = true;
    private PlayerMovement playerMovement;
    private PlayerInventory playerInventory;
    private MessageHandler messageHandler; 
    private FocusCamera focusCamera;
    
    private void Awake()
    {
        playerMovement = GameObject.FindWithTag("Player")?.gameObject.GetComponent<PlayerMovement>();
        playerInventory = GameObject.FindWithTag("Player")?.transform.Find("Inventory")?.GetComponent<PlayerInventory>();
        messageHandler = GameObject.FindWithTag("Player")?.transform.Find("MessageHandler")?.GetComponent<MessageHandler>();
        focusCamera = GetComponent<FocusCamera>();
    }

    public override void Interact()
    {
        if (locked)
        {
            messageHandler.QueueMessage("It looks like it won't work without power.");
            focusCamera.Activate();
        } else
        {
            messageHandler.QueueMessage("It has a key to the mansion inside.");
        }
    }

    public void Unlock()
    {
        locked = false;
    }
}

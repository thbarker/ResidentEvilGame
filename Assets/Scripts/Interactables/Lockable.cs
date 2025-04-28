using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lockable : Interactable
{
    public bool locked = false;
    public string key;
    public string lockedMessage = "It is locked.";
    protected PlayerMovement playerMovement;
    protected PlayerInventory playerInventory;
    protected PlayerDamage playerDamage;
    protected MessageHandler messageHandler;

    protected override void Awake()
    {
        base.Awake();
        playerMovement = GameObject.FindWithTag("Player")?.gameObject.GetComponent<PlayerMovement>();
        playerInventory = GameObject.FindWithTag("Player")?.transform.Find("Inventory")?.GetComponent<PlayerInventory>();
        playerDamage = GameObject.FindWithTag("Player")?.gameObject.GetComponent<PlayerDamage>();
        messageHandler = GameObject.FindWithTag("Player")?.transform.Find("MessageHandler")?.GetComponent<MessageHandler>();
    }
    public virtual void Use() { }
    public override void Interact()
    {
        if (playerDamage.GetIsBeingBitten())
        {
            return;
        }
        if (locked)
        {
            Key keyItem = playerInventory.CheckForKey(key);
            if (keyItem != null)
            {
                keyItem.uses--;
                if (keyItem.uses <= 0)
                {
                    hasFocusCamera = false;
                    Debug.Log("This item is no longer needed, it is being discarded.");
                    messageHandler.QueueMessage(keyItem.name + " is no longer needed, it is being discarded.");
                    playerInventory.RemoveItem(keyItem);
                }
                locked = false;
            }
            else
            {
                Debug.Log(lockedMessage);
                messageHandler.QueueMessage(lockedMessage);
            }
        }
        else
        {
            Use();
        }
        base.Interact();
    }
}

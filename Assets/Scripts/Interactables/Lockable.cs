using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class Lockable : Interactable
{
    public bool locked = false;
    public bool instantlyUse = false;
    public string key;
    public string lockedMessage = "It is locked.";
    protected PlayerMovement playerMovement;
    protected PlayerInventory playerInventory;
    protected PlayerDamage playerDamage;
    protected MessageHandler messageHandler;
    protected UIManager uiManager;
    public AudioSource lockableAudioSource;
    public AudioClip unlockingClip;

    protected override void Awake()
    {
        base.Awake();
        playerMovement = GameObject.FindWithTag("Player")?.gameObject.GetComponent<PlayerMovement>();
        playerInventory = GameObject.FindWithTag("Player")?.transform.Find("Inventory")?.GetComponent<PlayerInventory>();
        playerDamage = GameObject.FindWithTag("Player")?.gameObject.GetComponent<PlayerDamage>();
        messageHandler = GameObject.FindWithTag("Player")?.transform.Find("MessageHandler")?.GetComponent<MessageHandler>();
        uiManager = GameObject.FindWithTag("Player")?.transform.Find("UIManager")?.GetComponent<UIManager>();
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
                    if(uiManager.uiActive)
                    {
                        if(keyItem.discardMessage != "")
                        {
                            playerInventory.SetMessageText(keyItem.discardMessage);
                        }
                        else if (keyItem.plural)
                        {
                            playerInventory.SetMessageText(keyItem.name + " are no longer needed, they are being discarded.");
                        }
                        else
                        {
                            playerInventory.SetMessageText(keyItem.name + " is no longer needed, it is being discarded.");
                        }
                    }
                    playerInventory.RemoveItem(keyItem);
                    if(lockableAudioSource != null){
                        lockableAudioSource.PlayOneShot(unlockingClip);
                    }
                    if(instantlyUse)
                    {
                        Use(); 
                    } else if (!uiManager.uiActive)
                    {
                        if(keyItem.plural)
                        {
                            messageHandler.QueueMessage(keyItem.name + " are no longer needed, they are being discarded.");
                        } else
                        {
                            messageHandler.QueueMessage(keyItem.name + " is no longer needed, it is being discarded.");
                        }
                    }
                }
                locked = false;
            }
            else if (!uiManager.uiActive)
            {
                base.Interact();
                Debug.Log(lockedMessage);
                messageHandler.QueueMessage(lockedMessage);
            } else if (uiManager.uiActive)
            {
                playerInventory.SetMessageText("It is not necessary to use this right now.");
            }
        }
        else
        {
            Use();
        }
    }
}

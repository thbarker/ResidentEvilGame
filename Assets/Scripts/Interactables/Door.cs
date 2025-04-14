using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public bool locked = false;
    public string key;
    [Tooltip("Room on the Blue Side of the door.")]
    public RoomManager aRoom;
    [Tooltip("Room on the Magenta Side of the door.")]
    public RoomManager bRoom;
    private Transform spawnA, spawnB;
    public BoxCollider aPlayableArea, bPlayableArea;
    private PlayerMovement playerMovement;
    private PlayerInventory playerInventory;
    private PlayerDamage playerDamage;

    private void Awake()
    {
        playerMovement = GameObject.FindWithTag("Player")?.gameObject.GetComponent<PlayerMovement>();
        playerInventory = GameObject.FindWithTag("Player")?.transform.Find("Inventory")?.GetComponent<PlayerInventory>();
        playerDamage = GameObject.FindWithTag("Player")?.gameObject.GetComponent<PlayerDamage>();
        spawnA = transform.Find("Spawn 1");
        spawnB = transform.Find("Spawn 2");
    }
    private void Start()
    {
        if (key == "")
            locked = false;
        else
            locked = true;
    }

    public override void Interact()
    {
        if (playerDamage.GetIsBeingBitten())
        {
            return;
        }
        if(locked)
        {
            Key keyItem = playerInventory.CheckForKey(key);
            if (keyItem != null)
            {
                keyItem.uses--;
                if (keyItem.uses <= 0)
                {
                    Debug.Log("This item is no longer needed, it is being discarded.");
                    playerInventory.RemoveItem(keyItem);
                }
                locked = false;
            }
            else
            {
                Debug.Log("This door is locked.");
                return;
            }
        }
        Transform spawn;
        if (Vector3.Distance(playerMovement.transform.position, spawnA.position)
          < Vector3.Distance(playerMovement.transform.position, spawnB.position))
        {
            spawn = spawnB;
            playerMovement.UseDoor(spawn, bPlayableArea, aRoom, bRoom);
        }
        else
        {
            spawn = spawnA;
            playerMovement.UseDoor(spawn, aPlayableArea, bRoom, aRoom);
        }
    }
}

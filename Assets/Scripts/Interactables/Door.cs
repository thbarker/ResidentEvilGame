using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public bool locked = false;
    public string key;
    public RoomManager aRoom, bRoom;
    private Transform spawnA, spawnB;
    public BoxCollider aPlayableArea, bPlayableArea;
    private PlayerMovement playerMovement;
    private PlayerDamage playerDamage;

    private void Awake()
    {
        playerMovement = GameObject.FindWithTag("Player")?.gameObject.GetComponent<PlayerMovement>();
        playerDamage = GameObject.FindWithTag("Player")?.gameObject.GetComponent<PlayerDamage>();
        spawnA = transform.Find("Spawn 1");
        spawnB = transform.Find("Spawn 2");
    }

    public override void Interact()
    {
        if (playerDamage.GetIsBeingBitten())
        {
            return;
        }
        Transform spawn;
        if (Vector3.Distance(playerMovement.transform.position, spawnA.position)
          < Vector3.Distance(playerMovement.transform.position, spawnB.position))
        {
            spawn = spawnB;
            playerMovement.UseDoor(spawn, bPlayableArea, aRoom, bRoom);
        } else
        {
            spawn = spawnA;
            playerMovement.UseDoor(spawn, aPlayableArea, bRoom, aRoom);
        }
    }
}

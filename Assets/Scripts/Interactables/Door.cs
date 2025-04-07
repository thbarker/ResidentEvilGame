using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public bool locked = false;
    public string key;
    private Transform spawnA, spawnB;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GameObject.FindWithTag("Player")?.gameObject.GetComponent<PlayerMovement>();
        spawnA = transform.Find("Spawn 1");
        spawnB = transform.Find("Spawn 2");
    }

    public override void Interact()
    {
        Transform spawn;
        if (Vector3.Distance(playerMovement.transform.position, spawnA.position)
          < Vector3.Distance(playerMovement.transform.position, spawnB.position))
        {
            spawn = spawnB;
        } else
        {
            spawn = spawnA;
        }
        playerMovement.UseDoor(spawn);
    }
}

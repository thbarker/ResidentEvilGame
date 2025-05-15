using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Lockable
{
    [Tooltip("Room on the Blue Side of the door.")]
    public RoomManager aRoom;
    [Tooltip("Room on the Magenta Side of the door.")]
    public RoomManager bRoom;
    private Transform spawnA, spawnB;
    public BoxCollider aPlayableArea, bPlayableArea;
    public AudioSource doorAudioSource;
    public AudioClip doorClip;

    protected override void Awake()
    {
        base.Awake();
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

    public override void Use()
    {
        Transform spawn;
        doorAudioSource.PlayOneShot(doorClip);
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

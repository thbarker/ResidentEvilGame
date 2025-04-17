using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<GameObject> zombieObjects;
    private List<ZombieController> zombieControllers;
    public ZombieList zombieList;
    public PlayerMovement playerMovement;
    [SerializeField]
    [Tooltip("Distance zombies are reset back from spawn point when player enters the room. This is to ensure zombies can't insta bite a player when they enter the room.")]
    [Range(0f, 10f)]
    private float safeDistance = 3f;

    private void Awake()
    {
        zombieList = GameObject.FindWithTag("Player")?.transform.Find("ZombieList")?.GetComponent<ZombieList>();
        playerMovement = GameObject.FindWithTag("Player")?.GetComponent<PlayerMovement>();
        zombieControllers = new List<ZombieController>();
        foreach (GameObject zombie in zombieObjects)
        {
            zombieControllers.Add(zombie.GetComponent<ZombieController>());
        }
    }
    public void Start()
    {
        ExitRoom();
    }
    public void EnterRoom(Transform playerSpawn, BoxCollider playableArea)
    {
        Debug.LogWarning("Setting " + gameObject.name + " zombies to active");
        foreach (ZombieController controller in zombieControllers) 
        {
            if (controller != null)
            {
                controller.Activate();

                // Calculate distance from the spawn point to the zombie
                float distance = Vector3.Distance(playerSpawn.position, controller.gameObject.transform.position);

                // Check if the zombie is within the safe distance
                if (distance < safeDistance)
                {
                    Vector3 direction = (controller.gameObject.transform.position - playerSpawn.position).normalized;
                    Vector3 newPosition = playerSpawn.position + direction * safeDistance;
                    newPosition = ClampPositionWithinBounds(newPosition, playableArea);
                    controller.gameObject.transform.position = newPosition;
                    Debug.Log("Moved zombie to " + newPosition + " to maintain safe distance.");
                }
            }
        }
        zombieList.ResetList(zombieObjects);
    }
    public void ExitRoom()
    {
        Debug.LogWarning("Setting " + gameObject.name + " zombies to unactive");
        foreach (GameObject zombie in zombieObjects)
        {
            zombie.GetComponent<ZombieController>()?.Deactivate();
        }
    }
    private Vector3 ClampPositionWithinBounds(Vector3 position, BoxCollider playableArea)
    {
        // Use the Box Collider's bounds to clamp the position
        position.x = Mathf.Clamp(position.x, playableArea.bounds.min.x, playableArea.bounds.max.x);
        position.y = Mathf.Clamp(position.y, playableArea.bounds.min.y, playableArea.bounds.max.y);
        position.z = Mathf.Clamp(position.z, playableArea.bounds.min.z, playableArea.bounds.max.z);
        return position;
    }
    /// <summary>
    /// This function sets every zombie's animator speed to 0. This is meant to avoid using TimeScale = 0 for pausing.
    /// </summary>
    public void PauseAllZombies()
    {
        if (playerMovement.currentRoom == this)
        {
            foreach (ZombieController controller in zombieControllers)
            {
                controller.Pause();
            }
        }
    }
    /// <summary>
    /// This function sets every zombie's animator speed to 1. This is meant to avoid using TimeScale = 1 for resuming.
    /// </summary>
    public void ResumeAllZombies()
    {
        if (playerMovement.currentRoom == this)
        {
            foreach (ZombieController controller in zombieControllers)
            {
                controller.Resume();
            }
        }
    }
}

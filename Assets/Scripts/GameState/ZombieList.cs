using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SortType
{
    AimNear,
    AimFar,
    DistanceNear,
    DistanceFar
}

public class ZombieList : MonoBehaviour
{
    private List<GameObject> allZombies;
    private List<GameObject> visibleZombies;
    [SerializeField]
    private SortType sortBy = SortType.AimNear;
    public GameObject player; // Player object to compare distances and directions

    void Awake()
    {
        allZombies = new List<GameObject>();
        visibleZombies = new List<GameObject>();
        player = GameObject.FindWithTag("Player");
        if (player == null) Debug.LogError("Scene Must have a player object tagged Player");
    }

    public void Sort()
    {
        // Sort list based on the current sorting criteria
        switch (sortBy)
        {
            case SortType.DistanceNear:
                SortByDistanceNear();
                break;
            case SortType.DistanceFar:
                SortByDistanceFar();
                break;
            case SortType.AimNear:
                SortByAimNear();
                break;
            case SortType.AimFar:
                SortByAimFar();
                break;
        }
    }
    private void Update()
    {
        UpdateVisibleZombies();
    }

    private void SortByDistanceNear()
    {
        visibleZombies.Sort((a, b) => (a.transform.position - player.transform.position).sqrMagnitude
            .CompareTo((b.transform.position - player.transform.position).sqrMagnitude));
    }

    private void SortByDistanceFar()
    {
        visibleZombies.Sort((a, b) => (b.transform.position - player.transform.position).sqrMagnitude
            .CompareTo((a.transform.position - player.transform.position).sqrMagnitude));
    }

    private void SortByAimNear()
    {
        Vector3 playerForward = player.transform.forward;
        visibleZombies.Sort((a, b) => {
            Vector3 dirA = (a.transform.position - player.transform.position).normalized;
            Vector3 dirB = (b.transform.position - player.transform.position).normalized;
            float angleA = Vector3.Angle(playerForward, dirA);
            float angleB = Vector3.Angle(playerForward, dirB);
            return angleA.CompareTo(angleB);
        });
    }

    private void SortByAimFar()
    {
        Vector3 playerForward = player.transform.forward;
        visibleZombies.Sort((a, b) => {
            Vector3 dirA = (a.transform.position - player.transform.position).normalized;
            Vector3 dirB = (b.transform.position - player.transform.position).normalized;
            float angleA = Vector3.Angle(playerForward, dirA);
            float angleB = Vector3.Angle(playerForward, dirB);
            return angleB.CompareTo(angleA);
        });
    }

    public void Add(GameObject zombie)
    {
        allZombies.Add(zombie); 
        Sort();
    }
    public void ResetList(List<GameObject> zombies)
    {

        allZombies.Clear();
        visibleZombies.Clear();
        foreach (GameObject zombie in zombies)
        {
            allZombies.Add(zombie);
        }
        UpdateVisibleZombies();
    }
    public void Remove(GameObject zombie)
    {
        allZombies.Remove(zombie);
        visibleZombies.Remove(zombie);
        Sort();
    }
    public SortType GetSortType()
    {
        return sortBy;
    }

    public void SetSortType(SortType sortBy)
    {
        this.sortBy = sortBy;
    }
    public GameObject GetZombieAt(int index)
    {
        // Return the zombie at the specified index
        return visibleZombies[index];
    }

    public int GetZombieCount()
    {
        // Return the number of zombies in the list
        return visibleZombies.Count;
    }

    void UpdateVisibleZombies()
    {
        visibleZombies.Clear();  // Clear the list of visible zombies each time the method is called

        foreach (GameObject zombie in allZombies)
        {
            if (zombie == null) continue;  // Skip any null entries

            Vector3 direction = zombie.transform.position - player.transform.position;
            float distance = direction.magnitude;
            direction.Normalize();
            Vector3 origin = new Vector3(player.transform.position.x, player.transform.position.y + 1, player.transform.position.z);
            // Perform the raycast
            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
            {
                // Check if the hit object is the zombie we're checking against
                if (hit.collider.gameObject == zombie)
                {
                    visibleZombies.Add(zombie);  // Add to visible list if hit the zombie directly
                }
            }
        }
    }

    public void DetectedByAll()
    {
        ZombieController zombieController;
        foreach (GameObject zombie in allZombies)
        {
            zombieController  = zombie.GetComponent<ZombieController>();
            if(!zombieController.GetDetectedPlayer())
            {
                zombieController.DetectPlayer(true);
            }
        }
    }
}

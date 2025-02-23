using Pathfinding;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class RotateTowardsPath : MonoBehaviour
{
    public float transitionSpeed = 0.1f; // Duration to transition the rotation in the coroutine
    public float walkingRotationSpeed = 1f; // Speed factor for continuous rotation during walking
    private GameObject player;

    private Quaternion targetRotation; // Desired rotation
    private bool isRotating = false; // Flag to control coroutine-based rotation
    private bool active = false;

    public AIPath aiPath;
    private Vector3 target; // Target to rotate towards
    public GameObject visual;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        UpdateTarget();
        if (aiPath != null && !isRotating) // Only rotate if not currently in the coroutine rotation
        {
            RotateContinuously();
        }
    }
    private void UpdateTarget() {
        RaycastHit hit;
        Vector3 direction = player.transform.position - transform.position; // Direction from this object to the player
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        // Perform the raycast
        if (Physics.Raycast(origin, direction.normalized, out hit, 100f))
        {
            // Check if the raycast hit the player
            if (hit.collider.tag == "Player")
            {
                target = player.transform.position;
            }
            else
            {
                Vector3? nextWaypoint = aiPath.GetNextWaypoint();
                if (nextWaypoint.HasValue)
                {
                    Vector3 position = nextWaypoint.Value;
                    target = position;
                    visual.transform.position = position;
                }
            }
        }

        
        else
        {
            // Handle cases where there is no next waypoint (perhaps the end of the path)
            // You might want to keep the visual at the last known position or hide it.
            //Debug.Log("No next waypoint available or end of path reached.");
        }
    }


    void RotateContinuously()
    {
        if (active)
        {
            Vector3 directionToTarget = target - transform.position;
            directionToTarget.y = 0; // Keep the rotation horizontal
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            // Smoothly rotate towards the target rotation at a different speed when walking
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, walkingRotationSpeed * Time.deltaTime);
        }
    }

    IEnumerator RotateToTarget()
    {
        isRotating = true;
        Vector3 directionToTarget = target - transform.position;
        directionToTarget.y = 0; // Keep the rotation horizontal
        targetRotation = Quaternion.LookRotation(directionToTarget);

        float timeElapsed = 0;
        Quaternion startRotation = transform.rotation;

        while (timeElapsed < transitionSpeed)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / transitionSpeed);
            timeElapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.rotation = targetRotation; // Ensure the rotation is exactly the target at the end
        isRotating = false; // Reset the flag

        Debug.DrawLine(transform.position, target, Color.red);
    }

    public void Activate(bool on)
    {
        if (on && !isRotating)
        {
            active = true;
            StartCoroutine(RotateToTarget());
        }
        else
        {
            active = false;
        }
    }
}

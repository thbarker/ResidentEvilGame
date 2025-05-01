using Pathfinding;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class RotateTowardsPath : MonoBehaviour
{
    [Tooltip("Duration to transition the rotation in the coroutine")]
    public float transitionSpeed = 0.1f;
    [Tooltip("Speed factor for continuous rotation during walking")]
    public float walkingRotationSpeed = 1f;
    [Tooltip("Maximum rotation speed when facing away from the player")]
    public float maxRotationSpeed = 2f;
    [Tooltip("Minimum rotation speed when facing toward from the player")]
    public float minRotationSpeed = 1f;

    private GameObject player;

    private Quaternion targetRotation; // Desired rotation
    private bool isRotating = false; // Flag to control coroutine-based rotation
    [SerializeField]
    private bool active = false;

    public AIPath aiPath;
    public AIDestinationSetter destinationSetter;
    private Vector3 target; // Target to rotate towards
    public GameObject targetVisual;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        destinationSetter = GetComponent<AIDestinationSetter>();
        destinationSetter.target = player.transform;
    }

    void Update()
    {
        UpdateTarget();
        if (aiPath != null && !isRotating) // Only rotate if not currently in the coroutine rotation
        {
            RotateContinuously();
        }
        if(targetVisual != null)
        {
            targetVisual.transform.position = target;
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
                }
            }
        }

    }


    void RotateContinuously()
    {
        if (active)
        {
            Vector3 directionToTarget = target - transform.position;
            directionToTarget.y = 0; // Keep the rotation horizontal
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            UpdateWalkingRotationSpeed(targetRotation); // Update rotation speed based on the current and target rotation

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
    void UpdateWalkingRotationSpeed(Quaternion targetRotation)
    {
        Vector3 direction = player.transform.position - transform.position;

        // Normalize the direction vector to ensure it's a unit vector
        Vector3 normalizedDir = direction.normalized;

        // Calculate the dot product with the forward vector
        float dotProduct = Vector3.Dot(normalizedDir, -transform.forward);

        float t = (dotProduct + 1) / 2;

        float curvedT = Mathf.Pow(t, 4f);

        // Scale rotation speed based on the angle to the target
        walkingRotationSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, curvedT);
       
    }

    public void Activate(bool on)
    {
        active = on;
    }
}

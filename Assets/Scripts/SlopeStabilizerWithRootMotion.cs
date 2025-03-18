using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SlopeStabilizerWithRootMotion : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorMove()
    {
        RaycastHit hit;

        // Perform a raycast from slightly above the character to detect the slope directly below
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 2f))
        {
            // Only adjust if we are on a slope
            if (hit.normal != Vector3.up) // Confirm that the ground is sloped
            {
                // Calculate the upward slope direction by projecting the Animator's root motion onto the plane defined by the hit normal
                Vector3 forwardOnSlope = Vector3.ProjectOnPlane(animator.deltaPosition, hit.normal).normalized;

                // Ensure this movement is upward on the slope
                if (Vector3.Dot(forwardOnSlope, Vector3.up) <= 0)
                {
                    forwardOnSlope = -forwardOnSlope; // Reverse it if pointing downward
                }

                // Adjust the magnitude of the movement to match the original root motion magnitude
                forwardOnSlope *= animator.deltaPosition.magnitude;

                // Calculate the new position by adding this corrected movement to the current position
                Vector3 newPosition = transform.position + forwardOnSlope;

                // Smoothly interpolate to the new position to prevent any jarring transitions
                transform.position = Vector3.Lerp(transform.position, newPosition, 1.0f); // Lerp factor can be adjusted for smoother transition
            }
            else
            {
                // On flat ground, apply root motion normally
                transform.position += animator.deltaPosition;
            }
        }
        else
        {
            // No ground detected, apply root motion normally
            transform.position += animator.deltaPosition;
        }
    }
}

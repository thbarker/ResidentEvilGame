
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SlopeStabilizer : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        RaycastHit hit;

        // Cast a ray downwards to detect the slope
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1f))
        {
            // Check if the normal is not straight up (i.e., there's a slope)
            if (hit.normal != Vector3.up)
            {// Calculate the slope direction that is orthogonal to the hit.normal and goes up the slope
                Vector3 right = Vector3.Cross(hit.normal, Vector3.up).normalized;
                Vector3 slopeDirection = Vector3.Cross(right, hit.normal).normalized;

                // Ensure the slope direction points upwards along the slope, not downwards
                if (Vector3.Dot(slopeDirection, Vector3.up) < 0)
                {
                    slopeDirection = -slopeDirection;
                }

                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                float forceMagnitude = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * Physics.gravity.magnitude * rb.mass;

                // Apply the force
                rb.AddForce(slopeDirection * forceMagnitude, ForceMode.Force);

                // Debugging: Draw force and normal vectors
                Debug.DrawRay(transform.position, slopeDirection * 2, Color.red); // Red for force direction
                Debug.DrawRay(transform.position, hit.normal * 2, Color.blue);    // Blue for normal
            }
        }
    }
}

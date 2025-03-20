using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CameraMovement : MonoBehaviour
{
    public float sensitivityX = 5f;
    public float sensitivityY = 5f;
    public float sensitivityCoefficient = 100.0f;
    public float aimYaw = 45f;

    public bool aiming = false;

    public GameObject cameraChild;  // Reference to the child camera
    public float defaultCameraDistance = 5f;  // Default distance if no obstacles are detected

    private float rotationX = 0f;  // To store vertical rotation angle
    private float rotationY = 0f;  // To store horizontal rotation angle

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Lock the cursor to the center of the screen
    }

    void Update()
    {
        UpdateRotation();
        PerformCameraRaycastAdjustment();
    }

    void UpdateRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * sensitivityCoefficient * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * sensitivityCoefficient * Time.deltaTime;

        // Update horizontal rotation (yaw)
        Quaternion yaw = Quaternion.AngleAxis(mouseX, Vector3.up);
        transform.rotation = yaw * transform.rotation;

        if (aiming)
        {
            // Lock the vertical rotation to 45 degrees when the right mouse button is held
            rotationX = aimYaw;
        }
        else
        {
            // Update vertical rotation (pitch) based on mouse movement if the right mouse button is not held
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        }

        // Apply the calculated pitch rotation
        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y, 0);
    }


    void PerformCameraRaycastAdjustment()
    {
        RaycastHit hit;
        float maxDistance = Mathf.Abs(defaultCameraDistance);
        int layerMask = 1 << LayerMask.NameToLayer("Entity");
        layerMask = ~layerMask;  // Invert the layer mask to ignore the Entity layer
        if (Physics.Raycast(transform.position, -transform.forward, out hit, maxDistance, layerMask))
        {
            // If raycast hits an obstacle, set the camera's local z-position to the negative value of the hit distance
            cameraChild.transform.localPosition = new Vector3(0, 0, -hit.distance);
        }
        else
        {
            // If no obstacle is detected, revert to the default distance
            cameraChild.transform.localPosition = new Vector3(0, 0, -defaultCameraDistance);
        }
    }
}

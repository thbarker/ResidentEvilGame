using UnityEngine;

public class CubeController : MonoBehaviour
{
    public Transform cameraTransform;
    public Animator animator;
    [SerializeField]
    private string[] moveStates;
    public float moveSpeed = 5.0f;
    public float mouseSensitivity = 100.0f;
    public float distanceFromTarget = 5.0f;
    public float verticalAngleLimit = 45.0f;  // Limit for vertical camera rotation

    private float currentRotationAroundTarget = 0.0f;
    private float currentVerticalAngle = 0.0f;
    private bool canMove = true;  // Default to true to allow movement initially

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        animator = GetComponent<Animator>();
        UpdateCameraPosition();  // Position the camera at the start
    }

    void Update()
    {
        UpdateCanMove();
        UpdateAnimations();
        if (canMove)
        {
            MoveCubeCameraRelative();
        }
        RotateCameraAroundCube();
    }

    void MoveCubeCameraRelative()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDirection = cameraTransform.right * x + cameraTransform.forward * z;
        moveDirection.y = 0; // Ensure the movement is strictly horizontal

        transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);

        if(Input.GetAxis("Horizontal") > 0 || Input.GetAxis("Vertical") > 0)
        {
            animator.SetBool("Walking", true);
        } else
        {
            animator.SetBool("Walking", false);
        }
    }

    void RotateCameraAroundCube()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        currentRotationAroundTarget += mouseX;
        currentVerticalAngle -= mouseY;
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, -verticalAngleLimit, verticalAngleLimit);

        UpdateCameraPosition();
    }

    void UpdateCameraPosition()
    {
        // Calculate the new position and rotation of the camera
        Quaternion horizontalRotation = Quaternion.Euler(0, currentRotationAroundTarget, 0);
        Quaternion verticalRotation = Quaternion.Euler(currentVerticalAngle, 0, 0);
        Vector3 direction = horizontalRotation * verticalRotation * -Vector3.forward;

        cameraTransform.position = transform.position + direction * distanceFromTarget;
        cameraTransform.LookAt(transform.position);
    }

    // Public method to allow external scripts to set the movement capability
    public void SetCanMove(bool moveAllowed)
    {
        Debug.Log("Setting Can Move to " + moveAllowed);
        canMove = moveAllowed;
    }

    void UpdateCanMove()
    {
        // Detect if the player can move based on animationState
        canMove = false;
        foreach (string s in moveStates)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(s))
            {
                canMove = true;
            }
        }
    }

    void UpdateAnimations()
    {
        
    }
}

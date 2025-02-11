using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public Animator animator;
    [SerializeField]
    private string[] moveStates;
    public float moveSpeed = 5.0f;
    public float mouseSensitivity = 100.0f;
    public float distanceFromTarget = 5.0f;
    public float verticalAngleLimit = 45.0f;  // Limit for vertical camera rotation

    private Rigidbody rb; // Reference to the Rigidbody component
    private bool canMove = true;  // Default to true to allow movement initially

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        UpdateCanMove();
        UpdateAnimations();
        if (canMove)
        {
            MoveAndRotatePlayerCameraRelative();
        }
    }

    void MoveAndRotatePlayerCameraRelative()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 moveDirection = cameraTransform.right * x + cameraTransform.forward * z;
        moveDirection.y = 0; // Ensure the movement is strictly horizontal

        // Move the Rigidbody to the new position
        rb.MovePosition(rb.position + moveDirection.normalized * moveSpeed * Time.fixedDeltaTime);

        // Rotate player to face the movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, newRotation, Time.fixedDeltaTime * 10)); // Smooth rotation
        }

        if (Mathf.Abs(x) > 0.1 || Mathf.Abs(z) > 0.1)
        {
            animator.SetBool("Walking", true);
        }
        else
        {
            animator.SetBool("Walking", false);
        }
    }



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
        // Additional animation updates can be handled here if needed
    }
}

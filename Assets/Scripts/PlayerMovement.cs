using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public Animator animator;
    [SerializeField]
    private string[] disableInputStates;
    public float moveSpeed = 80.0f;
    public float runSpeed = 200.0f;
    public float backwardSpeed = 60f;
    public float rotationSpeed = 1.0f;
    public float quickTurnSpeed = 0.25f;
    public float quickTurnDelay = 0.5f;

    private Rigidbody rb; // Reference to the Rigidbody component
    private bool inputEnabled = true;  // Default to true to allow player input
    private bool aiming = false;
    private bool isRunning = false;
    private bool isQuickTurning = false;
    private bool canQuickTurn = true;

    private float currentSpeed;
    private float x, z;

    public PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls(); // Ensure that controls is initialized

        // Assuming 'z' and 'x' are float since they're 1D axes
        controls.Player.Vertical.performed += ctx => z = ctx.ReadValue<float>();
        controls.Player.Horizontal.performed += ctx => x = ctx.ReadValue<float>();

        // Cancel events should reset the values to zero
        controls.Player.Vertical.canceled += ctx => z = 0;
        controls.Player.Horizontal.canceled += ctx => x = 0;

        // Sprint
        controls.Player.Run.performed += ctx => isRunning = true;
        controls.Player.Run.canceled += ctx => isRunning = false;

        // Aim
        controls.Player.Aim.performed += ctx =>
        {
            StartCoroutine(Aim());
            UpdateAiming(true);
        };
        controls.Player.Aim.canceled += ctx =>
        {
            UpdateAiming(false);
        };
    }
    private void OnEnable()
    {
        controls.Player.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
    }
    void Start()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        animator = GetComponent<Animator>();
        currentSpeed = moveSpeed;
    }


    void FixedUpdate()
    {
        UpdateInputEnabled();
        UpdateVerticalAim();
        UpdateRotationAnim();
        if (inputEnabled)
        {

            RotatePlayer();
            if (!aiming && !isQuickTurning) 
            {
                MovePlayer();

            }
        }
    }

    void MovePlayer()
    {

        Vector3 movement = new Vector3();

        // Calculate the new position
        if (z > 0)
            movement = transform.forward * z * currentSpeed * Time.deltaTime;
        else if (z < 0)
            movement = transform.forward * z * backwardSpeed * Time.deltaTime;

        // Apply velocity
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);


        // Forward
        if (z > 0.1)
        {
            animator.SetFloat("Direction", 1);
            animator.SetFloat("WalkCycleOffset", 0.425f);
        }
        // Backward
        if (z < 0.1)
        {
            animator.SetFloat("Direction", -(backwardSpeed / moveSpeed));
            animator.SetFloat("WalkCycleOffset", 0.375f);
        }
        // Walking
        if (Mathf.Abs(z) > 0.1 && !isRunning)
        {
            animator.SetBool("Walking", true);
            currentSpeed = moveSpeed;
        }
        // Stop Walking
        else
        {
            animator.SetBool("Walking", false);
        }
        // Running
        if (z > 0.1 && isRunning)
        {
            // If We transition from walk to run, start the run cycle at the corresponding offset
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
            {
                float animationProgress = (animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.275f) % 1;
                animator.SetFloat("RunCycleOffset", animationProgress);
            }
            // If We transition from idle to run, start the run cycle at the corresponding offset
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.SetFloat("RunCycleOffset", 0.275f);
            }
            currentSpeed = runSpeed;
            animator.SetBool("Running", true);
        }
        // Stop Running
        else
        {
            currentSpeed = moveSpeed;
            animator.SetBool("Running", false);
        }
        // Quick Turn
        if (z < - 0.1 && isRunning)
        {
            if(canQuickTurn)
                StartCoroutine(QuickTurn());
        }
        
    }

    void RotatePlayer()
    {
        Debug.Log("Rotating");
        // Calculate the rotation amount
        float rotationAmount = x * rotationSpeed * Time.deltaTime;

        // Current rotation
        Quaternion currentRotation = rb.rotation;

        // Calculate new rotation
        Quaternion newRotation = currentRotation * Quaternion.Euler(0, rotationAmount, 0);

        // Apply new rotation to the Rigidbody
        rb.MoveRotation(newRotation);
    }

    public void SetInputEnabled(bool on)
    {
        inputEnabled = on;
    }

    void UpdateInputEnabled()
    {
        // Detect if the player can move based on animationState
        inputEnabled = false;
        foreach (string s in disableInputStates)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(s))
            {
                inputEnabled = true;
            }
        }
    }

    private IEnumerator Aim()
    {
        animator.SetTrigger("Aim");
        yield return new WaitForSeconds(0.25f);
        animator.ResetTrigger("Aim");
    }
    void UpdateAiming(bool on)
    {
        rb.velocity = Vector3.zero;
        if (on)
        {
            currentSpeed = moveSpeed;
            aiming = true;
            animator.SetBool("Aiming", true);
        }
        else
        {
            aiming = false;
            animator.SetBool("Aiming", false);
        }
    }

    void UpdateVerticalAim()
    {
        if(aiming)
        {
            animator.SetFloat("VerticalAim", z);
        }
    }
    void UpdateRotationAnim()
    {
        animator.SetFloat("Rotation", x);
    }

    IEnumerator QuickTurn()
    {
        if (inputEnabled)
        {
            isQuickTurning = true;
            canQuickTurn = false;
            animator.SetBool("QuickTurn", true);
            Quaternion initialRotation = transform.rotation;  // Capture the initial rotation
            Quaternion targetRotation = initialRotation * Quaternion.Euler(0, 180, 0);  // Calculate the target rotation

            float elapsedTime = 0;  // Track the elapsed time
            while (elapsedTime < quickTurnSpeed)
            {
                // Update the elapsed time
                elapsedTime += Time.deltaTime;
                // Calculate the fraction of time completed
                float fraction = elapsedTime / quickTurnSpeed;
                // Interpolate rotation from initial to target over time
                transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, fraction);
                // Wait until the next frame to continue
                yield return null;
            }
            // Ensure the rotation is exactly at the target to avoid small errors
            transform.rotation = targetRotation;
            animator.SetBool("QuickTurn", false);
            isQuickTurning = false;

            yield return new WaitForSeconds(quickTurnDelay);
            canQuickTurn = true;
        }
    }


}

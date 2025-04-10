using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInputManager))]
public class PlayerMovement : MonoBehaviour
{
    #region State Machine
    public PlayerStateMachine StateMachine { get; set; }
    public PlayerIdleState IdleState { get; set; }
    public PlayerMoveState MoveState { get; set; }
    public PlayerQuickturnState QuickturnState { get; set; }
    public PlayerAimState AimState { get; set; }
    public PlayerBitState BitState { get; set; }
    public PlayerDieState DieState { get; set; }
    #endregion

    public Transform cameraTransform;
    public Animator animator;
    [SerializeField]
    private string[] disableInputStates;
    public float moveSpeed = 80.0f;
    public float runSpeed = 200.0f;
    public float backwardSpeed = 60f;
    public float rotationSpeed = 1.0f;
    [Range(0.1f, 10f)]
    [Tooltip("Speed of the rotation toward a newly selected target")]
    public float changeTargetSpeed = 5.0f;
    [Range(0.01f, 1f)]
    public float quickTurnDuration = 0.25f;
    [Range(0.1f, 1f)]
    public float quickTurnDelay = 0.5f;
    [Range(0f, 1f)]
    [Tooltip("Intensity of aiming up, where a value of 1 is aiming 90 degrees and a value of 0 is no vertical aiming")]
    public float upAimOffset = 0.5f;
    [Range(0f, 1f)]
    [Tooltip("Intensity of aiming down, where a value of 1 is aiming 90 degrees and a value of 0 is no vertical aiming")]
    public float downAimOffset = 0.5f;
    [Range(0.01f, 1f)]
    [Tooltip("Amount of time it takes to look up/down. Used for the vertical aim smoothing")]
    public float verticalAimTime = 0.1f;
    [Range(0.01f, 5f)]
    [Tooltip("Duration of the transition sequence when using a door.")]
    public float roomTransitionDuration = 2f;

    public Rigidbody rb; // Reference to the Rigidbody component
    public PlayerControls controls;
    public RoomManager currentRoom;
    private bool inputEnabled = true;  // Default to true to allow player input
    private bool aiming = false;
    private bool isRunning = false;
    private bool isQuickTurning = false;
    public bool isUsingDoor = false;
    private bool canQuickTurn = true;
    private bool aimAfterQuickTurn = false;
    private float verticalAimLerpValue = 0;

    private Image blackScreen;
    private float fadeStartTime;

    private float currentSpeed;
    private float x, z;
    void OnValidate()
    {
    }

    private void Awake()
    {
        // Get reference to player controls
        controls = PlayerInputManager.controls;

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
            if (!isQuickTurning)
                UpdateAiming(true);
            else
                aimAfterQuickTurn = true;
        };
        controls.Player.Aim.canceled += ctx =>
        {
            UpdateAiming(false);
        };

        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        animator = GetComponent<Animator>();
        blackScreen = transform.Find("FadeCanvas")?.Find("Image")?.GetComponent<Image>();
        currentSpeed = moveSpeed;

        StateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, StateMachine);
        MoveState = new PlayerMoveState(this, StateMachine);
        QuickturnState = new PlayerQuickturnState(this, StateMachine);
        AimState = new PlayerAimState(this, StateMachine);
        BitState = new PlayerBitState(this, StateMachine);
        DieState = new PlayerDieState(this, StateMachine);
    }
    void Start()
    {
        StateMachine.Initialize(MoveState);
    }

    private void Update()
    {
        StateMachine.CurrentPlayerState.FrameUpdate();
    }

    void FixedUpdate()
    {
        StateMachine.CurrentPlayerState.PhysicsUpdate();
        UpdateInputEnabled();
    }
    public void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentPlayerState.AnimationTriggerEvent(triggerType);
    }

    public enum AnimationTriggerType
    {
        TakeDamage,
        Shoot
    }
    public void MovePlayer()
    {

        Vector3 movement = new Vector3();

        // Calculate the new position
        if (z > 0)
            movement = transform.forward * z * currentSpeed * Time.deltaTime;
        else if (z < 0 && canQuickTurn)
            movement = transform.forward * z * backwardSpeed * Time.deltaTime;
        else
            movement = Vector3.zero;

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
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walking"))
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
    }

    public void RotatePlayer()
    {
        // Calculate the rotation amount
        float rotationAmount = x * rotationSpeed * Time.deltaTime;

        // Current rotation
        Quaternion currentRotation = rb.rotation;

        // Calculate new rotation
        Quaternion newRotation = currentRotation * Quaternion.Euler(0, rotationAmount, 0);

        // Apply new rotation to the Rigidbody
        rb.MoveRotation(newRotation);
        UpdateRotationAnim();
    }

    public void SetInputEnabled(bool on)
    {
        inputEnabled = on;
    }

    void UpdateInputEnabled()
    {
        // Detect if the player can move based on animationState
        inputEnabled = true;
        foreach (string s in disableInputStates)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(s))
            {
                inputEnabled = false;
            }
        }
    }

    public void UpdateAiming(bool on)
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

    public void UpdateVerticalAim()
    {
        if (aiming)
        {
            // Calculate lerp speed, ensuring no division by zero
            float lerpSpeed = verticalAimTime != 0 ? Time.deltaTime / verticalAimTime : Time.deltaTime;

            // Lerping the verticalAimLerpValue towards z over time
            verticalAimLerpValue = Mathf.Lerp(verticalAimLerpValue, z, lerpSpeed);

            // Determine the offset based on the sign of z to handle smooth transition
            float aimOffset = (z >= 0) ? upAimOffset : downAimOffset;

            // Use a blended approach for setting the vertical aim, smoothing out the transition around z = 0
            float verticalAimTemp = 1 - ((verticalAimLerpValue * (aimOffset)) + 1) / 2;
            animator.SetFloat("VerticalAim", verticalAimTemp);
        }
    }
    /// <summary>
    /// This function should be used by a door script to teleport the player
    /// safely to the other side of the door. It is expected to handle all of 
    /// the updates that come with entering a new room.
    /// </summary>
    /// <param name="spawnPoint">The spawnpoint that the player should be teleported to.</param>
    /// <param name="playableArea">A Box collider provided by the door that represents the playable space for zombies to be repositioned to in case they are violating the safe distance.</param>
    /// <param name="from">The room the player is exiting.</param>
    /// <param name="to">The room the player is entering.</param>
    public void UseDoor(Transform spawnPoint, BoxCollider playableArea, RoomManager from, RoomManager to)
    {
        if (StateMachine.CurrentPlayerState == MoveState)
        {
            StartCoroutine(RoomTransition(roomTransitionDuration, spawnPoint, playableArea, from, to));
        }
        
    }

    IEnumerator RoomTransition(float seconds, Transform spawnPoint, BoxCollider playableArea, RoomManager from, RoomManager to)
    {
        rb.velocity = Vector3.zero;
        fadeStartTime = Time.time;
        StateMachine.ChangeState(IdleState);
        isUsingDoor = true;
        while (Time.time - fadeStartTime < seconds/3)
        {
            float t = (Time.time - fadeStartTime) / (seconds / 3);
            float alpha = Mathf.Lerp(0, 1, t);
            blackScreen.color = new Color(0,0,0,alpha);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 1);
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        // Exit previous room
        from.ExitRoom();

        yield return new WaitForSeconds(seconds / 3);

        // Enter next room
        to.EnterRoom(spawnPoint, playableArea);
        currentRoom = to;

        StateMachine.ChangeState(MoveState);
        fadeStartTime = Time.time;
        while (Time.time - fadeStartTime < seconds / 3)
        {
            float t = (Time.time - fadeStartTime) / (seconds / 3);
            float alpha = Mathf.Lerp(1, 0, t);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 0);
        isUsingDoor = false;
    }
    public void UpdateRotationAnim()
    {
        animator.SetFloat("Rotation", x);
    }

    public bool GetAiming() { return aiming; }
    public bool GetIsRunning() { return isRunning; }
    public bool GetIsQuickTurning() { return isQuickTurning; }
    public float GetZ() { return z; }
}

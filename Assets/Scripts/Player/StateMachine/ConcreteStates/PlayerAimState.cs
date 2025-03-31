using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : PlayerState
{
    private float startTime;
    private Rigidbody rb;
    private Animator animator;
    private Transform transform;
    private PlayerControls controls;
    private PlayerShoot playerShoot;
    private ZombieList zombieList; 
    private int currentTargetIndex = -1; // Start with -1 to indicate no target initially
    private bool isRotating = false;
    private bool canAttack = false;
    public float rotationSpeed;  // Adjust this value to control the speed of rotation
    private Quaternion targetRotation;



    public PlayerAimState(PlayerMovement playerMovement, PlayerStateMachine playerStateMachine) : base(playerMovement, playerStateMachine)
    {
        rb = playerMovement.rb;
        animator = playerMovement.animator;
        transform = playerMovement.transform;
        rotationSpeed = playerMovement.changeTargetSpeed;

        // Get reference to player controls
        controls = PlayerInputManager.controls;
        
        playerShoot = playerMovement.GetComponent<PlayerShoot>();

        controls.Player.ChangeTarget.performed += ctx => ChangeTarget();
        zombieList = playerMovement.transform.Find("ZombieList").GetComponent<ZombieList>();
        if (!zombieList) Debug.LogError("Scene must have a zombie list gameobject tagged as ZombieList");
    }

    public override void AnimationTriggerEvent(PlayerMovement.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        if (zombieList && zombieList.GetZombieCount() > 0)
        {
            zombieList.Sort();
            currentTargetIndex = -1;
            ChangeTarget();
        }
        startTime = Time.time;
        rb.velocity = Vector3.zero;
        animator.SetTrigger("Aim");
    }

    public override void ExitState()
    {
        base.ExitState();
        canAttack = false;
        playerShoot.canAttack = false;
        isRotating = false;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        // Reset the animation trigger
        if(Time.time - startTime > 0.25)
        {
            animator.ResetTrigger("Aim");
        }
        if(Time.time - startTime > 0.25)
        {
            playerShoot.canAttack = true;
            canAttack = true;
        }
        
        animator.SetFloat("AttackSpeed", (1 / playerShoot.fireRate));

        playerShoot.UpdateAttack();

        // Exit if not aiming
        if (!playerMovement.GetAiming())
        {
            playerMovement.StateMachine.ChangeState(playerMovement.MoveState);
        }

        // Attack when necessary


        if (isRotating)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Check if the rotation is close enough to the target rotation
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation; // Snap to the target rotation
                isRotating = false;
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // Rotate Player
        playerMovement.RotatePlayer();
        // Update the Vertical Aiming
        playerMovement.UpdateVerticalAim();
        // Handle Animations
        playerMovement.UpdateRotationAnim();
    }
    private void ChangeTarget()
    {
        // Ensure this is the active state
        if (playerMovement.StateMachine.CurrentPlayerState != this)
        {
            return;
        }
        if (zombieList == null || zombieList.GetZombieCount() == 0)
        {
            Debug.Log("No zombies available to target.");
            return;
        }

        // Increment the target index and wrap around if necessary
        currentTargetIndex = (currentTargetIndex + 1) % zombieList.GetZombieCount();

        // Set the new target
        GameObject newTarget = zombieList.GetZombieAt(currentTargetIndex);

        // Here you can implement how the target is visually or mechanically handled,
        // e.g., pointing a weapon, highlighting, etc.
        Face(newTarget);
        Debug.Log(currentTargetIndex);
    }

    private void Face(GameObject target)
    {
        targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        isRotating = true;
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerQuickturnState : PlayerState
{
    float startTime;
    private Rigidbody rb;
    private Animator animator;
    private Transform transform;
    private float quickTurnDuration;
    private float quickTurnDelay;
    Quaternion initialRotation;
    Quaternion targetRotation;
    public PlayerQuickturnState(PlayerMovement playerMovement, PlayerStateMachine playerStateMachine) : base(playerMovement, playerStateMachine)
    {
        rb = playerMovement.rb;
        animator = playerMovement.animator;
        transform = playerMovement.transform;

        quickTurnDuration = playerMovement.quickTurnDuration;
        quickTurnDelay = playerMovement.quickTurnDelay;
    }

    public override void AnimationTriggerEvent(PlayerMovement.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState(); 
        startTime = Time.time;
        rb.velocity = Vector3.zero;
        animator.SetBool("QuickTurn", true);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 1);
        initialRotation = transform.rotation; // Capture the initial rotation
        targetRotation = initialRotation * Quaternion.Euler(0, 179, 0); // Calculate the target rotation
    }

    public override void ExitState()
    {
        base.ExitState(); 
        animator.SetBool("QuickTurn", false);
        animator.SetLayerWeight(1, 1);
        animator.SetLayerWeight(2, 0);
        // Apply the final 1 degree rotation to ensure complete 180 degree rotation
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + 1, transform.rotation.eulerAngles.z);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (Time.time - startTime < quickTurnDuration)
        {
            // Calculate the fraction of time completed
            float fraction = (Time.time - startTime) / quickTurnDuration;
            // Interpolate rotation from initial to target over time
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, fraction);
        }
        else if (Time.time - startTime < (quickTurnDuration + quickTurnDelay))
        {
            animator.SetBool("QuickTurn", false);
        }
        else
        {
            if (playerMovement.GetAiming())
            {
                playerMovement.StateMachine.ChangeState(playerMovement.AimState);
            }
            else
            {
                playerMovement.StateMachine.ChangeState(playerMovement.MoveState);
            }
        }
    }


    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

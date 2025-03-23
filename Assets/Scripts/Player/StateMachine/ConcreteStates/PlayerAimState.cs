using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimState : PlayerState
{
    float startTime;
    private Rigidbody rb;
    private Animator animator;
    private Transform transform;
    public PlayerAimState(PlayerMovement playerMovement, PlayerStateMachine playerStateMachine) : base(playerMovement, playerStateMachine)
    {
        rb = playerMovement.rb;
        animator = playerMovement.animator;
        transform = playerMovement.transform;
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
        animator.SetTrigger("Aim");
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if(Time.time - startTime > 0.25)
        {
            animator.ResetTrigger("Aim");
        }
        if (!playerMovement.GetAiming())
        {
            playerMovement.StateMachine.ChangeState(playerMovement.MoveState);
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        playerMovement.RotatePlayer();
        playerMovement.UpdateVerticalAim();
        playerMovement.UpdateRotationAnim();
    }
}

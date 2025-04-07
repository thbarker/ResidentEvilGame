using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    private Animator animator;
    public PlayerMoveState(PlayerMovement playerMovement, PlayerStateMachine playerStateMachine) : base(playerMovement, playerStateMachine)
    {
        animator = playerMovement.animator;
    }

    public override void AnimationTriggerEvent(PlayerMovement.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
        animator.SetBool("Walking", false);
        animator.SetBool("Running", false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if(playerMovement.GetAiming())
        {
            playerMovement.StateMachine.ChangeState(playerMovement.AimState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        playerMovement.RotatePlayer();
        playerMovement.MovePlayer();

        if (playerMovement.GetZ() < -0.1 && playerMovement.GetIsRunning())
        {
            playerMovement.StateMachine.ChangeState(playerMovement.QuickturnState);
        }
    }
}

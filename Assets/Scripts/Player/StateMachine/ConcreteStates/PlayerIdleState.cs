using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    private Animator animator;
    public PlayerIdleState(PlayerMovement playerMovement, PlayerStateMachine playerStateMachine) : base(playerMovement, playerStateMachine)
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
        animator.speed = 0;
    }

    public override void ExitState()
    {
        base.ExitState();
        animator.speed = 1f;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

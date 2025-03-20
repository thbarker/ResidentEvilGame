using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleState : EnemyState
{
    private float detectionDistance;
    private Animator animator;
    public ZombieIdleState(ZombieController zombieController, EnemyStateMachine enemyStateMachine) : base(zombieController, enemyStateMachine)
    {
        detectionDistance = zombieController.GetDetectionDistance();
        animator = zombieController.GetComponent<Animator>();
    }

    public override void AnimationTriggerEvent(ZombieController.AnimationTriggerType triggerType)
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
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate(); 

        // If the zombie is within the detection radius, switch to the target state
        if (zombieController.GetDistanceToPlayer() < detectionDistance)
        {
            animator.SetBool("Detect", true);
            zombieController.StateMachine.ChangeState(zombieController.TargetState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

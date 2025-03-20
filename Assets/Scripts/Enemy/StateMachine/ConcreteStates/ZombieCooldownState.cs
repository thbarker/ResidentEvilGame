using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCooldownState : EnemyState
{
    private Animator animator;
    private float reachCooldown;
    float startTime;
    public ZombieCooldownState(ZombieController zombieController, EnemyStateMachine enemyStateMachine) : base(zombieController, enemyStateMachine)
    {
        animator = zombieController.animator;
        reachCooldown = zombieController.GetReachCooldown();
    }

    public override void AnimationTriggerEvent(ZombieController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState(); 
        animator.ResetTrigger("Reach");
        Debug.Log("I'm cooling down");
        startTime = Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();
        Debug.Log("I'm done with cooldown");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        // After the cooldown time is finished, switch to idle state
        if (Time.time - startTime > reachCooldown)
        {
            zombieController.StateMachine.ChangeState(zombieController.TargetState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieKnockbackState : EnemyState
{
    private GameObject player;
    private PlayerDamage playerDamage;
    private Animator animator;
    private ReachCollision reachCollision;
    private Transform zombieTransform;
    float startTime;
    Vector3 direction;
    Quaternion lookRotation;
    public ZombieKnockbackState(ZombieController zombieController, EnemyStateMachine enemyStateMachine) : base(zombieController, enemyStateMachine)
    {
        player = zombieController.player;
        playerDamage = zombieController.playerDamage;
        animator = zombieController.animator;
        reachCollision = zombieController.reachCollisionScript;
        zombieTransform = zombieController.transform;
    }

    public override void AnimationTriggerEvent(ZombieController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        animator.SetTrigger("PushBack");
        direction = (player.transform.position - zombieTransform.position).normalized;
        lookRotation = Quaternion.LookRotation(direction);
        startTime = Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();
        animator.applyRootMotion = true;
        animator.ResetTrigger("PushBack");
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (Time.time - startTime > 0.875)
        {
            zombieController.StateMachine.ChangeState(zombieController.IdleState);
        }
        RotateTowardsPlayer(4f);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    private void RotateTowardsPlayer(float rotationSpeed)
    {
        float t = Time.deltaTime * rotationSpeed;
        zombieTransform.rotation = Quaternion.Slerp(zombieTransform.rotation, lookRotation, t);
    }
}

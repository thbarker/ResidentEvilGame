using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDieState : EnemyState
{
    private GameObject player;
    private PlayerDamage playerDamage;
    private Animator animator;
    private ReachCollision reachCollision;
    private Transform zombieTransform;
    private CapsuleCollider capsuleCollider;
    float startTime;
    public bool dead;
    Vector3 direction;
    Quaternion lookRotation;
    public ZombieDieState(ZombieController zombieController, EnemyStateMachine enemyStateMachine) : base(zombieController, enemyStateMachine)
    {
        player = zombieController.player;
        playerDamage = zombieController.playerDamage;
        animator = zombieController.animator;
        reachCollision = zombieController.reachCollisionScript;
        zombieTransform = zombieController.transform;
        capsuleCollider = zombieController.capsuleCollider;
    }

    public override void AnimationTriggerEvent(ZombieController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        animator.SetTrigger("Death");
        startTime = Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (dead)
        {
            animator.SetBool("Dead", true);
        }
        dead = true;
        if(Time.time - startTime > 1f)
            capsuleCollider.enabled = false;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ZombieBiteState : EnemyState
{
    private Animator animator;
    private PlayerDamage playerDamage;
    private Transform playerTransform;
    private AIPath aiPath;
    private CapsuleCollider capsuleCollider;

    private float colliderRadius;
    private float biteDuration;
    private float biteRotationSpeed;
    private Vector3 startPosition;
    private Vector3 directionToZombie;
    private Vector3 targetPosition;
    private float timeElapsed;
    private float lerpDuration = 1f;
    float startTime;

    public ZombieBiteState(ZombieController zombieController, EnemyStateMachine enemyStateMachine) : base(zombieController, enemyStateMachine)
    {
        animator = zombieController.animator;
        playerDamage = zombieController.playerDamage;
        playerTransform = zombieController.player.transform;
        aiPath = zombieController.aiPath;
        capsuleCollider = zombieController.capsuleCollider;
        colliderRadius = zombieController.colliderRadius;
        biteDuration = zombieController.GetBiteDuration();
        biteRotationSpeed = zombieController.GetBiteRotationSpeed();
    }

    public override void AnimationTriggerEvent(ZombieController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState(); 
        animator.applyRootMotion = false;
        aiPath.enabled = false;
        capsuleCollider.radius = 0.01f;
        playerDamage.GetBit(zombieController.gameObject, zombieController.biteTransform, zombieController.transform); 
        animator.SetTrigger("Bite"); // Trigger the Bite animation
        startTime = Time.time;
        timeElapsed = 0;

        // Set up the lerp variables
        zombieController.rb.velocity = Vector3.zero;

        // Start position of the object
        startPosition = zombieController.transform.position;

        // Calculate the direction from the player to the zombie
        directionToZombie = (zombieController.transform.position - zombieController.player.transform.position).normalized;

        // Set the target position to be a certain distance from the player, but towards the zombie
        targetPosition = zombieController.player.transform.position + directionToZombie * 0.25f;

    }

    public override void ExitState()
    {
        base.ExitState();
        animator.applyRootMotion = true;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        // Lerp to Player
        LerpToPlayer();

        if (Time.time - startTime < biteDuration)
        {
            // Reset collider hit box after a small time
            if (Time.time - startTime > 1.25f)
            {
                capsuleCollider.radius = colliderRadius;
            }
            RotateTowardsPlayer(biteRotationSpeed); // Rotate towards player while biting
        } else
        {
            zombieController.StateMachine.ChangeState(zombieController.CooldownState);
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    
    private void LerpToPlayer()
    {
        if (timeElapsed < lerpDuration || zombieController.GetDistanceToPlayer() < 0.25)
        {
            // Calculate the percentage of completion using the elapsed time and duration
            float t = timeElapsed / lerpDuration;
            t = 1 - Mathf.Pow(1 - t, 2); // Ease out the speed

            // Update the position of the object
            zombieController.transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // Increment the elapsed time by the time passed since last frame
            timeElapsed += Time.deltaTime;
        }
        else if (zombieController.GetDistanceToPlayer() > 0.25)
        {
            // Ensure the object's position is exactly at the calculated target position
            //zombieController.transform.position = targetPosition;

            // Set Velocity to zero in case of low friction
            //zombieController.rb.velocity = Vector3.zero;
        }
    }
    private void RotateTowardsPlayer(float rotationSpeed)
    {
        Vector3 direction = (zombieController.player.transform.position - zombieController.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        zombieController.transform.rotation = Quaternion.Slerp(zombieController.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTargetState : EnemyState
{
    private Animator animator;
    private RotateTowardsPath rotateTowardsPath;
    private PlayerDamage playerDamage;
    private Transform playerTransform;

    private float detectionDistance;
    private float reachThreshold;
    private float reachThresholdMultiplier;
    private float scaledReachThreshold;
    private float minReachThreshold;
    private float maxReachThreshold;
    public ZombieTargetState(ZombieController zombieController, EnemyStateMachine enemyStateMachine) : base(zombieController, enemyStateMachine)
    {
        animator = zombieController.animator;
        rotateTowardsPath = zombieController.GetComponent<RotateTowardsPath>();
        playerDamage = zombieController.playerDamage;
        playerTransform = zombieController.player.transform;

        detectionDistance = zombieController.GetDetectionDistance();
        reachThreshold = zombieController.GetReachThreshold();
        reachThresholdMultiplier = zombieController.GetReachThresholdMultiplier();
        minReachThreshold = zombieController.GetMinReachThreshold();
        maxReachThreshold = zombieController.GetMaxReachThreshold();
    }

    public override void AnimationTriggerEvent(ZombieController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        // Activate the rotation script
        Debug.Log("Entering Walking State");
        rotateTowardsPath.Activate(true);
    }

    public override void ExitState()
    {
        base.ExitState();
        // Deactivate the rotation script
        Debug.Log("Exiting Walking State");
        rotateTowardsPath.Activate(false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        // Scale the Reach Threshold with player speed relative zombie
        ScaleReachThreshold();

        // If the zombie is outside the detection radius + 2, switch to the idle state
        if (zombieController.GetDistanceToPlayer() > detectionDistance + 4)
        {
            animator.SetBool("Detect", false);
            zombieController.StateMachine.ChangeState(zombieController.IdleState);
        }
        // If the player is being bit while the zombie is targeting, return to idle when close enough
        if (playerDamage.GetIsBeingBitten() 
            && zombieController.GetDistanceToPlayer() < 1)
        {
            zombieController.StateMachine.ChangeState(zombieController.IdleState);
        }
        // If the reach threshold is entered and the player isn't being bit, reach toward the player
        if (zombieController.GetDistanceToPlayer() < scaledReachThreshold && !playerDamage.GetIsBeingBitten())
        {
            zombieController.StateMachine.ChangeState(zombieController.ReachState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void ScaleReachThreshold()
    {
        Vector3 playerVelocity = playerDamage.GetVelocity();

        Vector3 directionToPlayer = (playerTransform.position - zombieController.transform.position).normalized;

        // Calculate the relative speed as the projection of the player's velocity on the directionToPlayer
        float relativeSpeed = -1 * Vector3.Dot(playerVelocity, directionToPlayer);

        if (relativeSpeed <= minReachThreshold)
        {
            // If speed is just under the walk speed or less, use the default threshold
            scaledReachThreshold = reachThreshold;
        }
        else if (relativeSpeed >= maxReachThreshold)
        {
            // If speed is run speed or more, use 1.5 times the default threshold
            scaledReachThreshold = reachThreshold * reachThresholdMultiplier;
        }
        else
        {
            // Scale the threshold linearly between default and 1.5 times the default
            // Calculate how far the speed is between walk and run speed
            float t = (relativeSpeed - minReachThreshold) / (maxReachThreshold - minReachThreshold);
            // Linearly interpolate between defaultReachThreshold and defaultReachThreshold * 1.5 based on t
            scaledReachThreshold = Mathf.Lerp(reachThreshold, reachThreshold * 1.5f, t);
        }
    }
}

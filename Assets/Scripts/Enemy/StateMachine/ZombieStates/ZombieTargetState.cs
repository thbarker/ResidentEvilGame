using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTargetState : EnemyState
{
    private Animator animator;
    private AIPath aiPath;
    private RotateTowardsPath rotateTowardsPath;
    private PlayerDamage playerDamage;
    private Transform playerTransform;
    private Transform transform;

    private float reachThreshold;
    private float reachThresholdMultiplier;
    private float scaledReachThreshold;
    private float minReachThreshold;
    private float maxReachThreshold;
    private float rememberPlayerTimer;
    private float losePlayerTime;
    public ZombieTargetState(ZombieController zombieController, EnemyStateMachine enemyStateMachine) : base(zombieController, enemyStateMachine)
    {
        animator = zombieController.animator;
        rotateTowardsPath = zombieController.GetComponent<RotateTowardsPath>();
        playerDamage = zombieController.playerDamage;
        playerTransform = zombieController.player.transform;
        aiPath = zombieController.aiPath;
        transform = zombieController.transform;

        reachThreshold = zombieController.GetReachThreshold();
        reachThresholdMultiplier = zombieController.GetReachThresholdMultiplier();
        minReachThreshold = zombieController.GetMinReachThreshold();
        maxReachThreshold = zombieController.GetMaxReachThreshold();
        losePlayerTime = zombieController.GetLosePlayerTime();
    }

    public override void AnimationTriggerEvent(ZombieController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        // Activate the rotation script
        aiPath.enabled = true;
        rotateTowardsPath.Activate(true);
        rememberPlayerTimer = 0f;
    }

    public override void ExitState()
    {
        base.ExitState();
        // Deactivate the rotation script
        rotateTowardsPath.Activate(false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        // If the player is dead return to idle
        if (playerDamage.dead && zombieController.GetDistanceToPlayer() < 1f)
        {
            zombieController.StateMachine.ChangeState(zombieController.EatState);
        }
        // Scale the Reach Threshold with player speed relative zombie
        ScaleReachThreshold();

        // Track the time since last saw the player
        if(!CanSeePlayer())
            rememberPlayerTimer += Time.deltaTime;
        else
            rememberPlayerTimer = 0;
        
        // If enough time since last saw the player has passed, stop targeting
        if(rememberPlayerTimer > losePlayerTime)
            LosePlayer();
        
        // If the player is being bit while the zombie is targeting, return to idle when close enough
        if (playerDamage.GetIsBeingBitten() 
            && zombieController.GetDistanceToPlayer() < 1
            && !playerDamage.dead)
        {
            zombieController.StateMachine.ChangeState(zombieController.IdleState);
        }
        // If the reach threshold is entered and the player isn't being bit or dead, reach toward the player
        if (zombieController.GetDistanceToPlayer() < scaledReachThreshold 
            && !playerDamage.GetIsBeingBitten() 
            && !playerDamage.dead)
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

    // Returns true if the zombie can detect the player
    private bool CanSeePlayer()
    {
        // Perform raycast to detect line of sight of zombie
        Vector3 direction = playerTransform.position - transform.position;
        float distance = MapDirectionToValue(direction); 
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        direction.Normalize();
        Debug.DrawLine(origin, origin + direction * distance, Color.magenta);
        // Perform the raycast
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            // Check if the hit object is the player
            if (hit.collider.gameObject == zombieController.player)
            {
                // Detect Player
                return true;
            }
        }
        return false;
    }
    private float MapDirectionToValue(Vector3 dir)
    {
        // Normalize the direction vector to ensure it's a unit vector
        Vector3 normalizedDir = dir.normalized;

        // Calculate the dot product with the forward vector
        float dotProduct = Vector3.Dot(normalizedDir, transform.forward);

        float t = (dotProduct + 1) / 2;

        float curvedT = Mathf.Pow(t, 3);

        // Map the dot product (-1 to 1) with the desired curve to [2f, 20f]
        float mappedValue = Mathf.Lerp(2f, 20f, curvedT);

        return mappedValue;
    }

    public void LosePlayer()
    {
        animator.SetBool("Detect", false);
        zombieController.SetDetectedPlayer(false);
        zombieController.StateMachine.ChangeState(zombieController.IdleState);
    }
}

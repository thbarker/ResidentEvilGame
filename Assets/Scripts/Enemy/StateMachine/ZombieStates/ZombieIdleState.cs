using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleState : EnemyState
{
    private float minDetectionDistance;
    private float maxDetectionDistance;
    private Animator animator;
    private Transform transform;
    private GameObject player;
    private PlayerDamage playerDamage;
    public ZombieIdleState(ZombieController zombieController, EnemyStateMachine enemyStateMachine) : base(zombieController, enemyStateMachine)
    {
        animator = zombieController.GetComponent<Animator>();
        transform = zombieController.transform;
        player = zombieController.player;
        playerDamage = zombieController.playerDamage;
        minDetectionDistance = zombieController.GetMinDetectionDistance();
        maxDetectionDistance = zombieController.GetMaxDetectionDistance();
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
        animator.SetBool("Idle", false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        if (!playerDamage.dead && !zombieController.dead)
        {
            // Perform raycast to detect line of sight of zombie
            Vector3 direction = player.transform.position - transform.position;
            float distance = MapDirectionToValue(direction);
            direction.Normalize();
            Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            Debug.DrawLine(origin, origin + direction * distance, Color.magenta);
            // Perform the raycast
            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
            {
                Debug.LogWarning(hit.collider);
                // Check if the hit object is the player
                if (hit.collider.gameObject == player)
                {
                    // Detect Player
                    DetectPlayer();
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    public  void DetectPlayer()
    {
        animator.SetBool("Detect", true);
        zombieController.SetDetectedPlayer(true);
        zombieController.StateMachine.ChangeState(zombieController.TargetState);
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
        float mappedValue = Mathf.Lerp(minDetectionDistance, maxDetectionDistance, curvedT);

        return mappedValue;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class ZombieReachState : EnemyState
{
    private GameObject player;
    private PlayerDamage playerDamage;
    private PlayerMovement playerMovement;
    private Animator animator;
    private ReachCollision reachCollision;
    private Transform zombieTransform;
    private UIManager uiManager;
    private float reachRotationSpeed;
    private float rotationSpeedDynamic;
    private float reachDuration;
    private float biteThreshold;
    float startTime;
    public ZombieReachState(ZombieController zombieController, EnemyStateMachine enemyStateMachine) : base(zombieController, enemyStateMachine)
    {
        player = zombieController.player;
        playerDamage = zombieController.playerDamage;
        playerMovement = playerDamage.GetComponent<PlayerMovement>();
        animator = zombieController.animator;
        reachCollision = zombieController.reachCollisionScript;
        zombieTransform = zombieController.transform;
        // Get a reference to uimanager
        uiManager = GameObject.FindWithTag("Player")?.transform.Find("UIManager")?.GetComponent<UIManager>();

        reachRotationSpeed = zombieController.GetReachRotationSpeed();
        reachDuration = zombieController.GetReachDuration();
        biteThreshold = zombieController.GetBiteThreshold();
    }

    public override void AnimationTriggerEvent(ZombieController.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState(); 
        animator.SetTrigger("Reach");
        zombieController.PlayGroan();
        startTime = Time.time;
        rotationSpeedDynamic = 0.5f * reachRotationSpeed;
    }

    public override void ExitState()
    {
        base.ExitState();
        zombieController.bite = false;
        animator.ResetTrigger("Reach");
        reachCollision.Activate(false);
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        // If the player is being bit or dead or using a door while the zombie is reaching, return to idle
        if (playerDamage.GetIsBeingBitten() || playerDamage.dead || playerMovement.isUsingDoor)
        {
            zombieController.StateMachine.ChangeState(zombieController.IdleState);
        }
        if(uiManager.uiActive)
        {
            startTime += Time.deltaTime;
        } else
        {
            // During the duration of the reach, the zombie rotates towards the player
            if (Time.time - startTime < reachDuration)
            {
                // Don't bite immidiately at the beginning of the reach for the sake of animation
                if (Time.time - startTime > 0.5f)
                    reachCollision.Activate(true);
                if (zombieController.bite)
                    zombieController.StateMachine.ChangeState(zombieController.BiteState);
                RotateTowardsPlayer(rotationSpeedDynamic); // Rotate towards player while reaching
            }
            // After the reach, change to a cooldown
            else
            {
                zombieController.StateMachine.ChangeState(zombieController.CooldownState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
    private void RotateTowardsPlayer(float rotationSpeed)
    {
        Vector3 direction = (player.transform.position - zombieTransform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float t = Time.deltaTime * rotationSpeed;
        zombieTransform.rotation = Quaternion.Slerp(zombieTransform.rotation, lookRotation, t);

    }
}

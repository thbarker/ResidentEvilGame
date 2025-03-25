using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieEatState : EnemyState
{
    private float minDetectionDistance;
    private float maxDetectionDistance;
    private Animator animator;
    private Rigidbody rb;
    private Transform transform;
    private GameObject player;
    private PlayerDamage playerDamage;
    private float duration; 
    private float timeElapsed;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    private float startPosition;
    private float targetPosition;
    public ZombieEatState(ZombieController zombieController, EnemyStateMachine enemyStateMachine) : base(zombieController, enemyStateMachine)
    {
        animator = zombieController.GetComponent<Animator>();
        rb = zombieController.rb;
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
        animator.SetTrigger("Eat");
        zombieController.GetComponent<CapsuleCollider>().radius = 0.15f;
        zombieController.GetComponent<CapsuleCollider>().height = 0.5f;
        zombieController.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.5f, 0);
        rb.constraints &= ~RigidbodyConstraints.FreezeRotationX;
        timeElapsed = 0;
        duration = 1f; 
        startRotation = transform.rotation;
        targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x - 20, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        startPosition = transform.position.y;
        targetPosition = transform.position.y + 0.1f;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        RotateXAxis();
    }

    private void RotateXAxis()
    {
        if (timeElapsed < duration)
        {
            // Interpolate rotation towards the target rotation over the specified duration
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, timeElapsed / duration);
            transform.position = new Vector3(transform.position.x,
                                            Mathf.Lerp(startPosition, targetPosition, timeElapsed / duration),
                                            transform.position.z);
            timeElapsed += Time.deltaTime; // Increment the time elapsed by the delta time since the last frame
        }
        else
        {
            // Ensure the rotation is set to the target rotation exactly at the end of the duration
            transform.rotation = targetRotation;
            transform.position = new Vector3(transform.position.x,
                                            targetPosition,
                                            transform.position.z);
        }
    }

}

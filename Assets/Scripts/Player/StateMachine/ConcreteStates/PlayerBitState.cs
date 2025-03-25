using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBitState : PlayerState
{
    private float lerpTime;
    private float biteDuration;
    private float currentLerpTime;
    private float startTime;
    private PlayerDamage playerDamage;
    private Rigidbody rb;
    private Animator animator;
    private Transform transform;
    private Transform zombieTransform;
    public PlayerBitState(PlayerMovement playerMovement, PlayerStateMachine playerStateMachine) : base(playerMovement, playerStateMachine)
    {
        transform = playerMovement.transform;
        rb = playerMovement.rb;
        animator = playerMovement.animator;
        playerDamage = playerMovement.GetComponent<PlayerDamage>();
        lerpTime = playerDamage.GetLerpTime();
        biteDuration = playerDamage.GetBiteDuration();
    }

    public override void AnimationTriggerEvent(PlayerMovement.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EnterState()
    {
        base.EnterState();
        rb.velocity = Vector3.zero;
        animator.SetTrigger("GetBit");
        animator.SetBool("GettingBit", true);
        zombieTransform = playerDamage.zombieTransform;
        currentLerpTime = 0.0f; // Reset the lerp time
        playerDamage.SetIsBeingBitten(true);
        startTime = Time.time;
    }

    public override void ExitState()
    {
        base.ExitState();
        playerDamage.SetIsBeingBitten(false);
        animator.SetBool("GettingBit", false);
        playerDamage.ResetBitingZombie();
    }

    public override void FrameUpdate()
    {
        
        base.FrameUpdate(); 
        if (Time.time - startTime > 0.5)
        {
            animator.ResetTrigger("GetBit");
        }
        if (Time.time - startTime > biteDuration)
        {
            if(playerDamage.GetHealth() <= 0)
            {
                playerMovement.StateMachine.ChangeState(playerMovement.DieState);
            } else
            {
                playerMovement.StateMachine.ChangeState(playerMovement.MoveState);
            }
        }
        // Increment the lerp time
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
        }

        // Calculate the lerp time 
        float t = currentLerpTime / lerpTime;

        // Lerp the rotation to face the zombie
        Quaternion targetRotation = Quaternion.LookRotation(zombieTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

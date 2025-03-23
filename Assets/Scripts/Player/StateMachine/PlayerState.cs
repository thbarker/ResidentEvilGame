using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerMovement playerMovement;
    protected PlayerStateMachine playerStateMachine;

    public PlayerState(PlayerMovement playerMovement, PlayerStateMachine playerStateMachine)
    {
        this.playerMovement = playerMovement;
        this.playerStateMachine = playerStateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void AnimationTriggerEvent(PlayerMovement.AnimationTriggerType triggerType) { }

}

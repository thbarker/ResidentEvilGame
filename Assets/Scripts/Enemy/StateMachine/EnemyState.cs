using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected ZombieController zombieController;
    protected EnemyStateMachine enemyStateMachine;

    public EnemyState(ZombieController zombieController, EnemyStateMachine enemyStateMachine)
    {
        this.zombieController = zombieController;
        this.enemyStateMachine = enemyStateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void AnimationTriggerEvent(ZombieController.AnimationTriggerType triggerType) { }

}

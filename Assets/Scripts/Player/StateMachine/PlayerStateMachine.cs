using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentEnemyState { get; set; }

    public void Initialize(PlayerState enemyState)
    {
        CurrentEnemyState = enemyState;
        CurrentEnemyState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentEnemyState.ExitState();
        CurrentEnemyState = newState;
        CurrentEnemyState.EnterState();
    }

}
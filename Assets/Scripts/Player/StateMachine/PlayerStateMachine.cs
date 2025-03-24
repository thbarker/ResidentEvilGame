using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentPlayerState { get; set; }

    public void Initialize(PlayerState playerState)
    {
        CurrentPlayerState = playerState;
        CurrentPlayerState.EnterState();
    }

    public void ChangeState(PlayerState newState)
    {
        CurrentPlayerState.ExitState();
        CurrentPlayerState = newState;
        CurrentPlayerState.EnterState();
    }

}
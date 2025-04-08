using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private PlayerControls controls;
    public bool uiActive = false;
    public ZombieList zombieList;
    public PlayerMovement playerMovement;
    private void Awake()
    {
        zombieList = GameObject.FindWithTag("Player")?.transform.Find("ZombieList")?.GetComponent<ZombieList>();
        playerMovement = GameObject.FindWithTag("Player")?.GetComponent<PlayerMovement>();
    }
    private void Start()
    {
        controls = PlayerInputManager.controls;
    }
    public void StartUI()
    {
        PlayerInputManager.ToggleActionMap(controls.UI);
        uiActive = true;
        playerMovement.currentRoom.PauseAllZombies();
        playerMovement.StateMachine.ChangeState(playerMovement.IdleState);
    }
    public void EndUI()
    {
        PlayerInputManager.ToggleActionMap(controls.Player);
        uiActive = false;
        playerMovement.currentRoom.ResumeAllZombies();
        playerMovement.StateMachine.ChangeState(playerMovement.MoveState);
    }
}

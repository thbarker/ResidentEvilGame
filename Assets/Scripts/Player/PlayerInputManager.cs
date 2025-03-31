using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerControls controls;
    public static event Action<InputActionMap> actionMapChange;
    private void Awake()
    {
        controls = new PlayerControls();
        ToggleActionMap(controls.Player);
    }
    private void OnEnable()
    {
        //controls.Player.Enable();
    }
    private void OnDisable()
    {
        //controls.Player.Disable();
    }
    public static void ToggleActionMap(InputActionMap actionMap)
    {
        if (actionMap.enabled)
            return;
        controls.Disable();
        actionMapChange?.Invoke(actionMap);
        actionMap.Enable();
    }
}

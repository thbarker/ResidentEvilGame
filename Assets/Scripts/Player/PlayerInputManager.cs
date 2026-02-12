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
        // Ensure only one active initializer per scene lifetime
        if (controls == null)
        {
            controls = new PlayerControls();
        }

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
    private void OnDestroy()
    {
        // CRITICAL: prevent dead subscribers after scene reload
        actionMapChange = null;

        // Optional but recommended: clean up Input System native state
        controls?.Dispose();
        controls = null;
    }
    public static void ToggleActionMap(InputActionMap actionMap)
    {
        if (controls == null || actionMap == null)
            return;

        if (actionMap.enabled)
            return;

        controls.Disable();

        // Safe invoke — subscribers are guaranteed alive
        actionMapChange?.Invoke(actionMap);

        actionMap.Enable();
    }
}
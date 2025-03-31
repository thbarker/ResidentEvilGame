using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private PlayerControls controls;
    public bool uiActive = false;
    private void Start()
    {
        controls = PlayerInputManager.controls;
    }
    public void StartUI()
    {
        PlayerInputManager.ToggleActionMap(controls.UI);
        uiActive = true;
        //Time.timeScale = 0;
    }
    public void EndUI()
    {
        PlayerInputManager.ToggleActionMap(controls.Player);
        uiActive = false;
        Time.timeScale = 1;
    }
}

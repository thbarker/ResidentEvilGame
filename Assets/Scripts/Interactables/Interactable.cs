using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected bool hasFocusCamera = false;
    private FocusCamera focusCamera;
    protected virtual void Awake()
    {
        if (GetComponent<FocusCamera>())
        {
            hasFocusCamera = true;
            focusCamera = GetComponent<FocusCamera>();
        }
    }
    
    public virtual void Interact() 
    { 
        if (hasFocusCamera)
        {
            focusCamera.Activate();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontGate : Door
{
    public GameObject chain;
    public override void Interact()
    {
        base.Interact();
        if(chain && locked == false)
            chain.SetActive(false);
    }
}

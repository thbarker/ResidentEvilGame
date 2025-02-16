using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachCollision : MonoBehaviour
{
    public TestTriggerStand script;
    public bool active = false;

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && active)
        {
            script.Bite();
        }
    }

    public void Activate(bool on)
    {
        active = on;
    }
}

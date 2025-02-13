using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachCollision : MonoBehaviour
{
    public TestTriggerStand script;
    private bool active = false;

    private void OnTriggerEnter(Collider other)
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

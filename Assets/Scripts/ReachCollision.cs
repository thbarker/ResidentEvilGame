using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachCollision : MonoBehaviour
{
    public TestTriggerStand script;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            script.Bite();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachCollision : MonoBehaviour
{
    public ZombieController controllerScript;
    public bool active = false;

    private void Start()
    {
        controllerScript = transform.parent.GetComponent<ZombieController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && active)
        {
            Debug.Log("Reach Collision is calling Bite()");
            controllerScript.Bite();
        }
    }

    public void Activate(bool on)
    {
        active = on;
    }
}

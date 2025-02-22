using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pushable : MonoBehaviour
{
    GameObject player;
    Rigidbody rb;
    PlayerDamage playerDamage;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
        if(player)
        {
            playerDamage = player.GetComponent<PlayerDamage>();
        } else
        {
            Debug.Log("No player found");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PushBack()
    {
        if (player != null)
        {
            Debug.Log("Pushing");
            Vector3 forceDirection = transform.position - player.transform.position; // Calculate direction from player to this object
            forceDirection.Normalize(); // Normalize the direction vector to have a magnitude of 1
            rb.AddForce(forceDirection * 100 * playerDamage.GetPushForce(), ForceMode.Impulse); // Apply the force
        }
    }
}

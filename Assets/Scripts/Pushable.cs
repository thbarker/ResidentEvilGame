using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AIPath))]
public class Pushable : MonoBehaviour
{
    private GameObject player;
    private Rigidbody rb;
    private PlayerDamage playerDamage;
    private AIPath aiPath;
    private Animator animator;
    private TestTriggerStand controllerScript;
    public float decelerationRate = 1f;

    private bool pushingBack = false;

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
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        controllerScript = GetComponent<TestTriggerStand>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pushingBack)
        {
            if (rb.velocity.magnitude < 0.01)
            {
                // Enable the pathing when pushback is done
                aiPath.enabled = true;
                pushingBack = false; 
            } else
            {
                // Slow down after the push
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * decelerationRate);
            }
        }
    }
    public void PushBack()
    {
        if (player != null)
        {
            Debug.Log(this.name + "Getting Pushed");
            animator.applyRootMotion = false;
            if(playerDamage.GetBitingZombie() != gameObject)
                controllerScript.PushBackAnimation(); // Animate the zombie
            pushingBack = true;
            Vector3 forceDirection = transform.position - player.transform.position; // Calculate direction from player to this object
            forceDirection.Normalize(); // Normalize the direction vector to have a magnitude of 1
            rb.velocity = Vector3.zero; // Reset the velocity before the push
            rb.AddForce(forceDirection * 100 * playerDamage.GetPushForce(), ForceMode.Impulse); // Apply the force
        }
    }
}

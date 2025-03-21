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
    private ZombieController controllerScript;
    public float decelerationRate = 1f;

    public bool pushingBack = false;

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
        controllerScript = GetComponent<ZombieController>();
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
                controllerScript.StateMachine.ChangeState(controllerScript.KnockbackState);
            pushingBack = true;
            Vector3 forceDirection = transform.position - player.transform.position; // Calculate direction from player to this object
            forceDirection.Normalize(); // Normalize the direction vector to have a magnitude of 1
            rb.velocity = Vector3.zero; // Reset the velocity before the push
            // Calculate the distance between the player and this object
            float distance = Vector3.Distance(transform.position, player.transform.position);
            // Use distance to scale the force, lessening the force the farther the object is
            float scaledForce = (100 * playerDamage.GetPushForce()) / Mathf.Clamp(distance, 1, float.MaxValue); // Example scaling factor
            // Apply the scaled force
            Vector3 pushForce = forceDirection * scaledForce;
            rb.AddForce(pushForce, ForceMode.Impulse); // Apply the force
        }
    }
}

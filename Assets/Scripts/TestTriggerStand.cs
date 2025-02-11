using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTriggerStand : MonoBehaviour
{
    public Animator animator;
    public GameObject player;
    public RotateTowardsPath script;
    public Transform biteTransform; // This is where the player will be when the bite animation occurs
    public Rigidbody rb;

    public string[] targetStates;

    private bool shouldTarget = false;
    private bool canBite = false; // Variable to control the bite cooldown
    private float distanceToPlayer = 0f;
    private bool cooldownActive = false;


    [SerializeField]
    private float biteThreshold;
    [SerializeField]
    private float biteCooldown = 5f; // Cooldown in seconds before the zombie can bite again

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargetting();
        DetectDistanceToPlayer();
        UpdateAnimController();
    }

    void UpdateTargetting()
    {
        // Detect if the zombie should be targeting the player
        shouldTarget = false;
        foreach (string s in targetStates)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(s))
            {
                shouldTarget = true;
                if(!cooldownActive)
                {
                    canBite = true;
                }
            }
        }
    }

    void DetectDistanceToPlayer()
    {
        // Detect Distance To Player to activate a Bite
        if (player)
            distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        else
        {
            Debug.Log("No Player GameObject Found");
        }
    }

    void UpdateAnimController()
    {
        if(animator)
        {
            animator.SetBool("CanBite", canBite);
        }
        // Bite if the player is too close and the zombie can bite
        if (distanceToPlayer < biteThreshold && canBite)
        {
            if (animator)
                animator.SetTrigger("Bite");
            if (player && player.GetComponent<PlayerDamage>())
            {
                Debug.Log("Calling GetBit()");
                player.GetComponent<PlayerDamage>().GetBit(biteTransform, transform);
                canBite = false; // Disable further bites until cooldown is over
                StartCoroutine(BiteCooldown());
            }
        }

        if (Input.GetButtonDown("Jump") && animator)
        {
            animator.SetTrigger("Stand");
        }
        if (script && animator && shouldTarget)
        {
            script.Activate(true);
        }
        else
        {
            script.Activate(false);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("Hit");
        }
    }

    IEnumerator BiteCooldown()
    {
        cooldownActive = true;
        yield return new WaitForSeconds(biteCooldown);
        canBite = true; // Re-enable biting after cooldown
        cooldownActive = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestTriggerStand : MonoBehaviour
{
    public Animator animator;
    public GameObject player;
    public RotateTowardsPath script;
    public Transform biteTransform; // This is where the player will be when the bite animation occurs
    public Rigidbody rb;
    public bool startStanding = true;
    public ReachCollision reachCollisionScript;

    public string[] targetStates;

    private bool shouldTarget = false;
    private bool canReach = true; // Variable to control the reach cooldown
    private bool canBite = false; // Variable to control if zombie can bite
    private float distanceToPlayer = 0f;
    private bool cooldownActive = false;
    private bool allowReachRotation = true;
    private float walkReachTransitionCounter = 0f;


    [SerializeField]
    private float biteThreshold, reachThreshold, reachDuration, biteCooldown;
    [SerializeField]
    private float reachCooldown = 5f; // Cooldown in seconds before the zombie can bite again
    [SerializeField]
    private float detectionDistance = 10f;
    [SerializeField]
    private float reachRotationSpeed = 1f;
    [SerializeField]
    private float lockedYPosition = 0;

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
        UpdateY();
    }

    void UpdateTargetting()
    {
        // Detect if the zombie should be targeting the player
        shouldTarget = false;
        foreach (string s in targetStates)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName(s) && canReach)
            {
                shouldTarget = true;
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
            animator.SetBool("StartStanding", startStanding);
            animator.SetBool("CanReach", canReach);
        }
        if(distanceToPlayer < detectionDistance)
        {
            animator.SetTrigger("Detect");
        }
        if (distanceToPlayer < reachThreshold && canReach)
        {
            // Reach toward the player if the reach threshold is entered
            StartCoroutine(Reach());
        }

        if (Input.GetButtonDown("Jump") && animator)
        {
            animator.SetTrigger("Stand");
        }

        UpdateRotationScript();

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("Hit");
            StartCoroutine(ResetHitTrigger());
        }
    }

    private void UpdateRotationScript()
    {
        if (script && animator && shouldTarget)
        {
            script.Activate(true);
        }
        else
        {
            script.Activate(false);
        }
    }

    // This function makes sure that the Y position of the zombie doesn't shift 
    private void UpdateY()
    {
        transform.position = new Vector3(transform.position.x, lockedYPosition, transform.position.z);
    }
    IEnumerator Reach()
    {
        canReach = false;
        canBite = true;
        Debug.Log("Setting Can Bite to True");
        reachCollisionScript.Activate(true);
        shouldTarget = false;
        allowReachRotation = true;
        animator.SetTrigger("Reach");
        animator.ResetTrigger("StopReaching");
        Debug.Log("Reaching");

        float startTime = Time.time;
        while (Time.time - startTime < reachDuration)
        {
            RotateTowardsPlayer(); // Rotate towards player while reaching
            yield return null;
        }

        animator.SetTrigger("StopReaching");
        animator.ResetTrigger("Reach");
        canBite = false;
        Debug.Log("Setting Can Bite to False");
        reachCollisionScript.Activate(false);
        Debug.Log("StopReaching");

        StartCoroutine(ReachCooldown());
        StopCoroutine(Reach());
    }

    private void RotateTowardsPlayer()
    {
        if (player && allowReachRotation)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * reachRotationSpeed);
        }
    }
    IEnumerator BiteCooldown()
    {
        yield return new WaitForSeconds(biteCooldown);
        canReach = true;
        StopCoroutine(BiteCooldown());
    }
    IEnumerator ReachCooldown()
    {
        yield return new WaitForSeconds(reachCooldown);
        canReach = true;
        StopCoroutine(ReachCooldown());
    }

    IEnumerator ResetHitTrigger()
    {
        yield return null;
        animator.ResetTrigger("Hit");
    }

    public void Bite()
    {
        // Bite if can Bite
        if (canBite)
        {
            if (animator)
                animator.SetTrigger("Bite");
            if (player && player.GetComponent<PlayerDamage>())
            {
                Debug.Log("Calling GetBit()");
                canReach = false; // Disable further bites until cooldown is over
                canBite = false;
                Debug.Log("Setting Can Bite to False");
                reachCollisionScript.Activate(false);
                player.GetComponent<PlayerDamage>().GetBit(biteTransform, transform);
                allowReachRotation = false;  // Disable rotation when biting
                StopCoroutine(Reach());
                StartCoroutine(BiteCooldown());
            }
        }
        
    }

}

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
    private float biteThreshold, reachThreshold, reachDuration, biteDuration;
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
        reachCollisionScript.Activate(true);
        shouldTarget = false;
        allowReachRotation = true;
        animator.SetTrigger("Reach");
        animator.ResetTrigger("StopReaching");
        Debug.Log("Reaching");

        canBite = true;
        Debug.Log("Setting Can Bite to True");

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
        yield return new WaitForSeconds(reachCooldown);
        canReach = true;
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
    IEnumerator BiteSequence()
    {
        Debug.Log("Calling GetBit()");
        player.GetComponent<PlayerDamage>().GetBit(biteTransform, transform);
        canReach = false; // Disable further reaches until bite sequence is over
        canBite = false; // Disable further bite attempts until bite sequence is over
        Debug.Log("Setting Can Bite to False");
        animator.SetTrigger("Bite"); // Trigger the Bite animation
        reachCollisionScript.Activate(false); // Disable the reach collision box
        allowReachRotation = false;  // Disable rotation when biting
        yield return null;
        animator.ResetTrigger("Bite"); // Reset the Trigger for the Bite animation next frame
        yield return new WaitForSeconds(biteDuration);
        canReach = true;
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
            if (player && player.GetComponent<PlayerDamage>())
            {
                StopCoroutine(Reach());
                StartCoroutine(BiteSequence());
            }
        }
        
    }

}

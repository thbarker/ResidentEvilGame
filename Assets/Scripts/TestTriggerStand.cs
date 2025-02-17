using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestTriggerStand : MonoBehaviour
{
    public Animator animator;
    private CapsuleCollider collider;
    public GameObject player;
    public RotateTowardsPath script;
    public Transform biteTransform; // This is where the player will be when the bite animation occurs
    public Rigidbody rb;
    public bool startStanding = true;
    public ReachCollision reachCollisionScript;

    public string[] targetStates;

    private bool shouldTarget = false;
    public bool canReach = true; // Variable to control the reach cooldown
    public bool canBite = false; // Variable to control if zombie can bite
    private float distanceToPlayer = 0f;
    private bool reachCoroutineRunning = false;
    private bool lerpCoroutineRunning = false;
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
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
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
        if (reachCoroutineRunning == true)
            yield break;
        reachCoroutineRunning = true;
        canReach = false;
        shouldTarget = false;
        allowReachRotation = true;
        animator.SetTrigger("Reach");

        // The zombie doesn't activate a bite until the reach is 0.5 seconds in
        yield return new WaitForSeconds(0.5f);
        reachCollisionScript.Activate(true);
        canBite = true;

        float startTime = Time.time;

        // During the duration of the reach, the zombie rotates towards the player
        while (Time.time - startTime < reachDuration - 0.5f)
        {
            RotateTowardsPlayer(); // Rotate towards player while reaching
            yield return null;
        }

        animator.ResetTrigger("Reach");
        yield return new WaitForSeconds(1f);
        canBite = false;
        reachCollisionScript.Activate(false);
        yield return new WaitForSeconds(reachCooldown);
        canReach = true;
        Debug.Log("Setting can Reach to True");
        reachCoroutineRunning = false;
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
        animator.applyRootMotion = false;
        collider.radius = 0.1f;
        player.GetComponent<PlayerDamage>().GetBit(biteTransform, transform);
        canReach = false; // Disable further reaches until bite sequence is over
        canBite = false; // Disable further bite attempts until bite sequence is over
        animator.SetTrigger("Bite"); // Trigger the Bite animation
        reachCollisionScript.Activate(false); // Disable the reach collision box
        allowReachRotation = false;  // Disable rotation when biting
        StartCoroutine(LerpToPlayer());
        yield return null;
        animator.ResetTrigger("Bite"); // Reset the Trigger for the Bite animation next frame
        yield return new WaitForSeconds(biteDuration);
        collider.radius = 0.1f;
        canReach = true;
    }

    IEnumerator LerpToPlayer()
    {
        if (lerpCoroutineRunning)
        {
            yield break;
        }
        lerpCoroutineRunning = true;

        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.25f);

        Vector3 startPosition = transform.position;  // Start position of the object
        // Calculate the target position as 0.5 units in front of the player
        Vector3 targetPosition = player.transform.position + player.transform.forward * 0.5f;

        float timeElapsed = 0f;
        float lerpDuration = 0.5f;  // Duration over which the lerp takes place

        while (timeElapsed < lerpDuration)
        {
            // Calculate the percentage of completion using the elapsed time and duration
            float t = timeElapsed / lerpDuration;
            t = t * t * (3f - 2f * t);  // Applying smoothstep for a smoother interpolation

            // Update the position of the object
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            // Increment the elapsed time by the time passed since last frame
            timeElapsed += Time.deltaTime;

            // Wait until the next frame before continuing the loop
            yield return null;
        }

        // Ensure the object's position is exactly at the calculated target position
        transform.position = targetPosition;

        // Set Velocity to zero in case of low friction
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(1f);
        animator.applyRootMotion = true;
        lerpCoroutineRunning = false; // Reset the flag
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

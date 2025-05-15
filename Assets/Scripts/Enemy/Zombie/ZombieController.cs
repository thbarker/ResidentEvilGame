using Pathfinding;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieController : Damageable
{
    #region State Machine
    public EnemyStateMachine StateMachine { get; set; }
    public ZombieIdleState IdleState { get; set; }
    public ZombieTargetState TargetState { get; set; }
    public ZombieReachState ReachState { get; set; }
    public ZombieCooldownState CooldownState { get; set; }
    public ZombieBiteState BiteState { get; set; }
    public ZombieKnockbackState KnockbackState { get; set; }
    public ZombieDieState DieState { get; set; }
    public ZombieEatState EatState { get; set; }
    #endregion

    #region Components
    public Animator animator;
    public AIPath aiPath;
    public CapsuleCollider capsuleCollider;
    public GameObject player;
    public PlayerDamage playerDamage;
    public RotateTowardsPath rotateTowardsPath;
    public Transform biteTransform; // This is where the player will be when the bite animation occurs
    public Rigidbody rb;
    public ReachCollision reachCollisionScript;
    private ZombieList zombieList;
    public AudioSource zombieAudioSource;
    public AudioClip grassFootstepClip;
    public AudioClip concreteFootstepClip;
    public AudioClip biteClip;
    public AudioClip[] reachClips;
    #endregion

    #region Helpers
    public float colliderRadius;
    public bool startStanding = true;
    private bool shouldTarget = false;
    private float walkReachTransitionCounter = 0f;
    private bool detectedPlayer = false;
    public bool dead = false;
    public bool bite = false;
    private float knockbackThreshold;
    #endregion

    #region Tunables
    [SerializeField]
    [Tooltip("Time during a reach that a bite is allowed.")]
    private float biteThreshold = 0.5f;
    [SerializeField]
    [Tooltip("Distance that a reach should be triggered.")]
    private float reachThreshold = 1.75f;
    [SerializeField]
    [Tooltip("Amount that a reach threshold increases with relative player speed. 1 means no change to the reach threshold, > 1 means the faster the player moves towards the zombie, the sooner it can begin to reach.")]
    private float reachThresholdMultiplier = 1.25f;
    [SerializeField]
    [Tooltip("Time it takes to perform a reach.")]
    private float reachDuration = 2;
    [SerializeField]
    [Tooltip("Time it takes to perform a bite.")]
    private float biteDuration = 3;
    [SerializeField]
    [Tooltip("Time it takes to reach again after reaching/biting.")]
    private float reachCooldown = 1.5f; // Cooldown in seconds before the zombie can bite again
    [SerializeField]
    [Tooltip("Distance for detection when player is directly behind the zombie. Note that this scales with direction in a cubic curve.")]
    private float minDetectionDistance = 2f;
    [SerializeField]
    [Tooltip("Distance for detection when player is directly in front of the zombie. Note that this scales with direction in a cubic curve.")]
    private float maxDetectionDistance = 20f;
    [SerializeField]
    [Tooltip("Time it takes to forget about the player, and stop targeting.")]
    private float losePlayerTime = 4f;
    [SerializeField]
    [Tooltip("Speed at which the zombie rotates towards player when reaching.")]
    private float reachRotationSpeed = 5f;
    [SerializeField]
    [Tooltip("Force added to the zombie at the start of the reach toward the player.")]
    private float reachBoost = 1f;
    [SerializeField]
    [Tooltip("Speed at which the zombie rotates towards player when biting.")]
    private float biteRotationSpeed = 20f;
    [SerializeField]
    private float lockedYPosition = 0;
    [SerializeField]
    [Tooltip("Minimum Reach Trigger Distance.")]
    private float minReachThreshold = 1f;
    [SerializeField]
    [Tooltip("Maximum Reach Trigger Distance.")]
    private float maxReachThreshold = 1.7f; [SerializeField]
    [Tooltip("Minimum Damage Range for a random knockback on damage.")]
    [Range(0, 100f)]
    private float minKnockThreshold = 10f;
    [SerializeField]
    [Tooltip("Maximum Damage Range for a random knockback on damage.")]
    [Range(0, 100f)]
    private float maxKnockThreshold = 30f;
    [SerializeField]
    [Tooltip("How big of a range should the random health variation be.")]
    [Range(0, 500)]
    private int healthVariation = 30;
    [SerializeField]
    [Tooltip("How much damage the zombie deals when biting.")]
    [Range(0, 500)]
    private int biteDamage = 25;

    void OnValidate()
    {
        // Ensure min is always less than max
        if (minKnockThreshold >= maxKnockThreshold)
        {
            maxKnockThreshold = minKnockThreshold + 1f; // Increment max to be just above min
        }
        if(healthVariation > (health) / 2)
        { 
            healthVariation = (health) / 2;
        }
    }
    #endregion

    #region Getters
    public float GetMinReachThreshold()
    {
        return minReachThreshold;
    }
    public float GetMaxReachThreshold()
    {
        return maxReachThreshold;
    }
    public float GetMinDetectionDistance()
    {
        return minDetectionDistance;
    }
    public float GetMaxDetectionDistance()
    {
        return maxDetectionDistance;
    }
    public float GetReachThreshold()
    {
        return reachThreshold;
    }
    public float GetReachThresholdMultiplier()
    {
        return reachThresholdMultiplier;
    }
    public float GetReachRotationSpeed()
    {
        return reachRotationSpeed;
    }
    public float GetReachDuration()
    {
        return reachDuration;
    }

    public float GetBiteThreshold()
    {
        return biteThreshold;
    }
    public float GetReachCooldown()
    {
        return reachCooldown;
    }
    public float GetBiteDuration()
    {
        return biteDuration;
    }
    public float GetBiteRotationSpeed()
    {
        return biteRotationSpeed;
    }
    public float GetLosePlayerTime()
    {
        return losePlayerTime;
    }
    public bool GetDetectedPlayer()
    {
        return detectedPlayer;
    }
    public int GetBiteDamage()
    {
        return biteDamage;
    }
    #endregion

    #region Setters
    public void SetDetectedPlayer(bool detectedPlayer)
    {
        this.detectedPlayer = detectedPlayer;
    }
    #endregion

    public override void ApplyDamage(int damage)
    {
        health -= damage;
        knockbackThreshold -= damage;
        if(knockbackThreshold <= 0 && health > 0)
        {
            StateMachine.ChangeState(KnockbackState);
            knockbackThreshold = Random.Range(minKnockThreshold, maxKnockThreshold);
        }
        if(knockbackThreshold > 0 && health > 0)
        {
            // Potentially add a hit reaction that doesn't slow the movement
            // Headshot();
        }
    }

    protected override void Die()
    {
        base.Die();
        zombieList.Remove(gameObject);
        StateMachine.ChangeState(DieState);
        dead = true;
    }

    public void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }

    public enum AnimationTriggerType
    {
        ApplyRootMotion,
        DealDamage
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        colliderRadius = capsuleCollider.radius;
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
        playerDamage = player.GetComponent<PlayerDamage>();
        aiPath = GetComponent<AIPath>();
        rotateTowardsPath = GetComponent<RotateTowardsPath>();
        zombieList = player.transform.Find("ZombieList").GetComponent<ZombieList>();

        if (!zombieList) Debug.LogError("Scene must have a zombie list gameobject tagged as ZombieList");
        else
        {
            zombieList.Add(gameObject);
        }

        StateMachine = new EnemyStateMachine();

        IdleState = new ZombieIdleState(this, StateMachine);
        TargetState = new ZombieTargetState(this, StateMachine);
        ReachState = new ZombieReachState(this, StateMachine);
        CooldownState = new ZombieCooldownState(this, StateMachine);
        BiteState = new ZombieBiteState(this, StateMachine);
        KnockbackState = new ZombieKnockbackState(this, StateMachine);
        DieState = new ZombieDieState(this, StateMachine);
        EatState = new ZombieEatState(this, StateMachine);

        health = Random.Range(health - healthVariation, health + healthVariation);
        health = Mathf.Clamp(health, 0, int.MaxValue);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!dead)
            StateMachine.Initialize(IdleState);
        knockbackThreshold = Random.Range(minKnockThreshold, maxKnockThreshold);
    }

    // Update is called once per frame
    void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();

        if (Input.GetKeyDown(KeyCode.H)) 
        {
            TakeDamage(10);
        }

        DebugTimeScale();
        UpdateAnimController();

    }
    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }


    void DebugTimeScale()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Time.timeScale -= 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            Time.timeScale = 1f;
        }
    }

    public float GetDistanceToPlayer()
    {
        // Detect Distance To Player to activate a Bite
        if (player)
            return Vector3.Distance(transform.position, player.transform.position);
        else
        {
            Debug.Log("No Player GameObject Found");
            return 0;
        }
    }

    void UpdateAnimController()
    {
        if (dead)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Headshot();
        }
    }


    public void Headshot()
    {
        animator.SetTrigger("Hit");
        StartCoroutine(ResetHitTrigger());
    }
    public void PushBackAnimation()
    {
        animator.SetTrigger("PushBack");
        StartCoroutine(ResetPushBackTrigger());
    }
    IEnumerator ResetHitTrigger()
    {
        yield return null;
        animator.ResetTrigger("Hit");
    }
    IEnumerator ResetPushBackTrigger()
    {
        yield return new WaitForSeconds(0.25f); ;
        animator.ResetTrigger("PushBack");
        yield return new WaitForSeconds(reachCooldown);
    }

    public void Bite()
    {
        if (player && playerDamage)
        {/*
            if (reachCoroutine != null)
            {
                EndReachCoroutine();
            }
            reachCoroutine = null;
            StartCoroutine(BiteSequence());*/
            bite = true;
        }

    }

    public void ApplyRootMotion()
    {
        //animator.applyRootMotion = true;
    }

    public void DetectPlayer(bool flag)
    {
        if (gameObject.activeSelf)
        {
            animator.SetBool("Detect", flag);
            SetDetectedPlayer(flag);
            if (flag)
                StateMachine.ChangeState(TargetState);
            else
                StateMachine.ChangeState(IdleState); 
        }
    }
    public void Deactivate()
    {
        if(!dead)
            DetectPlayer(false);
        gameObject.SetActive(false);
    }
    public void Activate()
    {
        gameObject.SetActive(true);
    }
    public void Pause()
    {
        if (gameObject.activeSelf)
        {
            animator.speed = 0;
            rb.velocity = Vector3.zero;
            rotateTowardsPath.enabled = false;
        }
    }
    public void Resume()
    {
        if (gameObject.activeSelf)
        {
            animator.speed = 1;
            rotateTowardsPath.enabled = true;
        }
    }
    void OnDrawGizmosSelected()
    {
    }

    #region AUDIO
    public void PlayFootsteps()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.1f; // Slightly above the feet
        Vector3 direction = Vector3.down;
        float rayDistance = 1.5f;

        if (Physics.Raycast(origin, direction, out hit, rayDistance))
        {
            AudioClip chosenClip = null;

            switch (hit.collider.tag)
            {
                case "Grass":
                    chosenClip = grassFootstepClip;
                    break;
                case "Concrete":
                    chosenClip = concreteFootstepClip;
                    break;
                default:
                    chosenClip = grassFootstepClip; // Fallback
                    break;
            }

            if (chosenClip != null)
            {
                zombieAudioSource.volume = Random.Range(0.5f, 0.7f);
                zombieAudioSource.pitch = Random.Range(0.8f, 1f);
                zombieAudioSource.PlayOneShot(chosenClip);
            }
        }
    }

    public void PlayBite()
    {
        zombieAudioSource.volume = Random.Range(1f, 1.2f);
        zombieAudioSource.pitch = Random.Range(0.8f, 1f);
        zombieAudioSource.PlayOneShot(biteClip);
    }

    public void PlayGroan()
    {
        zombieAudioSource.volume = Random.Range(1f, 1.2f);
        zombieAudioSource.pitch = Random.Range(0.8f, 1f);
        zombieAudioSource.PlayOneShot(reachClips[(int)Random.Range(0,reachClips.Length)]);
    }
    #endregion
    
}

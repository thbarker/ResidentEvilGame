using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerDamage : Damageable
{
    #region References
    private Rigidbody rb;
    private GameObject bitingZombie;
    public Animator animator; 
    public Transform zombieTransform;
    [SerializeField]
    private PlayerMovement playerMovement;
    public Image image;
    public TextMeshProUGUI deathText;
    #endregion

    #region Helpers
    private bool isBeingBitten = false;
    private float currentLerpTime = 0.0f;
    public bool dead = false;
    private float deathCanvasStartTime;
    #endregion

    #region Tunables
    [SerializeField]
    [Tooltip("Time in seconds to complete the lerp to zombie when bitten")]
    private float lerpTime = 1f;
    [SerializeField]
    [Tooltip("Amount of force used to push away zombies after a bite")]
    private float pushForce = 5f;
    [SerializeField]
    [Tooltip("How long does the bite last in seconds")]
    private float biteDuration = 3f;
    [SerializeField]
    [Tooltip("Range used to push away zombies after a bite")]
    private float pushRadius = 2f;
    [SerializeField]
    [Tooltip("Maximum Player Health")]
    [Range(0, 500)]
    public int maxHealth = 100;
    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        health = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    { 
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            ApplyDamage(10);
        }
    }

    public void GetBit(GameObject zombie, Transform zombieTransform)
    {
        bitingZombie = zombie;
        this.zombieTransform = zombieTransform;
        playerMovement.StateMachine.ChangeState(playerMovement.BitState);
    }

    public float GetPushForce()
    {
        return pushForce;
    }

    public void PushBack()
    {
        // Prepare the origin of the overlap circle to the center of the player gameobject
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        // Get all colliders within the detectionRadius that match the layer mask
        Collider[] hitColliders = Physics.OverlapSphere(origin, pushRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy")) // Make sure the collider has the Enemy tag
            {
                // Attempt to get the Pushable script attached to the collider
                Pushable pushable = hitCollider.GetComponent<Pushable>();
                if (pushable != null)
                {
                    // Call the PushBack function on the Pushable script
                    pushable.PushBack();
                }
            }
        }
    }
    public bool GetIsBeingBitten()
    {
        return isBeingBitten;
    }
    public void SetIsBeingBitten(bool flag)
    {
        isBeingBitten = flag;
    }

    public GameObject GetBitingZombie()
    {
        return bitingZombie;
    }

    public void ResetBitingZombie()
    {
        bitingZombie = null;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }
    public float GetLerpTime()
    {
        return lerpTime;
    }
    public float GetBiteDuration()
    {
        return biteDuration;
    }
    public float GetPushRadius()
    {
        return pushRadius;
    }

    public override void ApplyDamage(int damage)
    {
        health -= damage;
    }

    public void Heal(int damage)
    {
        health += damage;
        if(health > maxHealth) 
            health = maxHealth;
    }

    protected override void Die()
    {
        StartCoroutine(StartDeathCanvas());
        if(!isBeingBitten)
        {
            playerMovement.StateMachine.ChangeState(playerMovement.DieState);
        }
        dead = true;
    }

    IEnumerator StartDeathCanvas(){
        yield return new WaitForSeconds(8);
        deathCanvasStartTime = Time.time;
        while(Time.time - deathCanvasStartTime < 2f){
            float alpha = (Time.time - deathCanvasStartTime) / 2f;
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, alpha));
            deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, Mathf.Lerp(0, 1, alpha));
            yield return null;
        }
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Level_1");
    }
}

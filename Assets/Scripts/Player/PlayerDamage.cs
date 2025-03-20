using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    Animator animator;
    private bool isLerping = false;
    private bool isBeingBitten = false;
    private Transform biteTransform;
    private Transform zombieTransform;
    [SerializeField] 
    private float lerpTime = 1f;  // Time in seconds to complete the lerp
    [SerializeField]
    private float pushForce = 5f;  // Time in seconds to complete the lerp
    [SerializeField]
    private PlayerMovement movementScript;
    [SerializeField]
    private float biteDuration = 3f;
    [SerializeField]
    private float pushRadius = 2f;
    private float currentLerpTime = 0.0f;
    private Rigidbody rb;
    private GameObject bitingZombie;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLerping)
        {
            // Increment the lerp time
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            // Calculate the lerp time 
            float t = currentLerpTime / lerpTime;

            // Lerp the rotation to face the zombie
            Quaternion targetRotation = Quaternion.LookRotation(zombieTransform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);

            // Reset the lerp if it completes
            if (currentLerpTime >= lerpTime)
            {
                isLerping = false;  // Stops the lerp process
                currentLerpTime = 0.0f;  // Reset the lerp timer for the next time
            }
        
        }
    }

    public void GetBit(GameObject zombie, Transform bitePosition, Transform zombiePosition)
    {
        bitingZombie = zombie;
        StartCoroutine(BiteSequence(bitePosition, zombiePosition));
    }

    private IEnumerator BiteSequence(Transform bitePosition, Transform zombiePosition)
    {
        rb.velocity = Vector3.zero;
        movementScript.SetInputEnabled(false);
        animator.SetTrigger("GetBit");
        animator.SetBool("GettingBit", true);
        isLerping = true;
        isBeingBitten = true;
        biteTransform = bitePosition;
        zombieTransform = zombiePosition;
        currentLerpTime = 0.0f; // Reset the lerp time
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger("GetBit");
        yield return new WaitForSeconds(biteDuration - 0.5f);
        animator.SetBool("GettingBit", false);
        isBeingBitten = false;
        movementScript.SetInputEnabled(true);
        bitingZombie = null;
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

    public GameObject GetBitingZombie()
    {
        return bitingZombie;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }
}

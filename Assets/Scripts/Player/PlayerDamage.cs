using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    Animator animator; 
    private bool isBeingBitten = false;
    public Transform zombieTransform;
    [SerializeField] 
    private float lerpTime = 1f;  // Time in seconds to complete the lerp
    [SerializeField]
    private float pushForce = 5f;  // Time in seconds to complete the lerp
    [SerializeField]
    private PlayerMovement playerMovement;
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
        playerMovement = GetComponent<PlayerMovement>();
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
}

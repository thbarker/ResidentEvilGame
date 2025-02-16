using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    Animator animator;
    private bool isBeingBitten = false;
    private Transform biteTransform;
    private Transform zombieTransform;
    [SerializeField] 
    private float lerpTime = 0.25f;  // Time in seconds to complete the lerp
    [SerializeField]
    private PlayerMovement movementScript;
    [SerializeField]
    private float biteDuration = 3f;
    private float currentLerpTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingBitten)
        {
            // Increment the lerp time
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime)
            {
                currentLerpTime = lerpTime;
            }

            // Calculate the lerp percentage
            float perc = currentLerpTime / lerpTime;

            // Lerp the position
            transform.position = Vector3.Lerp(transform.position, biteTransform.position, perc);

            // Lerp the rotation to face the zombie
            Quaternion targetRotation = Quaternion.LookRotation(zombieTransform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, perc);

            // Reset the lerp if it completes
            if (currentLerpTime >= lerpTime)
            {
                isBeingBitten = false;  // Stops the lerp process
                currentLerpTime = 0.0f;  // Reset the lerp timer for the next time
            }
        }
    }

    public void GetBit(Transform bitePosition, Transform zombiePosition)
    {
        StartCoroutine(BiteSequence(bitePosition, zombiePosition));
    }

    private IEnumerator BiteSequence(Transform bitePosition, Transform zombiePosition)
    {
        animator.SetTrigger("GetBit");
        animator.SetBool("GettingBit", true);
        isBeingBitten = true;
        biteTransform = bitePosition;
        zombieTransform = zombiePosition;
        currentLerpTime = 0.0f;  // Reset the lerp time
        yield return null;
        animator.ResetTrigger("GetBit");
        yield return new WaitForSeconds(biteDuration);
        animator.SetBool("GettingBit", false);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private PlayerControls controls;
    private Animator animator;
    [SerializeField]
    [Range(0.01f, 2f)]
    private float fireRate;
    private bool canAttack = true;
    private bool isAttacking = false;
    private void Awake()
    {
        // Get reference to player controls
        controls = PlayerInputManager.controls;

        controls.Player.Attack.performed += ctx =>
        {
            isAttacking = true;
        };
        controls.Player.Attack.canceled += ctx =>
        {
            isAttacking = false;
        };

    }
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        UpdateAttack();
        UpdateAnimationLayer();
    }

    private IEnumerator Attack()
    {
        animator.SetTrigger("Attack");
        canAttack = false;
        yield return new WaitForSeconds(fireRate);
        canAttack = true;
    }

    private void UpdateAttack()
    {
        if(canAttack && isAttacking)
        {
            StartCoroutine(Attack());
        }
    }
    private void UpdateAnimationLayer()
    {

    }
}

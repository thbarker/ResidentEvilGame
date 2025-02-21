using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Animator))]
public class PlayerShoot : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerMovement playerMovement;
    private Animator animator;
    [SerializeField]
    [Range(0.01f, 2f)]
    private float fireRate;
    private bool canAttack = false;
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
            animator.ResetTrigger("Attack");
            isAttacking = false;
        };

    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
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
        animator.SetFloat("AttackSpeed", (1 / fireRate));
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Aiming"))
        {
            canAttack = true;
        } else
        {
            canAttack = false;
        }
    }
}

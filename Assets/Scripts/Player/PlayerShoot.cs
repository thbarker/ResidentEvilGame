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
    private ZombieList zombieList;
    private Animator animator;
    [SerializeField]
    [Range(0.01f, 2f)]
    public float fireRate;
    [SerializeField]
    [Range(0, 50)]
    private int damage = 10;
    public Transform shootingPoint;
    public bool canAttack = false;
    private bool isAttacking = false;
    private bool attackCooldown = false;
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
        zombieList = transform.Find("ZombieList").GetComponent<ZombieList>();
    }

    private IEnumerator Attack()
    {
        if (!canAttack || attackCooldown)
            yield break;
        animator.SetTrigger("Attack");
        attackCooldown = true;

        // Become Detected by all Zombies
        zombieList.DetectedByAll();

        // Perform raycast
        RaycastHit hit;
        Vector3 rayOrigin = shootingPoint.position; // This might need to be adjusted based on your character
        Vector3 rayDirection = shootingPoint.forward; // Assumes the character shoots forward
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, 100f))
        {
            // Debug to visualize the ray in the scene view
            Debug.DrawRay(rayOrigin, rayDirection * 100f, Color.red, 2f);

            // Check if the hit object has a Damageable component
            Damageable damageable = hit.collider.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage); // Apply damage
            }
        }

        yield return new WaitForSeconds(fireRate);
        attackCooldown = false;
    }

    public void UpdateAttack()
    {
        if(canAttack && isAttacking)
        {
            StartCoroutine(Attack());
        }
    }
}

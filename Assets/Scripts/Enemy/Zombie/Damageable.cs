using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    public abstract void ApplyDamage(float damage);

    [SerializeField]
    [Range(0f, 500f)]
    protected float health;

    public void TakeDamage(float damage)
    {
        ApplyDamage(damage);
        if (health <= 0)
            Die();
    }

    protected virtual void Die() {}
}

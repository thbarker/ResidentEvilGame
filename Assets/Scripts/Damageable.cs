using UnityEngine;

public abstract class Damageable : MonoBehaviour
{
    public abstract void ApplyDamage(int damage);

    [SerializeField]
    [Range(0, 9999)]
    protected int health;

    public void TakeDamage(int damage)
    {
        ApplyDamage(damage);
        if (health <= 0)
            Die();
    }

    protected virtual void Die() {}

    public int GetHealth()
    {
        return health;
    }
}

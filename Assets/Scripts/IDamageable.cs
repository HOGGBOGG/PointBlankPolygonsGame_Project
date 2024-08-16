using UnityEngine;

public interface IDamageable
{
    public float CurrentHealth { get; }
    public float MaxHealth { get; }

    public delegate void TakeDamageEvent(float Damage);
    public event TakeDamageEvent OnTakeDamage;

    public delegate void DeathEvent(Vector3 Position);
    public event DeathEvent OnDeath;

    public void TakeDamage(float Damage);
}
using Unity.VisualScripting;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float _Health;
    [SerializeField]
    private float _MaxHealth = 100;
    public float CurrentHealth { get => _Health; private set => _Health = value; }
    public float MaxHealth { get => _MaxHealth; set => _MaxHealth = value; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    public delegate void DecreaseCounter();
    public DecreaseCounter DecreaseAliveEnemies;

    public GameObject ParentObjectToDisable;

    private void OnEnable()
    {
        _Health = MaxHealth;
        OnTakeDamage?.Invoke(0);//To reinitialise the healthbar
    }

    public void IncreaseCurrentPlayerHP(float amount)
    {
        float valueAfter = CurrentHealth + amount;
        if(valueAfter >= MaxHealth) 
        {
            CurrentHealth = MaxHealth;
        }
        else
        {
            CurrentHealth += amount;
            OnTakeDamage?.Invoke(0);
        }
    }

    public void SetCurrentToMax()
    {
        CurrentHealth = MaxHealth;
    }

    public void InvokeOnTakeDamage()
    {
        OnTakeDamage?.Invoke(CurrentHealth);
    }

    public void TakeDamage(float Damage)
    {
        float damageTaken = Mathf.Clamp(Damage, 0, CurrentHealth);

        CurrentHealth -= damageTaken;

        if (damageTaken != 0)
        {
            OnTakeDamage?.Invoke(damageTaken);
        }

        if (CurrentHealth == 0 && damageTaken != 0)
        {
            DecreaseAliveEnemies?.Invoke();
            ParentObjectToDisable.SetActive(false);
            OnDeath?.Invoke(transform.position);
        }
    }

}
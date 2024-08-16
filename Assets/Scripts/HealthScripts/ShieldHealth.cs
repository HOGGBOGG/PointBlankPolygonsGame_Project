using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHealth : MonoBehaviour,IDamageable
{
    public Transform ScaleTransform;
    [Header("If using SphereCollider")]
    public SphereCollider SphereCollider;
    [Header("If using BoxCollider")]
    public bool useBoxCollider = false;
    public BoxCollider BoxCollider;
    public MeshRenderer meshRenderer;

    public float minLimit;
    public float maxLimit;
    public float regenSpeed = 10f;
    public float CurrentHealth { get; set; }
    public float MaxHealth { get; set; }

    public event IDamageable.TakeDamageEvent OnTakeDamage;
    public event IDamageable.DeathEvent OnDeath;

    public float currHp = 1000;
    public float maxHp = 1000;

    private void Update()
    {
        CurrentHealth += Time.deltaTime * regenSpeed / 4f;
        CurrentHealth = Mathf.Clamp(CurrentHealth, CurrentHealth, MaxHealth);
        VaryShieldSize(2f);
    }

    public void TakeDamage(float Damage)
    {
        float damageTaken = Mathf.Clamp(Damage, 0, CurrentHealth);

        CurrentHealth -= damageTaken;
        currHp -= damageTaken;

        if (damageTaken != 0)
        {
            OnTakeDamage?.Invoke(damageTaken);
        }

        if (CurrentHealth == 0 && damageTaken != 0)
        {
            OnDeath?.Invoke(transform.position);
            if (!useBoxCollider)
                SphereCollider.enabled = false;
            else BoxCollider.enabled = false;
            meshRenderer.enabled = false;
            StartCoroutine(ShieldHeal());
        }
    }

    private void VaryShieldSize(float arb)
    {
        float t = 1f - CurrentHealth / MaxHealth;
        float scale = Mathf.Lerp(maxLimit, minLimit, t);
        ScaleTransform.localScale = Vector3.one * scale;
        currHp = CurrentHealth;
    }

    private IEnumerator ShieldHeal()
    {
        float AmountToHeal = 950f;
        while(AmountToHeal >= 0f)
        {
            float hpRegen = Time.deltaTime * regenSpeed;
            CurrentHealth += hpRegen;
            AmountToHeal -= hpRegen;
            VaryShieldSize(hpRegen);
            yield return null;
        }
        float time = 0f;
        meshRenderer.enabled = true;
        while(time < 1f) 
        {
            ScaleTransform.localScale = Vector3.one * Mathf.Lerp(minLimit, maxLimit,time);
            time += Time.deltaTime / 3f;
            yield return null;
        }
        if (!useBoxCollider)
            SphereCollider.enabled = true;
        else BoxCollider.enabled = true;
    }


    void OnEnable()
    {
        MaxHealth = maxHp;
        currHp = maxHp;
        CurrentHealth = maxHp;
        OnTakeDamage += VaryShieldSize;
    }

}

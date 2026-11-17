using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Actions
    public Action<float> OnTakeDamage;
    public Action<float, GameObject> OnHurtBySource;
    public Action OnDeath;

    // Config
    public float MaxHealth;

    // Vars
    public float CurrentHealth;
    public bool IsIntangible;
    public float Percent => Mathf.Clamp01(CurrentHealth / MaxHealth);

    // Refs
    public HealthBar healthBar;

    public void Start()
    {
        healthBar.Init(CurrentHealth / MaxHealth);
    }

    public void TakeDamage(float amount, GameObject source = null)
    {
        if (amount == 0.0f || amount > 0.0f && IsIntangible) return;

        float actualDamageTaken = Mathf.Clamp(amount, 0, CurrentHealth);
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        OnTakeDamage?.Invoke(amount);
        if (amount > 0.0f && source != null) OnHurtBySource?.Invoke(amount, source);
        if (healthBar != null) healthBar.SetHealth(Percent);
        
        if (source != null)
        {
            if (source.TryGetComponent(out MeleeEffect meleeEffect))
            {
                if (meleeEffect.Source != null && meleeEffect.Source.TryGetComponent(out Health health))
                {
                    health.TakeDamage(-actualDamageTaken * meleeEffect.LifestealFactor);
                }
            }
            if (source.TryGetComponent(out Projectile projectile))
            {
                if (projectile.Source != null && projectile.Source.TryGetComponent(out Health health))
                {
                    health.TakeDamage(-actualDamageTaken * projectile.LifestealFactor);
                }
            }
        }

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void SetIntangible(bool invulerable)
    {
        IsIntangible = invulerable;
    }
}

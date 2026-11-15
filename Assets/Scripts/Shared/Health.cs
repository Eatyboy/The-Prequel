using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    // Actions
    public Action<float> OnTakeDamage;
    public Action OnDeath;

    // Config
    public float MaxHealth;

    // Vars
    public float CurrentHealth;
    public bool IsIntangible;

    // Refs
    [SerializeField] private HealthBar healthBar;

    public void TakeDamage(float amount, GameObject source = null)
    {
        if (amount == 0.0f || amount > 0.0f && IsIntangible) return;

        float actualDamageTaken = Mathf.Clamp(amount, 0, CurrentHealth);
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        OnTakeDamage?.Invoke(amount);
        if (healthBar != null) healthBar.SetHealth(Mathf.Max(CurrentHealth, 0) / MaxHealth);
        
        if (source != null)
        {
            if (source.TryGetComponent(out MeleeEffect meleeEffect))
            {
                if (meleeEffect.Source.TryGetComponent(out Health health))
                {
                    health.TakeDamage(-actualDamageTaken * meleeEffect.LifestealFactor);
                }
            }
            if (source.TryGetComponent(out Projectile projectile))
            {
                if (projectile.Source.TryGetComponent(out Health health))
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
        GetComponentInChildren<SpriteRenderer>().color = invulerable ? new Color(1f, 1f, 1f, 0.1f) : Color.white;
    }
}

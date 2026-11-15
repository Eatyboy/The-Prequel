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
        if (amount == 0.0f || IsIntangible) return;

        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        OnTakeDamage?.Invoke(amount);
        if (healthBar != null) healthBar.SetHealth(Mathf.Max(CurrentHealth, 0) / MaxHealth);
        // Source knows it dealt damage, do something like lifesteal
        //if (source != null && source.TryGetComponent(out DamageSource damageSource))
        //{
        //    damageSource.
        //}

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

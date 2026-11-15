using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action<float> OnTakeDamage;
    public Action OnDeath;
    [SerializeField] private HealthBar healthBar;

    public float CurrentHealth;
    public float MaxHealth;

    public void TakeDamage(float amount, GameObject source = null)
    {
        if (amount == 0.0f) return;

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
}

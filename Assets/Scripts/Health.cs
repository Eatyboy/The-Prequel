using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action<float> OnTakeDamage;
    public Action OnDeath;

    public float CurrentHealth;
    public float MaxHealth;

    public void TakeDamage(float amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth - amount, 0, MaxHealth);
        OnTakeDamage?.Invoke(amount);

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }
}

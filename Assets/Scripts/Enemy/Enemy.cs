using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Start()
    {
        health.OnDeath += Die;
    }
    
    private void Die()
    {
        Destroy(gameObject);
    }
}

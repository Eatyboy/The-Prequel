using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Singleton<Player>
{
    private Health health;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
        health = GetComponent<Health>();
    }

    private void Start()
    {
        health.OnDeath += () => { Debug.Log("Die"); };
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            health.TakeDamage(10);
        }
    }
}

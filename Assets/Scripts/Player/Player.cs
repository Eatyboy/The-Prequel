using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Singleton<Player>
{
    public Vector2 LookDir;

    private Health health;

    // Actions
    private InputAction look;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
        health = GetComponent<Health>();

        look = InputSystem.actions.FindAction("Look");
    }

    private void Start()
    {
        health.OnDeath += () => { Debug.Log("Die"); };
    }

    private void Update()
    {
        LookDir = (look.ReadValue<Vector2>() - (Vector2)transform.position).normalized;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            health.TakeDamage(10);
        }
    }
}

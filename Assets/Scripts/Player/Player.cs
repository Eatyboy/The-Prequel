using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Singleton<Player>
{
    public Vector2 LookPos;
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
        LookPos = Camera.main.ScreenToWorldPoint(look.ReadValue<Vector2>());
        LookDir = (LookPos - (Vector2)transform.position).normalized;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            health.TakeDamage(10);
        }
    }
}

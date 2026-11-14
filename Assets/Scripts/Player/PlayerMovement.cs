using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerAbilities)), RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CircleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Config")]
    public float speed;

    // Vars
    private Vector2 moveDir;

    // Refs
    private InputAction moveAction;
    private CircleCollider2D col;
    private Rigidbody2D rb;
    private PlayerAbilities abilities;

    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask foregroundLayerMask;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 3.0f;
    [SerializeField] private float dashDuration = 0.5f;
    private float dashSpeed => dashDistance / dashDuration;
    [SerializeField] private float dashDamage = 2.0f;

    public bool isDashing = false;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        abilities = GetComponent<PlayerAbilities>();
    }

    private void Update()
    {
        moveDir = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!isDashing)
        {
            rb.linearVelocity = moveDir * speed;
        }
    }

    public async void Dash()
    {
        isDashing = true;
        float dashTimer = 0.0f;
        Vector2 initialDirection = moveDir;
        rb.linearVelocity = dashSpeed * initialDirection;
        rb.excludeLayers |= enemyLayerMask | foregroundLayerMask;
        HashSet<Collider2D> enemiesHit = new();

        while (dashTimer < dashDuration)
        {
            dashTimer += Time.fixedDeltaTime;

            var enemyColliders = Physics2D.OverlapCircleAll(transform.position, col.radius, enemyLayerMask);
            foreach (var enemyCol in enemyColliders)
            {
                if (enemiesHit.Contains(enemyCol)) continue;

                var enemy = enemyCol.GetComponent<Enemy>();
                enemy.health.TakeDamage(dashDamage);
                enemiesHit.Add(enemyCol);
            }

            await UniTask.WaitForFixedUpdate(cancellationToken: default);
        }
        
        rb.excludeLayers &= ~(enemyLayerMask | foregroundLayerMask);
        isDashing = false;
    }
}

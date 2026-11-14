using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Health health;

    [Header("Refs")]
    [SerializeField] private Projectile bullet;
    public EnemyMovement movement;

    [Header("Config")]
    [SerializeField] private float attackCooldown = 1.0f;
    private float attackTimer = 0.0f;

    private void Awake()
    {
        health = GetComponent<Health>();
        movement = GetComponent<EnemyMovement>();
    }

    private void Start()
    {
        health.OnDeath += Die;
    }

    private void Update()
    {
        if (movement.distanceToTarget <= movement.stopDistance)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                var newBullet = Instantiate(bullet);
                newBullet.Init(transform.position, (movement.Target.position - transform.position).normalized, gameObject, "Enemy");
                attackTimer = 0.0f;
            }
        } else
        {
            attackTimer = 0.0f;
        }
    }

    private void Die()
    {
        Player.Instance.onKill.Invoke();
        Destroy(gameObject);
    }
}

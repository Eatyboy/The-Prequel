using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Health health;

    public bool IsStunned;

    private float stunCountdown;

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

        health.OnTakeDamage += amount => OnTakeDamage(amount);
    }

    private void Start()
    {
        health.OnDeath += Die;
    }

    private void Update()
    {
        if (IsStunned)
        {
            stunCountdown = Mathf.Max(stunCountdown - Time.deltaTime, 0.0f);
            if (stunCountdown <= 0.0f)
            {
                IsStunned = false;
                movement.LockMovement(false);
            }
        }
        else
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
            }
            else
            {
                attackTimer = 0.0f;
            }
        }
    }

    public void Stun(float duration)
    {
        if (duration == 0.0f) return;

        IsStunned = true;
        stunCountdown = duration;
        movement.LockMovement(true);
    }

    private void Die()
    {
        Player.Instance.onKill.Invoke();
        Destroy(gameObject);
    }

    private void OnTakeDamage(float amount)
    {
        PopupManager.Instance.SpawnDamagePopup(transform.position, amount);
    }
}

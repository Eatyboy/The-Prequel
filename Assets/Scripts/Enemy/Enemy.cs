using Cysharp.Threading.Tasks;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Health health;

    public bool IsStunned;
    public bool IsDead;

    private float stunCountdown;

    [Header("Refs")]
    [SerializeField] private Projectile bullet;
    public EnemyMovement movement;
    private EnemyAnimation animation;

    [Header("Config")]
    [SerializeField] private float attackCooldown = 1.0f;
    private float attackTimer = 0.0f;

    private void Awake()
    {
        health = GetComponent<Health>();
        movement = GetComponent<EnemyMovement>();
        animation = GetComponent<EnemyAnimation>();

        health.OnTakeDamage += OnTakeDamage;
    }

    private void Start()
    {
        health.OnDeath += Die;
    }

    private void Update()
    {
        if (IsDead) return;
        health.healthBar.transform.position = (Vector2)transform.position + EnemyManager.Instance.enemyHealthBarOffset;
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

                    animation.TriggerAnimation("Attack");
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

    private async void Die()
    {
        IsDead = true;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<EnemyMovement>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
        Player.Instance.onKill.Invoke();
        Destroy(health.healthBar.gameObject);
        animation.TriggerAnimation("Death", false);
        await UniTask.WaitForSeconds(10.0f);
        Destroy(gameObject);
    }

    private void OnTakeDamage(float amount)
    {
        PopupManager.Instance.SpawnDamagePopup(transform.position, amount);
    }
}

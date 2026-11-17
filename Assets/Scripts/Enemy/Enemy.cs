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
    [SerializeField] private MeleeEffect meleeEffect;
    public EnemyMovement movement;
    private EnemyAnimation animation;
    [SerializeField] private Transform projPoint;

    [Header("Config")]
    [SerializeField] private float attackCooldown = 1.0f;
    public bool IsMelee;
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
            Attack();
        }
    }

    public void Attack()
    {
        if (movement.distanceToTarget <= movement.stopDistance)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                Vector2 playerDirection = (movement.Target.position - transform.position).normalized;
                if (IsMelee)
                {
                    Melee(playerDirection);
                }
                else
                {
                    Shoot(playerDirection);
                }
            }
        }
        else
        {
            attackTimer = 0.0f;
        }
    }

    public void Shoot(Vector2 dir)
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.crossbowFire);
        var newBullet = Instantiate(bullet);
        newBullet.Init(projPoint.position, dir, gameObject, "Enemy");
        attackTimer = 0.0f;

        animation.TriggerAnimation("Attack");
    }

    public void Melee(Vector2 dir)
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.chomp);
        var newMelee = Instantiate(meleeEffect, transform);
        newMelee.Init(projPoint.position + (Vector3)dir, dir, gameObject, "Enemy");
        attackTimer = 0.0f;
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
        GetComponentInChildren<SpriteRenderer>().sortingOrder = -2;
        Player.Instance.onKill.Invoke();
        Destroy(health.healthBar.gameObject);
        animation.TriggerAnimation("Death", false);
        try
        {
            await UniTask.WaitForSeconds(10.0f, cancellationToken: this.GetCancellationTokenOnDestroy());
            Destroy(gameObject);
        }
        catch
        {

        }
    }

    private void OnTakeDamage(float amount)
    {
        PopupManager.Instance.SpawnDamagePopup(transform.position, amount);
        if (amount > 0.0f) AudioManager.instance.PlayOneShot(AudioManager.instance.enemyHit);
    }
}

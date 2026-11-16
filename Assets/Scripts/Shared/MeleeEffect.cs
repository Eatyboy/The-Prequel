using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class MeleeEffect : MonoBehaviour
{
    // Config
    public float Damage;
    public float LifestealFactor;
    public GameObject Source;
    public string IgnoreTag;
    public float BloodCost;

    public float StunDuration;
    public float RotationSpeed;
    public bool IsMultihit;
    public float MultihitInterval;
    public bool CanDestroyProjectiles;

    public float VisualDuration;
    public float HurtDuration;

    // Vars
    private float visualCountdown;
    private float hurtCountdown;
    private float multihitCountdown;

    private List<GameObject> alreadyHit;

    public void Init(Vector2 origin, Vector2 startDirection, GameObject source, string ignoreTag = "")
    {
        transform.SetPositionAndRotation(origin, Quaternion.FromToRotation(transform.right, startDirection));
        Source = source;
        IgnoreTag = ignoreTag;
    }

    private void Start()
    {
        alreadyHit = new List<GameObject>();

        visualCountdown = VisualDuration;
        hurtCountdown = HurtDuration;
        multihitCountdown = MultihitInterval;

        if (Source != null)
        {
            if (Source.TryGetComponent(out Health health))
            {
                health.TakeDamage(BloodCost);
            }
        }

        // NOTE: Very scuffed cause both are triggers, only used for death refusal
        if (CanDestroyProjectiles)
        {
            if (TryGetComponent(out CapsuleCollider2D col))
            {
                float dst = Mathf.Min(col.size.x, col.size.y);
                List<Projectile> allProjectiles = FindObjectsByType<Projectile>().ToList();

                foreach (Projectile projectile in allProjectiles)
                {
                    if (Vector3.Distance(transform.position, projectile.transform.position) < dst)
                    {
                        projectile.Die();
                    }
                }
            }
        }
    }

    private void Update()
    {
        visualCountdown -= Time.deltaTime;
        hurtCountdown -= Time.deltaTime;
        

        if (visualCountdown <= 0 && hurtCountdown <= 0)
        {
            Die();
        }

        if (IsMultihit)
        {
            multihitCountdown -= Time.deltaTime;
            if (multihitCountdown <= 0)
            {
                multihitCountdown = MultihitInterval;

                GetComponent<Collider2D>().enabled = false;
                alreadyHit.Clear();
                GetComponent<Collider2D>().enabled = true;
            }
        }
    }

    private void FixedUpdate()
    {
        float angleDiff = Vector3.SignedAngle(transform.right, Player.Instance.LookDir, Vector3.back);
        angleDiff = Mathf.Clamp(angleDiff, -RotationSpeed, RotationSpeed);
        transform.RotateAround(Player.Instance.transform.position, Vector3.back, angleDiff);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hurtCountdown <= 0)
        {
            return;
        }

        if (alreadyHit.Contains(collision.gameObject))
        {
            return;
        }

        if (!collision.transform.CompareTag(IgnoreTag))
        {
            if (collision.transform.TryGetComponent(out Health health))
            {
                if (health.IsIntangible) return;

                health.TakeDamage(Damage, gameObject);
                alreadyHit.Add(collision.gameObject);
            }
            if (collision.transform.TryGetComponent(out Enemy enemy))
            {
                enemy.Stun(StunDuration);
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}

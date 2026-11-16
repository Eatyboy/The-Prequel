using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Config
    public float Damage;
    public float LifestealFactor;
    public GameObject Source;
    public string IgnoreTag;
    public float BloodCost;

    public float Speed;
    public float Range;
    public float MaxLifetime => Range / Speed;
    public int PierceNum;
    public Vector2 StartDirection;

    // Vars
    private float currentLifetime;

    // Refs
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentLifetime = MaxLifetime;
        transform.rotation = Quaternion.FromToRotation(transform.right, StartDirection);

        if (Source != null)
        {
            if (Source.TryGetComponent(out Health health))
            {
                health.TakeDamage(BloodCost);
            }
        }
    }

    public void Init(Vector2 origin, Vector2 startDirection, GameObject source, string ignoreTag = "")
    {
        transform.position = origin;
        StartDirection = startDirection;
        Source = source;
        IgnoreTag = ignoreTag;
    }

    private void Update()
    {
        currentLifetime -= Time.deltaTime;

        if (currentLifetime <= 0)
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.right * Speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.transform.CompareTag(IgnoreTag))
        {
            if (collision.transform.TryGetComponent(out Health health))
            {
                if (health.IsIntangible) return;

                health.TakeDamage(Damage, gameObject);
                PierceNum--;

                if (PierceNum <= 0)
                {
                    Die();
                }
            }
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Config
    public float Damage;
    public float Speed;
    public GameObject Source;
    public string IgnoreTag;
    public float MaxLifetime;
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
                health.TakeDamage(Damage, Source);
                PierceNum--;

                if (PierceNum <= 0)
                {
                    Die();
                }
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}

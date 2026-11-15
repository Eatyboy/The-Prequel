using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MeleeEffect : MonoBehaviour
{
    // Config
    public float Damage;
    public GameObject Source;
    public string IgnoreTag;

    public float VisualDuration;
    public float HurtDuration;

    // Vars
    private float visualCountdown;
    private float hurtCountdown;

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
    }

    private void Update()
    {
        visualCountdown -= Time.deltaTime;
        hurtCountdown -= Time.deltaTime;

        if (visualCountdown <= 0 && hurtCountdown <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (visualCountdown <= 0)
        {
            return;
        }

        if (!collision.transform.CompareTag(IgnoreTag))
        {
            if (collision.transform.TryGetComponent(out Health health))
            {
                if (alreadyHit.Contains(collision.gameObject))
                {
                    return;
                }
                health.TakeDamage(Damage, Source);
                alreadyHit.Add(collision.gameObject);
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}

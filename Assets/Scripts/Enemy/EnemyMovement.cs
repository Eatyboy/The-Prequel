using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Config")]
    public float speed;
    public float stopDistance;

    [Header("Refs")]
    public Transform Target;

    // Vars
    private Vector2 moveDir;
    private float distanceToTarget;

    // Refs
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Target = Player.Instance.transform;
    }

    private void FixedUpdate()
    {
        if (Target != null)
        {
            Vector2 offsetToTarget = Target.position - transform.position;
            moveDir = offsetToTarget.normalized;
            distanceToTarget = offsetToTarget.magnitude;
        }
        else
        {
            moveDir = Vector2.zero;
            distanceToTarget = float.PositiveInfinity;
        }

        if (distanceToTarget > stopDistance)
        {
            rb.linearVelocity = moveDir * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}

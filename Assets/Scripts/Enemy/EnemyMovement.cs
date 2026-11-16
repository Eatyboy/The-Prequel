using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public bool IsMovementLocked;

    [Header("Config")]
    public float speed;
    public float stopDistance;

    [Header("Refs")]
    public Transform Target;

    // Vars
    public Vector2 moveDir;
    public Vector2 lookDir;
    public float distanceToTarget;

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
        if (IsMovementLocked || Target == null)
        {
            moveDir = Vector2.zero;
            distanceToTarget = float.PositiveInfinity;
        }
        else
        {
            Vector2 offsetToTarget = Target.position - transform.position;
            lookDir = offsetToTarget.normalized;
            moveDir = lookDir;
            distanceToTarget = offsetToTarget.magnitude;
        }

        if (distanceToTarget <= stopDistance)
        {
            moveDir = Vector2.zero;
        }

        rb.linearVelocity = moveDir * speed;
    }

    public void LockMovement(bool isLocked)
    {
        IsMovementLocked = isLocked;
    }
}

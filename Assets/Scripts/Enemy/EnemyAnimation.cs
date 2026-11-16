using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private string state = "Idle";
    private string direction = "Down";

    private Animator animator;
    private EnemyMovement enemyMovement;

    private bool isTriggeredAnim;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        if (Mathf.Abs(enemyMovement.lookDir.y) > Mathf.Abs(enemyMovement.lookDir.x))
        {
            if (enemyMovement.lookDir.y < 0)
            {
                direction = "Down";
            }
            else if (enemyMovement.lookDir.y > 0)
            {
                direction = "Up";
            }
        }
        else
        {
            if (enemyMovement.lookDir.x > 0)
            {
                direction = "Right";
            }
            else if (enemyMovement.lookDir.x < 0)
            {
                direction = "Left";
            }
        }

        if (isTriggeredAnim)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                isTriggeredAnim = false;
            }
        }
        else
        {
            if (enemyMovement.moveDir != Vector2.zero)
            {
                state = "Walk";
            }
            else
            {
                state = "Idle";
            }

            animator.Play($"{state}{direction}");
        }
    }

    public void TriggerAnimation(string name, bool directional = true)
    {
        animator.Play($"{name}{(directional ? direction : "")}");
        Debug.Log($"{name}{(directional ? direction : "")}");
        isTriggeredAnim = true;
    }
}

using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private string state = "Idle";
    private string direction = "Down";

    private Animator animator;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement.moveDir != Vector2.zero)
        {
            state = "Walk";

            if (playerMovement.moveDir.y < 0)
            {
                direction = "Down";
            }
            else if (playerMovement.moveDir.y > 0)
            {
                direction = "Up";
            }
            else if (playerMovement.moveDir.x > 0)
            {
                direction = "Right";
            }
            else if (playerMovement.moveDir.x < 0)
            {
                direction = "Left";
            }
        }
        else
        {
            state = "Idle";
        }

        animator.Play($"{state}{direction}");
    }
}

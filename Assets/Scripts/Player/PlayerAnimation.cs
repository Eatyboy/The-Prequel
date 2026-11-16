using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private string state = "Idle";
    private string direction = "Down";

    public Animator animator;
    private PlayerMovement playerMovement;

    private bool isTriggeredAnim;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
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

        if (playerMovement.moveDir != Vector2.zero)
        {
            state = "Walk";
        }
        else
        {
            state = "Idle";
        }

        if (!isTriggeredAnim)
        {
            animator.Play($"{state}{direction}");
        }
    }

    public void TriggerAnimation(string name, bool directional = true)
    {
        animator.Play($"{name}{(directional ? direction : "")}");
        isTriggeredAnim = true;
    }

    public void EndTriggerAnimation()
    {
        isTriggeredAnim = false;
    }
}

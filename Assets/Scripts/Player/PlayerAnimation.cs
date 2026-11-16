using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private string state = "Idle";
    private string direction = "Down";
    private string lookDirection = "Down";

    public Animator animator;
    private PlayerMovement playerMovement;
    private Player player;

    private bool isTriggeredAnim;
    public bool isDirectionLocked;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (!isDirectionLocked) UpdateDirections();

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

    private void UpdateDirections()
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

        if (Mathf.Abs(player.LookDir.y) > Mathf.Abs(player.LookDir.x))
        {
            if (player.LookDir.y < 0)
            {
                lookDirection = "Down";
            }
            else if (player.LookDir.y > 0)
            {
                lookDirection = "Up";
            }
        }
        else
        {
            if (player.LookDir.x > 0)
            {
                lookDirection = "Right";
            }
            else if (player.LookDir.x < 0)
            {
                lookDirection = "Left";
            }
        }
    }

    public void LockDirection(bool isLocked)
    {
        isDirectionLocked = isLocked;
    }

    public void TriggerAnimation(string name, bool directional = true, bool useLookDir = false)
    {
        animator.Play($"{name}{(directional ? (useLookDir ? lookDirection : direction) : "")}");
        isTriggeredAnim = true;
    }

    public void EndTriggerAnimation()
    {
        isTriggeredAnim = false;
    }
}

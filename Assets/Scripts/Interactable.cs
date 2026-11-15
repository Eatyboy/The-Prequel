using System;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Action OnInteract;
    public bool isInteractable = true;
    public bool IsDisabled;

    private const float DEFAULT_INTERACT_RADIUS = 1.0f;

    private void Awake()
    {
        if (!TryGetComponent<Collider2D>(out _))
        {
            var newCollider = gameObject.AddComponent<CircleCollider2D>();
            newCollider.radius = DEFAULT_INTERACT_RADIUS;
            newCollider.isTrigger = true;
        }
    }

    public void Interact()
    {
        if (isInteractable) OnInteract.Invoke();
    }

    public void Enable(bool isEnabled)
    {
        // Note: only supports disabling currently
        IsDisabled = !isEnabled;
        if (IsDisabled)
        {
            Player.Instance.interactables.Remove(this);
            if (Player.Instance.interactables.Count > 0)
            {
                HeadsUpDisplay.Instance.Interact(Player.Instance.interactables[^1].transform);
            }
            else
            {
                HeadsUpDisplay.Instance.Interact(null);
            }
        }
    }
}

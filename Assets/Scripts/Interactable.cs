using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Action OnInteract;
    public bool isInteractable = true;
    public bool IsDisabled;

    private const float DEFAULT_INTERACT_RADIUS = 1.0f;

    private void Awake()
    {
        gameObject.layer = InteractionZone.InteractionLayer;

        if (!TryGetComponent<Collider2D>(out var col))
        {
            var circle = gameObject.AddComponent<CircleCollider2D>();
            circle.radius = DEFAULT_INTERACT_RADIUS;
            col = circle;
        }
        col.isTrigger = true;
    }

    public void Interact()
    {
        if (isInteractable && !IsDisabled) OnInteract?.Invoke();
    }

    public void Enable(bool isEnabled)
    {
        // Note: only supports disabling currently
        IsDisabled = !isEnabled;
        if (IsDisabled)
        {
            Player.Instance.RemoveInteractable(this);
        }
    }
}

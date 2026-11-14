using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Action OnInteract;
    public bool isInteractable = true;

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
}

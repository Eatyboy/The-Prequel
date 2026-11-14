using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerAbility
{
    None,
    Projectile,
    Dash,
    BatSwarm,
    Revive,
    Stun,
}

public class Player : Singleton<Player>
{
    private Health health;
    private List<Interactable> interactables = new();

    private InputAction interactAction;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
        health = GetComponent<Health>();

        interactAction = InputSystem.actions.FindAction("Interact");
    }

    private void Start()
    {
        health.OnDeath += () => { Debug.Log("Die"); };
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            health.TakeDamage(10);
        }

        if (interactAction.WasPressedThisFrame()) Interact();
    }

    private void Interact()
    {
        if (interactables.Count > 0)
        {
            interactables[^1].OnInteract?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            interactables.Add(collision.GetComponent<Interactable>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            interactables.Remove(collision.GetComponent<Interactable>());
        }
    }
}

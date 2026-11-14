using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Flags]
public enum PlayerAbility
{
    None        = 0,
    Projectile  = 1 << 0,
    Dash        = 1 << 1,
    BatSwarm    = 1 << 2,
    Revive      = 1 << 3,
    Stun        = 1 << 4,
}

public class Player : Singleton<Player>
{
    public Vector2 LookPos;
    public Vector2 LookDir;

    public Health health;
    public PlayerAbilities abilities;
    public PlayerMovement movement;
    private List<Interactable> interactables = new();

    private InputAction interactAction;

    // Actions
    private InputAction look;
    public Action onKill;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
        health = GetComponent<Health>();
        abilities = GetComponent<PlayerAbilities>();
        movement = GetComponent<PlayerMovement>();

        interactAction = InputSystem.actions.FindAction("Interact");
        look = InputSystem.actions.FindAction("Look");
    }

    private void Start()
    {
        health.OnDeath += () => { Debug.Log("Die"); };
    }

    private void Update()
    {
        LookPos = Camera.main.ScreenToWorldPoint(look.ReadValue<Vector2>());
        LookDir = (LookPos - (Vector2)transform.position).normalized;

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

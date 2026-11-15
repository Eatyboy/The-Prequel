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
    Claws       = 1 << 5,
    DeathRefusal= 1 << 6,
}

public class Player : Singleton<Player>
{
    [HideInInspector] public Vector2 LookPos;
    [HideInInspector] public Vector2 LookDir;

    [HideInInspector] public Health health;
    [HideInInspector] public PlayerAbilities abilities;
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public List<Interactable> interactables = new();

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
        health.OnDeath += PlayerDeath;
    }

    private void Update()
    {
        LookPos = Camera.main.ScreenToWorldPoint(look.ReadValue<Vector2>());
        LookDir = (LookPos - (Vector2)transform.position).normalized;

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
            HeadsUpDisplay.Instance.Interact(interactables[^1].transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            interactables.Remove(collision.GetComponent<Interactable>());
            if (interactables.Count > 0)
            {
                HeadsUpDisplay.Instance.Interact(interactables[^1].transform);
            }
            else
            {
                HeadsUpDisplay.Instance.Interact(null);
            }
        }
    }

    private void PlayerDeath()
    {
        if (abilities.CanUse(PlayerAbility.DeathRefusal))
        {
            abilities.abilities[PlayerAbility.DeathRefusal].GoOnCooldown();
            abilities.DeathRefusal();
        }
        else
        {
            Debug.Log("Die frfr");
        }
    }
}

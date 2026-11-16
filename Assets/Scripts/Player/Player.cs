using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    private PlayerAnimation anim;

    public bool IsInteractLocked;
    public bool IsDead;

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
        anim = GetComponent<PlayerAnimation>();

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
        if (IsInteractLocked) return;
        if (interactables.Count > 0)
        {
            interactables[^1].OnInteract?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            if (interactables.Contains(collision.GetComponent<Interactable>())) return;
            interactables.Add(collision.GetComponent<Interactable>());
            UpdateInteractPopup();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            interactables.Remove(collision.GetComponent<Interactable>());
            UpdateInteractPopup();
        }
    }

    public void LockInteract(bool isLocked)
    {
        IsInteractLocked = isLocked;

        UpdateInteractPopup();
    }

    private void UpdateInteractPopup()
    {
        if (!IsInteractLocked && interactables.Count > 0)
        {
            HeadsUpDisplay.Instance.Interact(interactables[^1].transform);
        }
        else
        {
            HeadsUpDisplay.Instance.Interact(null);
        }
    }

    private async void PlayerDeath()
    {
        if (abilities.CanUse(PlayerAbility.DeathRefusal))
        {
            abilities.abilities[PlayerAbility.DeathRefusal].GoOnCooldown();
            abilities.DeathRefusal();
        }
        else
        {
            IsDead = true;
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerAbilities>().enabled = false;
            GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            anim.TriggerAnimation("Death", false);
            await UniTask.WaitForSeconds(3.0f);
            SceneManager.LoadScene("Main");
        }
    }

    public async Task GainPower(PlayerAbility unlocks)
    {
        abilities.Unlock(unlocks);
        anim.TriggerAnimation("Stake", false);
        abilities.LockAbilities(true);
        movement.LockMovement(true);
        health.SetIntangible(true);
        await UniTask.WaitForSeconds(0.9f);
        anim.EndTriggerAnimation();
        abilities.LockAbilities(false);
        movement.LockMovement(false);
        await UniTask.WaitForSeconds(0.1f);
        health.SetIntangible(false);
    }
}

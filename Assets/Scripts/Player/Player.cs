using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using NUnit.Framework;
using Unity.Cinemachine;
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
    [SerializeField] private DeathScreen deathScreen;

    // Actions
    private InputAction look;
    public Action onKill;

    private EventInstance heartbeat;

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

        deathScreen.gameObject.SetActive(false);
    }

    private void Start()
    {
        health.OnDeath += PlayerDeath;
        health.OnHurtBySource += OnHurt;

        heartbeat = AudioManager.instance.CreateEventInstance(AudioManager.instance.heartbeat);
        heartbeat.setParameterByName("Health", health.Percent);
        heartbeat.start();
    }

    private void OnDestroy()
    {
        StopHeartbeat();
    }

    private void OnHurt(float amount, GameObject source)
    {
        if (health.CurrentHealth <= 0) return;
        AudioManager.instance.PlayOneShot(AudioManager.instance.playerHurt);
    }

    private void StopHeartbeat()
    {
        if (!heartbeat.isValid()) return;
        heartbeat.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        heartbeat.release();
        heartbeat.clearHandle();
    }

    private void Update()
    {
        LookPos = Camera.main.ScreenToWorldPoint(look.ReadValue<Vector2>());
        LookDir = (LookPos - (Vector2)transform.position).normalized;

        if (interactAction.WasPressedThisFrame()) Interact();

        if (heartbeat.isValid()) heartbeat.setParameterByName("Health", health.Percent);
    }

    private void Interact()
    {
        if (IsInteractLocked) return;
        if (interactables.Count > 0)
        {
            interactables[^1].Interact();
        }
    }

    public void AddInteractable(Interactable interactable)
    {
        if (interactable.IsDisabled || interactables.Contains(interactable)) return;
        interactables.Add(interactable);
        UpdateInteractPopup();
    }

    public void RemoveInteractable(Interactable interactable)
    {
        if (interactables.Remove(interactable)) UpdateInteractPopup();
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
            AudioManager.instance.PlayOneShot(AudioManager.instance.deathRefusal);
            abilities.DeathRefusal();
        }
        else
        {
            health.OnDeath -= PlayerDeath;
            IsDead = true;
            StopHeartbeat();
            AudioManager.instance.PlayOneShot(AudioManager.instance.death);
            AudioManager.instance.StopMusic();
            GetComponent<CapsuleCollider2D>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerAbilities>().enabled = false;
            GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
            anim.TriggerAnimation("Death", false);
            await UniTask.WaitForSeconds(3.0f);

            deathScreen.gameObject.SetActive(true);
            deathScreen.OpenDeathScreen();
        }
    }

    public void OpenCoffin()
    {
        anim.TriggerAnimation("IdleDown", false);
        abilities.LockAbilities(true);
        movement.LockMovement(true);
        health.SetIntangible(true);
        anim.LockDirection(true);
    }

    public void UnlockEverything()
    {
        abilities.LockAbilities(false);
        movement.LockMovement(false);
        anim.LockDirection(false);
        health.SetIntangible(false);
    }

    public async UniTask GainPower(PlayerAbility unlocks)
    {
        anim.TriggerAnimation("Stake", false);

        await UniTask.WaitForSeconds(0.2f);

        AudioManager.instance.PlayOneShot(AudioManager.instance.stakeStab);
        abilities.Unlock(unlocks);

        await UniTask.WaitForSeconds(0.1f);

        GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        await UniTask.WaitForSeconds(0.6f);

        anim.EndTriggerAnimation();
    }
}

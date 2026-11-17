using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAbilities : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private MeleeEffect meleeEffectPrefab;
    [SerializeField] private Projectile rangedProjectilePrefab;
    [SerializeField] private MeleeEffect stunConePrefab;
    [SerializeField] private MeleeEffect batSwarmPrefab;
    [SerializeField] private MeleeEffect deathRefusalExplosionPrefab;

    [Header("Refs")]
    [SerializeField] private Transform projPoint;

    public bool IsAbilitiesLocked;

    // Refs
    private PlayerMovement movement;
    private PlayerAnimation anim;

    // Actions
    private InputAction primaryAttack;
    private InputAction secondaryAttack;
    private InputAction ability1;
    private InputAction ability2;
    private InputAction dashAction;

    public PlayerAbility unlockedAbilities = PlayerAbility.None;

    private EventInstance batSwarmSound;

    [Serializable]
    public class AbilityState
    {
        public PlayerAbility ability;
        public float cooldown;
        public bool unlockedByDefault = false;
        public float cooldownTimer;
        public bool onCooldown => cooldownTimer > 0.0f;
        public float cooldownProportion => cooldownTimer / cooldown;
        public bool isUnlocked => Player.Instance.abilities.unlockedAbilities.HasFlag(ability);

        public AbilityState(PlayerAbility ability)
        {
            this.ability = ability;
        }

        public void GoOnCooldown()
        {
            cooldownTimer = cooldown;
        }
    }

    [SerializeField] public Dictionary<PlayerAbility, AbilityState> abilities = new()
    {
        [PlayerAbility.Projectile] = new(PlayerAbility.Projectile),
        [PlayerAbility.Dash] = new(PlayerAbility.Dash),
        [PlayerAbility.BatSwarm] = new(PlayerAbility.BatSwarm),
        [PlayerAbility.Revive] = new(PlayerAbility.Revive),
        [PlayerAbility.Stun] = new(PlayerAbility.Stun),
        [PlayerAbility.Claws] = new(PlayerAbility.Claws),
        [PlayerAbility.DeathRefusal] = new(PlayerAbility.DeathRefusal),
    };
    [SerializeField] public Dictionary<PlayerAbility, AbilityIndicator> abilityIndicators = new();

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        anim = GetComponent<PlayerAnimation>();
        
        primaryAttack = InputSystem.actions.FindAction("PrimaryAttack");
        secondaryAttack = InputSystem.actions.FindAction("SecondaryAttack");
        ability1 = InputSystem.actions.FindAction("Ability1");
        ability2 = InputSystem.actions.FindAction("Ability2");
        dashAction = InputSystem.actions.FindAction("Dash");
    }

    private void Start()
    {
        Player.Instance.onKill += () => { abilities[PlayerAbility.Dash].cooldownTimer = 0.0f; };

        foreach (var abilityState in abilities.Values)
        {
            abilityState.cooldownTimer = 0.0f;
            if (abilityState.unlockedByDefault) unlockedAbilities |= abilityState.ability;
            if (abilityIndicators.TryGetValue(abilityState.ability, out var indicator)) 
            {
                indicator.gameObject.SetActive(abilityState.isUnlocked);
                indicator.GetComponentInParent<ManualHorizontalLayoutGroup>().ApplyLayout();
            }
        }
    }

    public bool CanUse(PlayerAbility ability) => !IsAbilitiesLocked && unlockedAbilities.HasFlag(ability) && !abilities[ability].onCooldown;

    private void Update()
    {
        if (primaryAttack.IsPressed() && CanUse(PlayerAbility.Claws))
        {
            abilities[PlayerAbility.Claws].GoOnCooldown();
            PrimaryAttack(Player.Instance.LookDir);
        }
        if (secondaryAttack.WasPressedThisFrame() && CanUse(PlayerAbility.Projectile))
        {
            abilities[PlayerAbility.Projectile].GoOnCooldown();
            SecondaryAttack(Player.Instance.LookDir);
        }
        if (ability1.WasPressedThisFrame() && CanUse(PlayerAbility.Stun))
        {
            abilities[PlayerAbility.Stun].GoOnCooldown();
            ConeStun();
        }
        if (ability2.WasPressedThisFrame() && CanUse(PlayerAbility.BatSwarm))
        {
            abilities[PlayerAbility.BatSwarm].GoOnCooldown();
            BatSwarm();
        }
        if (dashAction.WasPressedThisFrame() && CanUse(PlayerAbility.Dash))
        {
            abilities[PlayerAbility.Dash].GoOnCooldown();
            AudioManager.instance.PlayOneShot(AudioManager.instance.slicingWhirl);
            movement.Dash();
        }

        foreach (var abilityState in abilities.Values)
        {
            if (!abilityState.isUnlocked) continue;
            bool wasOnCooldown = abilityState.onCooldown;
            abilityState.cooldownTimer = Mathf.Max(abilityState.cooldownTimer - Time.deltaTime, 0.0f);

            if (wasOnCooldown && !abilityState.onCooldown && abilityState.ability == PlayerAbility.DeathRefusal)
            {
                AudioManager.instance.PlayOneShot(AudioManager.instance.reviveOffCooldown);
            }
        }
    }

    public void Unlock(PlayerAbility ability)
    {
        unlockedAbilities |= ability;
        AudioManager.instance.PlayOneShot(AudioManager.instance.unlockAbility);
        if (abilityIndicators.TryGetValue(ability, out var indicator))
        {
            indicator.gameObject.SetActive(true);
            indicator.GetComponentInParent<ManualHorizontalLayoutGroup>().ApplyLayout();
            indicator.DoStartAnimation();
        }
    }

    public void DeathRefusal()
    {
        LockAbilities(true);
        movement.LockMovement(true);
        anim.TriggerAnimation("Revive", false);
        Player.Instance.health.SetIntangible(true);
        Player.Instance.health.CurrentHealth = 1;
        MeleeEffect deathRefuse = Instantiate(deathRefusalExplosionPrefab, transform);
        deathRefuse.Init(projPoint.position, Player.Instance.LookDir, gameObject, "Player");
        //StartCoroutine(DeathRefuse(anim.animator.GetCurrentAnimatorClipInfo(0)[0].clip.length));
        // TODO: Hardcoded cause current clip doesn't get updated immediately
        StartCoroutine(DeathRefuse(1.0f));
    }

    public void LockAbilities(bool isLocked)
    {
        IsAbilitiesLocked = isLocked;
        Player.Instance.LockInteract(isLocked);
    }

    public IEnumerator LockAbilitiesIE(bool isLocked, float delay)
    {
        yield return new WaitForSeconds(delay);
        LockAbilities(isLocked);
    }

    // Claw melee
    public void PrimaryAttack(Vector2 direction)
    {
        LockAbilities(true);
        AudioManager.instance.PlayOneShot(AudioManager.instance.clawSwipe);
        MeleeEffect meleeEffect = Instantiate(meleeEffectPrefab, transform);
        meleeEffect.Init(projPoint.position + (Vector3)direction, direction, gameObject, "Player");
        StartCoroutine(LockAbilitiesIE(false, meleeEffect.VisualDuration));
    }

    // Blood spike
    public void SecondaryAttack(Vector2 direction)
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.bloodSpike);
        Projectile projectile = Instantiate(rangedProjectilePrefab);
        projectile.Init(projPoint.position + (Vector3)direction, direction, gameObject, "Player");
    }

    private async void ConeStun()
    {
        LockAbilities(false);
        anim.TriggerAnimation("Stun", true, true);
        AudioManager.instance.PlayOneShot(AudioManager.instance.stunFlash);
        MeleeEffect stunCone = Instantiate(stunConePrefab, transform);
        stunCone.Init(projPoint.position, Player.Instance.LookDir, gameObject, "Player");
        await UniTask.WaitForSeconds(stunCone.VisualDuration);
        anim.EndTriggerAnimation();
        LockAbilities(false);
    }

    private void BatSwarm()
    {
        LockAbilities(true);
        Player.Instance.health.SetIntangible(true);
        MeleeEffect batSwarm = Instantiate(batSwarmPrefab, transform);
        batSwarm.Init(projPoint.position, Player.Instance.LookDir, gameObject, "Player");
        StartCoroutine(BatSwarm(3.0f));
    }

    private IEnumerator BatSwarm(float duration)
    {
        movement.GoIntangible(true);

        StopBatSwarmSound();
        batSwarmSound = AudioManager.instance.CreateEventInstance(AudioManager.instance.batSwarm);
        batSwarmSound.start();

        yield return new WaitForSeconds(duration);

        StopBatSwarmSound();
        movement.StopIntangibility();
        Player.Instance.health.SetIntangible(false);
        LockAbilities(false);
    }

    private void StopBatSwarmSound()
    {
        if (!batSwarmSound.isValid()) return;
        batSwarmSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        batSwarmSound.release();
        batSwarmSound.clearHandle();
    }

    private void OnDestroy()
    {
        StopBatSwarmSound();
    }

    private IEnumerator DeathRefuse(float duration)
    {
        yield return new WaitForSeconds(duration);
        Player.Instance.health.SetIntangible(false);
        LockAbilities(false);
        anim.EndTriggerAnimation();
        movement.LockMovement(false);
    }
}

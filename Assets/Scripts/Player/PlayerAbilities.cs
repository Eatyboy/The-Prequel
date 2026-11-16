using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                LayoutRebuilder.ForceRebuildLayoutImmediate(indicator.transform.parent.GetComponent<RectTransform>());
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
            AbilityOne();
        }
        if (ability2.WasPressedThisFrame() && CanUse(PlayerAbility.BatSwarm))
        {
            abilities[PlayerAbility.BatSwarm].GoOnCooldown();
            AbilityTwo();
        }
        if (dashAction.WasPressedThisFrame() && CanUse(PlayerAbility.Dash))
        {
            abilities[PlayerAbility.Dash].GoOnCooldown();
            movement.Dash();
        }

        foreach (var abilityState in abilities.Values)
        {
            if (!abilityState.isUnlocked) continue;
            abilityState.cooldownTimer = Mathf.Max(abilityState.cooldownTimer - Time.deltaTime, 0.0f);
        }
    }

    public void Unlock(PlayerAbility ability)
    {
        unlockedAbilities |= ability;
        if (abilityIndicators.TryGetValue(ability, out var indicator))
        {
            indicator.gameObject.SetActive(true);
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
        deathRefuse.Init(transform.position, Player.Instance.LookDir, gameObject, "Player");
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
        MeleeEffect meleeEffect = Instantiate(meleeEffectPrefab, transform);
        meleeEffect.Init(transform.position + (Vector3)direction, direction, gameObject, "Player");
        StartCoroutine(LockAbilitiesIE(false, meleeEffect.VisualDuration));
    }

    // Blood spike
    public void SecondaryAttack(Vector2 direction)
    {
        Projectile projectile = Instantiate(rangedProjectilePrefab);
        projectile.Init(transform.position + (Vector3)direction, direction, gameObject, "Player");
    }

    // Cone stun
    private void AbilityOne()
    {
        LockAbilities(false);
        MeleeEffect stunCone = Instantiate(stunConePrefab, transform);
        stunCone.Init(transform.position, Player.Instance.LookDir, gameObject, "Player");
        StartCoroutine(LockAbilitiesIE(false, stunCone.VisualDuration));
    }

    // Bat swarm invulnerability
    private void AbilityTwo()
    {
        LockAbilities(true);
        Player.Instance.health.SetIntangible(true);
        MeleeEffect batSwarm = Instantiate(batSwarmPrefab, transform);
        batSwarm.Init(transform.position, Player.Instance.LookDir, gameObject, "Player");
        StartCoroutine(BatSwarm(3.0f));
    }

    private IEnumerator BatSwarm(float duration)
    {
        movement.GoIntangible(true);
        yield return new WaitForSeconds(duration);
        movement.StopIntangibility();
        Player.Instance.health.SetIntangible(false);
        LockAbilities(false);
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

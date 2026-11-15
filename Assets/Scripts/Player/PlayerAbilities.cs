using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAbilities : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private MeleeEffect stunConePrefab;

    // Refs
    private PlayerMovement movement;
    private PlayerAttacks attacks;

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
    };
    [SerializeField] public Dictionary<PlayerAbility, AbilityIndicator> abilityIndicators = new();

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        attacks = GetComponent<PlayerAttacks>();
        
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
            }
        }
    }

    private bool CanUse(PlayerAbility ability) => unlockedAbilities.HasFlag(ability) && !abilities[ability].onCooldown;

    private void Update()
    {
        if (primaryAttack.IsPressed() && CanUse(PlayerAbility.Claws))
        {
            abilities[PlayerAbility.Claws].GoOnCooldown();
            attacks.PrimaryAttack(Player.Instance.LookDir);
        }
        if (secondaryAttack.WasPressedThisFrame() && CanUse(PlayerAbility.Projectile))
        {
            abilities[PlayerAbility.Projectile].GoOnCooldown();
            attacks.SecondaryAttack(Player.Instance.LookDir);
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

    // Cone stun
    private void AbilityOne()
    {
        MeleeEffect stunCone = Instantiate(stunConePrefab, transform);
        stunCone.Init(transform.position, Player.Instance.LookDir, gameObject, "Player");
    }

    // Bat swarm invulnerability
    private void AbilityTwo()
    {

    }
}

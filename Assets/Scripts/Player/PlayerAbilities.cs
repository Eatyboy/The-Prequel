using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerAbilities : MonoBehaviour
{
    // Refs
    private PlayerMovement movement;

    // Actions
    private InputAction ability1;
    private InputAction ability2;
    private InputAction dashAction;

    public PlayerAbility unlockedAbilities = PlayerAbility.None;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        
        ability1 = InputSystem.actions.FindAction("Ability1");
        ability2 = InputSystem.actions.FindAction("Ability2");
        dashAction = InputSystem.actions.FindAction("Dash");
    }

    private void Update()
    {
        if (ability1.WasPressedThisFrame())
        {
            AbilityOne();
        }
        if (ability2.WasPressedThisFrame())
        {
            AbilityTwo();
        }
        if (dashAction.WasPressedThisFrame() && unlockedAbilities.HasFlag(PlayerAbility.Dash))
        {
            movement.Dash();
        }
    }

    // Cone stun
    private void AbilityOne()
    {

    }

    // Bat swarm invulnerability
    private void AbilityTwo()
    {

    }
}

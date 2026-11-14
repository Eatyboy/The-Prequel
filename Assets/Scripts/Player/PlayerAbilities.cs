using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAbilities : MonoBehaviour
{
    // Actions
    private InputAction ability1;
    private InputAction ability2;

    private void Awake()
    {
        ability1 = InputSystem.actions.FindAction("Ability1");
        ability2 = InputSystem.actions.FindAction("Ability2");
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

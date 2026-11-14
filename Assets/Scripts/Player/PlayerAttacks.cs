using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Projectile rangedProjectile;

    // Actions
    private InputAction primaryAttack;
    private InputAction secondaryAttack;

    private void Awake()
    {
        primaryAttack = InputSystem.actions.FindAction("PrimaryAttack");
        secondaryAttack = InputSystem.actions.FindAction("SecondaryAttack");
    }

    private void Update()
    {
        if (primaryAttack.WasPressedThisFrame())
        {
            PrimaryAttack(Player.Instance.LookDir);
        }
        if (secondaryAttack.WasPressedThisFrame())
        {
            SecondaryAttack(Player.Instance.LookDir);
        }
    }

    // Claw melee
    private void PrimaryAttack(Vector2 direction)
    {
        
    }

    // Blood spike
    private void SecondaryAttack(Vector2 direction)
    {
        Projectile projectile = Instantiate(rangedProjectile);
        projectile.Init(transform.position, direction, gameObject, "Player");
    }
}

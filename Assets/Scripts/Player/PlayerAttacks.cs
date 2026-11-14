using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject rangedProjectile;

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
        GameObject projectileGO = Instantiate(rangedProjectile);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        projectile.Source = gameObject;
        projectile.StartDirection = direction;
        projectileGO.transform.position = transform.position;
        projectile.IgnoreTag = "Player";
    }
}

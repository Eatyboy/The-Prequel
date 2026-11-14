using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject meleeProjectile;

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
            SecondaryAttack();
        }
    }

    // Claw melee
    private void PrimaryAttack(Vector2 direction)
    {
        GameObject projectileGO = Instantiate(meleeProjectile);
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        projectile.Source = gameObject;
        projectile.StartDirection = direction;
        projectileGO.transform.position = transform.position;
        projectile.IgnoreTag = "Player";
    }

    // Blood spike
    private void SecondaryAttack()
    {

    }
}

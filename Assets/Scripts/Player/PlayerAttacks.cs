using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] MeleeEffect meleeEffectPrefab;
    [SerializeField] Projectile rangedProjectilePrefab;

    private void Update()
    {
        // Input handled in PlayerAbilities
    }

    // Claw melee
    public void PrimaryAttack(Vector2 direction)
    {
        MeleeEffect meleeEffect = Instantiate(meleeEffectPrefab, transform);
        meleeEffect.Init(transform.position + (Vector3)direction, direction, gameObject, "Player");
    }

    // Blood spike
    public void SecondaryAttack(Vector2 direction)
    {
        Projectile projectile = Instantiate(rangedProjectilePrefab);
        projectile.Init(transform.position + (Vector3)direction, direction, gameObject, "Player");
    }
}

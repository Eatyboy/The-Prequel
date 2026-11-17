using Cysharp.Threading.Tasks;
using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerAbilities)), RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Config")]
    public float speed;

    // Vars
    public Vector2 moveDir;

    // Refs
    private InputAction moveAction;
    private CapsuleCollider2D capsuleCol;
    private Rigidbody2D rb;
    private PlayerAbilities abilities;
    private PlayerAnimation anim;
    private Player player;

    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask foregroundLayerMask;

    [Header("Dash")]
    [SerializeField] private float dashDamageRadius = 1.0f;
    [SerializeField] private float dashDistance = 3.0f;
    [SerializeField] private float dashDuration = 0.5f;
    private float dashSpeed => dashDistance / dashDuration;
    [SerializeField] private float dashDamage = 2.0f;

    public bool isDashing = false;
    public bool isMovementLocked;
    public bool isInvisible;

    private EventInstance runningSound;
    private bool isRunningSoundPlaying;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
        capsuleCol = GetComponent<CapsuleCollider2D>();
        abilities = GetComponent<PlayerAbilities>();
        anim = GetComponent<PlayerAnimation>();
        player = GetComponent<Player>();
    }

    private void Start()
    {
        runningSound = AudioManager.instance.CreateEventInstance(AudioManager.instance.running);
    }

    private void OnDestroy()
    {
        if (!runningSound.isValid()) return;
        runningSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        runningSound.release();
        runningSound.clearHandle();
    }

    private void Update()
    {
        if (!player.IsDead) moveDir = moveAction.ReadValue<Vector2>();
        UpdateRunningSound();
    }

    private void UpdateRunningSound()
    {
        bool isRunning = !player.IsDead
            && !isMovementLocked
            && !isDashing
            && !isInvisible
            && Time.timeScale > 0.0f
            && moveDir.sqrMagnitude > 0.01f;

        if (isRunning == isRunningSoundPlaying || !runningSound.isValid()) return;

        if (isRunning) runningSound.start();
        else runningSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        isRunningSoundPlaying = isRunning;
    }

    private void FixedUpdate()
    {
        if (isMovementLocked)
        {
            moveDir = Vector2.zero;
        }

        if (!isDashing)
        {
            rb.linearVelocity = moveDir * speed;
        }
    }

    public async void Dash()
    {
        isDashing = true;
        anim.TriggerAnimation("Dash");
        float dashTimer = 0.0f;
        Vector2 initialDirection = moveDir;
        rb.linearVelocity = dashSpeed * initialDirection;
        GoIntangible();
        HashSet<Collider2D> enemiesHit = new();

        while (dashTimer < dashDuration)
        {
            dashTimer += Time.fixedDeltaTime;

            var enemyColliders = Physics2D.OverlapCircleAll(transform.position, dashDamageRadius, enemyLayerMask);
            foreach (var enemyCol in enemyColliders)
            {
                if (enemiesHit.Contains(enemyCol)) continue;

                var enemy = enemyCol.GetComponent<Enemy>();
                enemy.health.TakeDamage(dashDamage);
                enemiesHit.Add(enemyCol);
            }

            await UniTask.WaitForFixedUpdate(cancellationToken: default);
        }

        StopIntangibility();
        isDashing = false;
        anim.EndTriggerAnimation();
    }

    public void LockMovement(bool isLocked)
    {
        isMovementLocked = isLocked;
    }

    public void GoIntangible(bool isInvisible = false)
    {
        capsuleCol.excludeLayers |= enemyLayerMask | foregroundLayerMask;
        if (isInvisible)
        {
            this.isInvisible = true;
            foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
            {
                sr.enabled = false;
            }
        }
    }

    public void StopIntangibility()
    {
        capsuleCol.excludeLayers &= ~(enemyLayerMask | foregroundLayerMask);
        isInvisible = false;
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = true;
        }
    }
}

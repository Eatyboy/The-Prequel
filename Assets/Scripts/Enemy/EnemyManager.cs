using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [Header("Refs")]
    [SerializeField] private List<EnemyWave> enemyWaves;
    [SerializeField] private Canvas enemyHealthBarCanvas;
    [SerializeField] private HealthBar enemyHealthBarPrefab;

    [Header("Config")]
    [SerializeField] private float loopFactor = 0.9f;
    [SerializeField] private float spawnDelaySeconds = 5.0f;
    [SerializeField] private Vector2 spawnDistanceRange = new(10.0f, 15.0f);
    public Vector2 enemyHealthBarOffset;

    [Header("Vars")]
    public List<Enemy> enemies;

    public bool shouldSpawnEnemies = true;

    private float loopTimeFactor;
    private bool isSpawning;

    private void Start()
    {
        loopTimeFactor = 1.0f;
        HandleSpawning();
    }

    private void Update()
    {
        if (!isSpawning)
        {
            // :sob:
            // Just making time shorter between spawns
            loopTimeFactor *= loopFactor;
            HandleSpawning();
        }
    }

    public Enemy SpawnEnemy(Enemy enemyPrefab)
    {
        float spawnDistance = Random.Range(spawnDistanceRange.x, spawnDistanceRange.y);
        float spawnAngle = Random.Range(0.0f, 2.0f * Mathf.PI);
        Vector2 spawnPosition = new(spawnDistance * Mathf.Cos(spawnAngle), spawnDistance * Mathf.Sin(spawnAngle));
        Enemy enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);

        HealthBar healthBar = Instantiate(enemyHealthBarPrefab, spawnPosition + enemyHealthBarOffset, Quaternion.identity, enemyHealthBarCanvas.transform);
        enemy.health.healthBar = healthBar;

        return enemy;
    }

    private async void HandleSpawning(CancellationToken ct = default)
    {
        isSpawning = true;
        foreach (var wave in enemyWaves)
        {
            try
            {
                await UniTask.WaitForSeconds(wave.initialDelay * loopTimeFactor, cancellationToken: this.GetCancellationTokenOnDestroy());
                foreach (var group in wave.spawnGroups)
                {
                    for (int enemyIndex = 0; enemyIndex < group.amount; enemyIndex++)
                    {
                        enemies.Add(SpawnEnemy(group.enemy));
                        await UniTask.WaitForSeconds(group.delayBetweenEnemySpawns * loopTimeFactor, cancellationToken: this.GetCancellationTokenOnDestroy());
                    }
                    await UniTask.WaitForSeconds(group.delayUntilNextSpawnGroup * loopTimeFactor, cancellationToken: this.GetCancellationTokenOnDestroy());
                }
            }
            catch
            {

            }
        }
        isSpawning = false;
    }
}

[System.Serializable]
public class EnemySpawnGroup
{
    public Enemy enemy;
    public int amount = 1;
    public float delayBetweenEnemySpawns = 0.0f;
    public float delayUntilNextSpawnGroup = 0.0f;
}

[System.Serializable]
public class EnemyWave
{
    public List<EnemySpawnGroup> spawnGroups;
    public float initialDelay;
}

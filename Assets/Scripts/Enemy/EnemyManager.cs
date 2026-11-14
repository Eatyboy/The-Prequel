using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<MonoBehaviour>
{
    [Header("Refs")]
    [SerializeField] private Enemy enemyPrefab;

    [Header("Config")]
    [SerializeField] private float spawnDelaySeconds = 5.0f;
    [SerializeField] private Vector2 spawnDistanceRange = new(10.0f, 15.0f);

    [Header("Vars")]
    public List<Enemy> enemies;

    private float spawnTimer = 0.0f;

    private void Start()
    {
        spawnTimer = spawnDelaySeconds;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        spawnTimer -= dt;
        if (spawnTimer <= 0.0f)
        {
            enemies.Add(SpawnEnemy());
            spawnTimer = spawnDelaySeconds;
        }
    }

    public Enemy SpawnEnemy()
    {
        float spawnDistance = Random.Range(spawnDistanceRange.x, spawnDistanceRange.y);
        float spawnAngle = Random.Range(0.0f, 2.0f * Mathf.PI);
        Vector2 spawnPosition = new(spawnDistance * Mathf.Cos(spawnAngle), spawnDistance * Mathf.Sin(spawnAngle));
        Enemy enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);

        return enemy;
    }
}

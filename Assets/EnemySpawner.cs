using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;     // Time between spawns
    public int maxEnemies = 10;          // Max alive at once
    public float spawnRadius = 20f;      // Radius around player to spawn

    private List<GameObject> enemies = new List<GameObject>();
    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            // Clean up destroyed enemies
            enemies.RemoveAll(e => e == null);
            if (enemies.Count >= maxEnemies)
                continue;

            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (player == null || enemyPrefab == null)
            return;
        // Random position around player
        Vector2 spawnDir = Random.insideUnitCircle.normalized;
        Vector2 spawnPos = (Vector2)player.position + spawnDir * spawnRadius;

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        enemies.Add(enemy);
    }
}

using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;     // Time between spawns
    public float spawnRadius = 20f;    // Distance from player to spawn

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
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (player == null || enemyPrefab == null)
            return;

        // pick a random direction and spawn at that radius around the player
        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector2 spawnPos = (Vector2)player.position + dir * spawnRadius;

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}

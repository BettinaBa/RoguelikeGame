using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;

    private float nextSpawnTime;
    private int enemiesDefeated;
    private float startTime;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval / GetDifficultyMultiplier();
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0)
            return;

        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
        EnemyHealth eh = enemy.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.maxHealth = Mathf.RoundToInt(eh.maxHealth * GetDifficultyMultiplier());
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
            ai.damage = Mathf.RoundToInt(ai.damage * GetDifficultyMultiplier());
    }

    public void RegisterKill()
    {
        enemiesDefeated++;
    }

    float GetDifficultyMultiplier()
    {
        float timeFactor = (Time.time - startTime) / 60f;
        float killFactor = enemiesDefeated * 0.1f;
        return 1f + timeFactor + killFactor;
    }
}

using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Spawn Settings")]
    public GameObject meleePrefab;     // your melee Enemy prefab
    public GameObject shooterPrefab;   // your ranged ShooterEnemy prefab
    public float spawnInterval = 5f;
    [Tooltip("How many enemies to spawn each wave")]
    public int spawnCount = 3;

    private float nextSpawnTime = 0f;
    private int enemiesDefeated;
    /// <summary>
    /// How many enemies have been killed so far.
    /// </summary>
    public int EnemiesDefeated => enemiesDefeated;

    private float startTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        startTime = Time.time;
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        // Pause spawning if the game is over
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        if (Time.time >= nextSpawnTime)
        {
            // spawn a small squad
            for (int i = 0; i < spawnCount; i++)
                SpawnEnemy();

            // schedule the next wave
            nextSpawnTime = Time.time + spawnInterval / GetDifficultyMultiplier();
        }
    }

    void SpawnEnemy()
    {
        // choose melee or shooter at random
        GameObject prefabToSpawn = (Random.value < 0.5f)
            ? meleePrefab
            : shooterPrefab;

        if (prefabToSpawn == null)
            return;

        // determine spawn area
        Bounds bounds = (MapChunkManager.Instance != null)
            ? MapChunkManager.Instance.GetLoadedBounds()
            : GetCameraBounds();

        float offset = 1f;
        Vector3 spawnPos;
        switch (Random.Range(0, 4))
        {
            case 0:
                spawnPos = new Vector3(bounds.min.x + offset,
                                       Random.Range(bounds.min.y, bounds.max.y),
                                       0f);
                break;
            case 1:
                spawnPos = new Vector3(bounds.max.x - offset,
                                       Random.Range(bounds.min.y, bounds.max.y),
                                       0f);
                break;
            case 2:
                spawnPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                                       bounds.max.y - offset,
                                       0f);
                break;
            default:
                spawnPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                                       bounds.min.y + offset,
                                       0f);
                break;
        }

        // instantiate & scale stats
        GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

        // scale health if it has EnemyHealth
        var eh = enemy.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.maxHealth = Mathf.RoundToInt(eh.maxHealth * GetDifficultyMultiplier());

        // scale damage if it has EnemyAI
        var ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
            ai.damage = Mathf.RoundToInt(ai.damage * GetDifficultyMultiplier());
    }

    private Bounds GetCameraBounds()
    {
        var cam = Camera.main;
        if (cam == null)
            return new Bounds();

        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;
        return new Bounds(cam.transform.position,
                          new Vector3(width, height, 0f));
    }

    /// <summary>
    /// Call this when an enemy dies.
    /// </summary>
    public void RegisterKill()
    {
        enemiesDefeated++;
    }

    /// <summary>
    /// Increases over time and per kill to ramp difficulty.
    /// </summary>
    float GetDifficultyMultiplier()
    {
        float timeFactor = (Time.time - startTime) / 60f;  // +1 per minute
        float killFactor = enemiesDefeated * 0.1f;         // +0.1 per kill
        return 1f + timeFactor + killFactor;
    }
}
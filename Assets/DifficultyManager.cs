using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
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
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval / GetDifficultyMultiplier();
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
            return;

        // Determine spawn bounds
        Bounds bounds = MapChunkManager.Instance != null
            ? MapChunkManager.Instance.GetLoadedBounds()
            : GetCameraBounds();

        // Choose a random edge position
        float offset = 1f;
        Vector3 spawnPos;
        switch (Random.Range(0, 4))
        {
            case 0:
                spawnPos = new Vector3(bounds.min.x + offset, Random.Range(bounds.min.y, bounds.max.y), 0f);
                break;
            case 1:
                spawnPos = new Vector3(bounds.max.x - offset, Random.Range(bounds.min.y, bounds.max.y), 0f);
                break;
            case 2:
                spawnPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y - offset, 0f);
                break;
            default:
                spawnPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.min.y + offset, 0f);
                break;
        }

        // Instantiate enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        // Scale health by difficulty
        EnemyHealth eh = enemy.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.maxHealth = Mathf.RoundToInt(eh.maxHealth * GetDifficultyMultiplier());

        // Scale AI damage by difficulty
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
            ai.damage = Mathf.RoundToInt(ai.damage * GetDifficultyMultiplier());
    }

    private Bounds GetCameraBounds()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return new Bounds();

        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;
        return new Bounds(cam.transform.position, new Vector3(width, height, 0f));
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

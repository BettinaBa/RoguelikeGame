using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Spawn Settings")]
    public GameObject meleePrefab;
    public GameObject shooterPrefab;
    [Tooltip("Prefab for the mini-boss.")]
    public GameObject bossPrefab;
    [Tooltip("Seconds until boss appears.")]
    public float bossSpawnTime = 180f;
    [Tooltip("Minimum distance (units) from player when boss spawns.")]
    public float minBossDistance = 8f;
    [Tooltip("Seconds between regular enemy spawns.")]
    public float spawnInterval = 5f;

    private float nextSpawnTime;
    private bool bossSpawned = false;
    private int enemiesDefeated;
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
        nextSpawnTime = startTime + spawnInterval;
    }

    void Update()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        float elapsed = Time.time - startTime;

        // 1) Boss spawn after bossSpawnTime, at least minBossDistance away
        if (!bossSpawned && elapsed >= bossSpawnTime)
        {
            SpawnBoss();
            bossSpawned = true;
        }

        // 2) Regular enemies
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval / GetDifficultyMultiplier();
        }
    }

    private void SpawnEnemy()
    {
        GameObject prefab = (Random.value < 0.5f) ? meleePrefab : shooterPrefab;
        if (prefab == null) return;

        Vector3 pos = GetEdgeSpawnPosition();
        var enemy = Instantiate(prefab, pos, Quaternion.identity);

        // **Only scale health**, and at half the usual rate
        if (enemy.TryGetComponent<EnemyHealth>(out var eh))
        {
            float ramp = GetDifficultyMultiplier();
            float slowRamp = 1f + (ramp - 1f) * 0.5f;
            eh.maxHealth = Mathf.RoundToInt(eh.maxHealth * slowRamp);
        }

        // leave damage untouched
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null) return;
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            // fallback to camera-center if no Player found
            var cam = Camera.main;
            Instantiate(bossPrefab,
                cam != null ? (Vector3)cam.transform.position : Vector3.zero,
                Quaternion.identity);
            return;
        }

        // pick random point in view until it's far enough from player
        Bounds area = GetCameraBounds();
        Vector3 spawn;
        int tries = 0;
        do
        {
            spawn = new Vector3(
                Random.Range(area.min.x, area.max.x),
                Random.Range(area.min.y, area.max.y),
                0f
            );
            tries++;
        }
        while (Vector2.Distance(spawn, player.transform.position) < minBossDistance
               && tries < 20);

        Instantiate(bossPrefab, spawn, Quaternion.identity);
        Debug.Log("Mini-boss spawned at " + spawn);
    }

    private Vector3 GetEdgeSpawnPosition()
    {
        Bounds b = (MapChunkManager.Instance != null)
            ? MapChunkManager.Instance.GetLoadedBounds()
            : GetCameraBounds();
        float offset = 1f;
        switch (Random.Range(0, 4))
        {
            case 0: return new Vector3(b.min.x + offset, Random.Range(b.min.y, b.max.y), 0f);
            case 1: return new Vector3(b.max.x - offset, Random.Range(b.min.y, b.max.y), 0f);
            case 2: return new Vector3(Random.Range(b.min.x, b.max.x), b.max.y - offset, 0f);
            default: return new Vector3(Random.Range(b.min.x, b.max.x), b.min.y + offset, 0f);
        }
    }

    private Bounds GetCameraBounds()
    {
        var cam = Camera.main;
        if (cam == null) return new Bounds(Vector3.zero, Vector3.one * 10f);
        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;
        return new Bounds(cam.transform.position, new Vector3(width, height, 0f));
    }

    public void RegisterKill()
    {
        enemiesDefeated++;
        // if you have RunMetrics:
        RunMetrics.Instance?.RegisterKill();
    }

    private float GetDifficultyMultiplier()
    {
        float timeFactor = (Time.time - startTime) / 60f;     // +1 per minute
        float killFactor = enemiesDefeated * 0.1f;            // +0.1 per kill
        return 1f + timeFactor + killFactor;
    }
}

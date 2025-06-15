using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Spawn Settings")]
    public GameObject meleePrefab;
    public GameObject shooterPrefab;
    public GameObject teleporterPrefab;
    public GameObject advancedPrefab;
    public GameObject behaviorPrefab;
    [Tooltip("Prefab for the mini-boss.")]
    public GameObject bossPrefab;
    [Tooltip("Seconds until boss appears.")]
    public float bossSpawnTime = 180f;
    [Tooltip("Minimum distance (units) from player when boss spawns.")]
    public float minBossDistance = 8f;
    [Tooltip("Seconds between regular enemy spawns.")]
    public float spawnInterval = 5f;

    [Header("Difficulty Weights")]
    [Tooltip("Multiplier added per minute elapsed.")]
    public float timeWeight = 1f;
    [Tooltip("Multiplier added per enemy kill.")]
    public float killWeight = 0.1f;
    [Tooltip("Multiplier added per point of damage taken.")]
    public float damageWeight = 0.05f;
    [Tooltip("Weight for time spent chasing vs. attacking.")]
    public float chaseWeight = 0.5f;

    [Tooltip("Multiplier added based on shot accuracy (0-1).")]
    public float accuracyWeight = 1f;
    [Tooltip("Multiplier added per second faster kill time.")]
    public float killTimeWeight = 1f;

    [Header("Difficulty Curve")]
    [Tooltip("Growth rate for the logistic difficulty curve.")]
    public float growthRate = 0.5f;
    [Tooltip("Midpoint for the logistic difficulty curve.")]
    public float midpoint = 10f;

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
        GameObject prefab;
        float roll = Random.value;
        if (roll < 0.2f)
            prefab = meleePrefab;
        else if (roll < 0.4f)
            prefab = shooterPrefab;
        else if (roll < 0.6f)
            prefab = teleporterPrefab;
        else if (roll < 0.8f)
            prefab = advancedPrefab;
        else
            prefab = behaviorPrefab;
        if (prefab == null) return;

        Vector3 pos = GetEdgeSpawnPosition();
        var enemy = Instantiate(prefab, pos, Quaternion.identity);

        // Scale health and move speed using the difficulty multiplier
        float ramp = GetDifficultyMultiplier();
        float slowRamp = 1f + (ramp - 1f) * 0.5f;

        if (enemy.TryGetComponent<EnemyHealth>(out var eh))
            eh.maxHealth = Mathf.RoundToInt(eh.maxHealth * slowRamp);

        if (enemy.TryGetComponent<EnemyAI>(out var ai))
            ai.moveSpeed *= slowRamp;
        else if (enemy.TryGetComponent<AdvancedEnemyAI>(out var adv))
            adv.moveSpeed *= slowRamp;
        else if (enemy.TryGetComponent<BehaviorEnemyAI>(out var btree))
            btree.moveSpeed *= slowRamp;

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

        // pick random point in view until it's far enough from the player
        Bounds area = GetCameraBounds();
        Vector3 spawn = player.transform.position + Vector3.right * minBossDistance;
        int tries = 0;
        while (tries < 20)
        {
            Vector3 candidate = new Vector3(
                Random.Range(area.min.x, area.max.x),
                Random.Range(area.min.y, area.max.y),
                0f
            );
            tries++;
            if (Vector2.Distance(candidate, player.transform.position) < minBossDistance)
                continue;
            spawn = candidate;
            break;
        }

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
        float elapsedMin = (Time.time - startTime) / 60f;
        float timeFactor = elapsedMin * timeWeight;
        float killFactor = enemiesDefeated * killWeight;

        float damageFactor = 0f;
        float chaseFactor = 0f;
        float accuracyFactor = 0f;
        float killTimeFactor = 0f;
        var m = RunMetrics.Instance;
        if (m != null)
        {
            damageFactor = m.damageTaken * damageWeight;
            float ratio = m.timeInChase / Mathf.Max(1f, m.timeInAttack);
            chaseFactor = ratio * chaseWeight;
            accuracyFactor = m.Accuracy * accuracyWeight;
            if (m.AverageKillTime > 0f)
                killTimeFactor = killTimeWeight / m.AverageKillTime;
        }

        float linearScore = timeFactor + killFactor + damageFactor + chaseFactor + accuracyFactor + killTimeFactor;
        float logistic = 1f / (1f + Mathf.Exp(-growthRate * (linearScore - midpoint)));
        return 1f + logistic;
    }
}

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

        Bounds bounds = MapChunkManager.Instance != null ? MapChunkManager.Instance.GetLoadedBounds() : new Bounds();
        bool useChunk = bounds.size != Vector3.zero;
        if (!useChunk)
        {
            Camera cam = Camera.main;
            if (cam == null)
                return;

            float height = cam.orthographicSize * 2f;
            float width = height * cam.aspect;
            Vector3 camPos = cam.transform.position;
            bounds = new Bounds(camPos, new Vector3(width, height, 0f));
        }

        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float minY = bounds.min.y;
        float maxY = bounds.max.y;
        float offset = 1f;

        Vector3 spawnPos = Vector3.zero;
        switch (Random.Range(0, 4))
        {
            case 0:
                spawnPos = new Vector3(minX + offset, Random.Range(minY, maxY), 0f);
                break;
            case 1:
                spawnPos = new Vector3(maxX - offset, Random.Range(minY, maxY), 0f);
                break;
            case 2:
                spawnPos = new Vector3(Random.Range(minX, maxX), maxY - offset, 0f);
                break;
            default:
                spawnPos = new Vector3(Random.Range(minX, maxX), minY + offset, 0f);
                break;
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        EnemyHealth eh = enemy.GetComponent<EnemyHealth>();
        if (eh != null)
            eh.maxHealth = Mathf.RoundToInt(eh.maxHealth * GetDifficultyMultiplier());
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null)
            ai.damage = Mathf.RoundToInt(ai.damage * GetDifficultyMultiplier());

        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            enemy.transform.up = (player.position - enemy.transform.position).normalized;
            EnemyFollow follow = enemy.GetComponent<EnemyFollow>();
            if (follow != null)
                follow.player = player;
        }
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

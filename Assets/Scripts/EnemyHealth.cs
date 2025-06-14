using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    public int CurrentHealth => currentHealth;

    [Header("Drop Settings")]
    [Tooltip("Chance for dropping health on death")] public float healthDropChance = 0.2f;
    public int experienceReward = 1;

    [Header("Feedback")]
    public FloatingDamageText damageTextPrefab;

    private float spawnTime;

    void Start()
    {
        currentHealth = maxHealth;
        spawnTime = Time.time;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Enemy HP: {currentHealth}/{maxHealth}");

        if (damageTextPrefab != null)
        {
            var txt = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            txt.Setup(amount);
        }

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        RunMetrics.Instance?.RegisterKillTime(Time.time - spawnTime);
        // 1) Spawn procedural loot
        if (LootGenerator.Instance != null)
            LootGenerator.Instance.DropLoot(transform.position);

        // 2) Register kill
        DifficultyManager.Instance?.RegisterKill();

        // Trigger AoE burst if player has the upgrade
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var stats = player.GetComponent<PlayerStats>();
            if (stats != null && stats.killAoE)
            {
                var prefab = Resources.Load<GameObject>("AoEBurst");
                if (prefab != null)
                    Instantiate(prefab, transform.position, Quaternion.identity);
            }
        }

        // 3) Tear down
        Destroy(gameObject);

        // 4) Regenerate room if needed
        var rg = FindAnyObjectByType<RoomGenerator>();
        if (rg != null) rg.RegenerateRoom();
    }
}

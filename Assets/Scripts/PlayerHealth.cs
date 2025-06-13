using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Base health of the player; overridden by permanent upgrades.")]
    public int maxHealth = 5;
    private int currentHealth;

    [Tooltip("Floating damage text prefab.")]
    public FloatingDamageText damageTextPrefab;

    public int CurrentHealth => currentHealth;

    void Start()
    {
        // Load permanent MaxHP (Meta_MaxHP)
        maxHealth = PlayerPrefs.GetInt("Meta_MaxHP", maxHealth);
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        // Register into run metrics
        RunMetrics.Instance?.RegisterDamage(amount);

        // Handle shield hits first
        var stats = GetComponent<PlayerStats>();
        if (stats != null && stats.shieldHits > 0)
        {
            stats.shieldHits--;
            Debug.Log($"Shield blocked! Remaining: {stats.shieldHits}");
            return;
        }

        Debug.Log($"TakeDamage: before={currentHealth}, dmg={amount}");
        currentHealth -= amount;

        if (damageTextPrefab != null)
        {
            var txt = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            txt.Setup(amount);
        }

        UpdateUI();

        if (currentHealth <= 0)
            OnDeath();
    }

    void OnDeath()
    {
        Debug.Log("Player died → Game Over");
        GameOverManager.Instance?.GameOver();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Healed to {currentHealth}/{maxHealth}");
        UpdateUI();
    }

    void UpdateUI()
    {
        // left empty if your UIUpdater polls CurrentHealth
    }
}

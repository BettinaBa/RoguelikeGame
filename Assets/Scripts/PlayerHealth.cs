using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Base health of the player; will be overridden by any permanent upgrades.")]
    public int maxHealth = 5;
    private int currentHealth;

    [Tooltip("Prefab for floating damage text.")]
    public FloatingDamageText damageTextPrefab;

    /// <summary>
    /// Exposes current health for UI.
    /// </summary>
    public int CurrentHealth => currentHealth;

    void Start()
    {
        // Apply any permanent +MaxHP upgrades from PlayerPrefs:
        //   Meta_MaxHP key holds the actual max health (defaults to whatever you set in the inspector).
        maxHealth = PlayerPrefs.GetInt("Meta_MaxHP", maxHealth);

        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        Debug.Log($"TakeDamage called: HP before = {currentHealth}, damage = {amount}");
        currentHealth -= amount;
        Debug.Log("Player Health: " + currentHealth);
        if (damageTextPrefab != null)
        {
            var text = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            text.Setup(amount);
        }
        UpdateUI();

        if (currentHealth <= 0)
        {
            Debug.Log("Player Died! Calling OnDeath()");
            OnDeath();
        }
    }

    void OnDeath()
    {
        if (GameOverManager.Instance != null)
            GameOverManager.Instance.GameOver();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Player Health: " + currentHealth);
        UpdateUI();
    }

    void UpdateUI()
    {
        // Leave empty if your UIUpdater is polling CurrentHealth each frame
    }
}

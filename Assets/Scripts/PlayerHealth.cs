using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Maximum health of the player.")]
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
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
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
            Debug.Log("Player Died!");
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
        // You can fire events here or leave empty if UIUpdater polls in Update().
    }
}
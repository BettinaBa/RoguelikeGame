using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthBar;
    public Slider xpBar;

    [Header("Player References (optional)")]
    public PlayerHealth playerHealth;
    public PlayerExperience playerXP;

    void Awake()
    {
        // Auto-assign if you didn't wire them in the Inspector
        if (playerHealth == null)
            playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        if (playerXP == null)
            playerXP = Object.FindFirstObjectByType<PlayerExperience>();
    }

    void Start()
    {
        if (playerHealth != null && healthBar != null)
        {
            // ensure slider range is correct
            healthBar.minValue = 0;
            healthBar.maxValue = playerHealth.maxHealth;
            healthBar.value = playerHealth.CurrentHealth;
        }

        if (playerXP != null && xpBar != null)
        {
            xpBar.minValue = 0;
            xpBar.maxValue = playerXP.XPToNextLevel;
            xpBar.value = playerXP.CurrentXP;
        }
    }

    void Update()
    {
        if (playerHealth != null && healthBar != null)
            healthBar.value = playerHealth.CurrentHealth;

        if (playerXP != null && xpBar != null)
            xpBar.value = playerXP.CurrentXP;
    }
}
using System.Collections.Generic;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [Tooltip("Current XP accumulated")]
    [SerializeField] private int currentXP = 0;
    [Tooltip("XP required to reach next level")]
    [SerializeField] private int xpToLevel = 10;

    [Tooltip("Multiplier applied to all XP gained")]
    public float xpGainMultiplier = 1f;

    /// <summary>
    /// Exposes current XP for UI.
    /// </summary>
    public int CurrentXP => currentXP;

    /// <summary>
    /// Exposes XP required to next level for UI.
    /// </summary>
    public int XPToNextLevel => xpToLevel;

    void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// Adds experience points (after applying xpGainMultiplier).
    /// </summary>
    public void AddXP(int amount)
    {
        int gained = Mathf.RoundToInt(amount * xpGainMultiplier);
        currentXP += gained;

        if (currentXP >= xpToLevel && LevelUpManager.Instance != null)
        {
            currentXP -= xpToLevel;
            var options = GenerateLevelUpOptions();
            LevelUpManager.Instance.ShowLevelUpOptions(options);
        }

        UpdateUI();
    }

    /// <summary>
    /// Alias for AddXP, used by PickupCollector.
    /// </summary>
    public void AddExperience(int amount) => AddXP(amount);

    /// <summary>
    /// Hook for updating any XP UI. If you poll CurrentXP/XPToNextLevel, leave empty.
    /// </summary>
    private void UpdateUI() { }

    /// <summary>
    /// Builds three dummy upgrade descriptions.
    /// </summary>
    private List<string> GenerateLevelUpOptions()
    {
        return new List<string>
        {
            "+10% Damage",
            "+15% XP Gain",
            "+10% Move Speed"
        };
    }
}

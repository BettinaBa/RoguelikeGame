using System.Collections.Generic;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    [Tooltip("Current XP accumulated")]
    public int currentXP = 0;
    [Tooltip("XP required to reach next level")]
    public int xpToLevel = 10;

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
    /// Adds experience points.
    /// </summary>
    public void AddXP(int amount)
    {
        currentXP += amount;
        if (currentXP >= xpToLevel)
        {
            currentXP -= xpToLevel;
            List<string> options = GenerateLevelUpOptions();
            LevelUpManager.Instance.ShowLevelUpOptions(options);
        }
        UpdateUI();
    }

    /// <summary>
    /// Alias for AddXP, for compatibility with PickupCollector.
    /// </summary>
    public void AddExperience(int amount)
    {
        AddXP(amount);
    }

    void UpdateUI()
    {
        // You can fire events here or leave empty if UIUpdater polls in Update().
    }

    List<string> GenerateLevelUpOptions()
    {
        // Replace these with your real upgrade logic!
        return new List<string> {
            "+10% Damage",
            "+15% XP Gain",
            "+10% Move Speed"
        };
    }
}
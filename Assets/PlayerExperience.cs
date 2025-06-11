using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerExperience : MonoBehaviour
{
    public int currentXP = 0;
    public int xpToLevel = 10;

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
        // Update XP bar or text here
    }

    List<string> GenerateLevelUpOptions()
    {
        // Generate three dummy options, replace with real logic
        return new List<string> {
            "+10% Damage",
            "+15% XP Gain",
            "+10% Move Speed"
        };
    }
}

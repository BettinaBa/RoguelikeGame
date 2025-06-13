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

    // track upgrades already chosen so we don't offer them again
    private readonly HashSet<string> chosenUpgrades = new HashSet<string>();

    /// <summary>
    /// Record an upgrade choice so it won't be offered again.
    /// </summary>
    public void RegisterUpgrade(string name)
    {
        chosenUpgrades.Add(name);
    }

    // the full list of possible upgrades
    private static readonly List<string> allUpgrades = new List<string>
    {
        "+15% Critical Strike Chance",
        "+150% Critical Damage",
        "+10% Attack Speed",
        "Piercing Bullets",
        "+20% Health Regeneration",
        "On Hit: 10% Stun",
        "Unlock Dash Ability",
        "+1 Extra Projectile",
        "5% Life Steal",
        "Kill: AoE Blast",
        "+10% XP Gain",
        "Shield (2 hits)"
    };

    void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// Adds experience (after xpGainMultiplier), then levels up if needed.
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
    /// Compatibility alias.
    /// </summary>
    public void AddExperience(int amount) => AddXP(amount);

    private void UpdateUI() { /* if you poll CurrentXP/XPToNextLevel, leave blank */ }

    /// <summary>
    /// Uses RunMetrics to weight each upgrade, then samples 3 without replacement.
    /// </summary>
    private List<string> GenerateLevelUpOptions()
    {
        // 1) Finalize and fetch run metrics
        RunMetrics.Instance.FinalizeMetrics();
        var m = RunMetrics.Instance;

        float elapsedMin = Mathf.Max(1f, Time.timeSinceLevelLoad / 60f);
        float dpm = m.damageTaken / elapsedMin;  // damage per minute
        float kpm = m.kills / elapsedMin;  // kills per minute

        // 2) Build weighted list excluding already chosen upgrades
        var weighted = new List<(string name, float w)>();
        foreach (var name in allUpgrades)
        {
            if (chosenUpgrades.Contains(name))
                continue;

            float weight = 1f;

            // bias toward more HP if taking lots of damage
            if (name.Contains("Health") && dpm > 5f)
                weight += (dpm - 5f);

            // bias toward damage upgrades if you're killing slowly
            if ((name.Contains("Damage") || name.Contains("Crit")) && kpm < 2f)
                weight += (2f - kpm);

            // bias toward speed/dash if you're spending too long chasing
            if ((name.Contains("Dash") || name.Contains("Speed")) &&
                m.timeInChase > m.timeInAttack)
            {
                weight += (m.timeInChase - m.timeInAttack) * 0.5f;
            }

            weighted.Add((name, weight));
        }

        // 3) Sample three distinct upgrades
        var chosen = new List<string>();
        for (int pick = 0; pick < 3 && weighted.Count > 0; pick++)
        {
            // compute total weight
            float total = 0f;
            foreach (var pw in weighted) total += pw.w;

            // pick a random slot
            float r = Random.value * total;
            float accum = 0f;
            for (int i = 0; i < weighted.Count; i++)
            {
                accum += weighted[i].w;
                if (r <= accum)
                {
                    chosen.Add(weighted[i].name);
                    weighted.RemoveAt(i);
                    break;
                }
            }
        }

        return chosen;
    }
}

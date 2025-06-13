using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject levelUpPanel;        // Your panel GameObject
    public Transform buttonContainer;     // Container transform for option buttons
    public GameObject optionButtonPrefab; // A prefab with a Button + child Text

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public void ShowLevelUpOptions(List<string> options)
    {
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);

        // clear old
        foreach (Transform t in buttonContainer)
            Destroy(t.gameObject);

        // make new
        foreach (var opt in options)
        {
            string choice = opt; // capture for the lambda
            var go = Instantiate(optionButtonPrefab, buttonContainer);
            go.GetComponentInChildren<Text>().text = choice;
            go.GetComponent<Button>()
              .onClick.AddListener(() =>
              {
                  ApplyUpgrade(choice);
                  HideLevelUpOptions();
              });
        }
    }

    private void HideLevelUpOptions()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ApplyUpgrade(string choice)
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var stats = player.GetComponent<PlayerStats>();
        var xp = player.GetComponent<PlayerExperience>();
        var mv = player.GetComponent<PlayerMovement>();
        var ph = player.GetComponent<PlayerHealth>();

        switch (choice)
        {
            case "+1 Extra Projectile":
                stats.extraProjectiles++;
                break;

            case "+10% Attack Speed":
                stats.fireRateMultiplier *= 1.1f;
                break;

            case "+10% Damage":
                stats.damageMultiplier *= 1.1f;
                break;

            case "+15% Critical Strike Chance":
                stats.critChance += 0.15f;
                break;

            case "+150% Critical Damage":
                stats.critMultiplier += 1.5f;
                break;

            case "Piercing Bullets":
                stats.piercingBullets = true;
                break;

            case "+20% Health Regeneration":
                stats.healthRegenPerSec += ph.maxHealth * 0.20f;
                break;

            case "5% Life Steal":
                stats.lifeStealFraction += 0.05f;
                break;

            case "Shield (2 hits)":
                stats.shieldHits += 2;
                break;

            case "Unlock Dash Ability":
                stats.dashUnlocked = true;
                break;

            case "+10% XP Gain":
                xp.xpGainMultiplier *= 1.1f;
                break;

            case "+10% Move Speed":
                mv.moveSpeed *= 1.1f;
                break;

            case "Kill: AoE Blast":
            case "On Hit: 10% Stun":
                Debug.LogWarning($"Upgrade '{choice}' not implemented yet.");
                break;

            default:
                Debug.LogWarning($"Unknown upgrade: {choice}");
                break;
        }

        Debug.Log($"Applied upgrade: {choice}");
    }
}

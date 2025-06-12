using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance { get; private set; }

    [Header("UI References")]
    public GameObject levelUpPanel;       // Drag in your LevelUpPanel
    public Transform buttonContainer;    // Drag in your ButtonContainer (Transform)
    public GameObject optionButtonPrefab; // Drag in your OptionButton prefab

    // Local multipliers (reset each run)
    private float damageMultiplier = 1f;
    private float xpGainMultiplierLocal = 1f;
    private float speedMultiplierLocal = 1f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    /// <summary>
    /// Call to pause the game and show three choice buttons.
    /// </summary>
    public void ShowLevelUpOptions(List<string> options)
    {
        // Pause everything
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);

        // Clear old buttons
        foreach (Transform t in buttonContainer)
            Destroy(t.gameObject);

        // Create new buttons
        foreach (string opt in options)
        {
            var buttonGO = Instantiate(optionButtonPrefab, buttonContainer);
            buttonGO.GetComponentInChildren<Text>().text = opt;
            buttonGO.GetComponent<Button>()
                .onClick.AddListener(() =>
                {
                    ApplyUpgrade(opt);
                    HideLevelUpOptions();
                });
        }
    }

    /// <summary>
    /// Hide panel and resume the game.
    /// </summary>
    private void HideLevelUpOptions()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Parse the choice string and apply the corresponding buff.
    /// </summary>
    private void ApplyUpgrade(string choice)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (choice.Contains("Damage"))
        {
            damageMultiplier *= 1.1f;
            var atk = player.GetComponent<PlayerAttack>();
            if (atk != null)
                atk.damage = Mathf.RoundToInt(atk.damage * 1.1f);
        }
        else if (choice.Contains("XP Gain"))
        {
            xpGainMultiplierLocal *= 1.15f;
            var xp = player.GetComponent<PlayerExperience>();
            if (xp != null)
                xp.xpGainMultiplier = xpGainMultiplierLocal;
        }
        else if (choice.Contains("Move Speed"))
        {
            speedMultiplierLocal *= 1.1f;
            var mv = player.GetComponent<PlayerMovement>();
            if (mv != null)
                mv.moveSpeed *= 1.1f;
        }

        Debug.Log($"Applied upgrade [{choice}]. " +
                  $"DMG×{damageMultiplier:F2}, " +
                  $"XP×{xpGainMultiplierLocal:F2}, " +
                  $"SPD×{speedMultiplierLocal:F2}");
    }
}

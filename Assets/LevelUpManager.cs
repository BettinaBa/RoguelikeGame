using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance { get; private set; }

    public GameObject optionPanel;
    public Button optionButtonPrefab;
    public PlayerAttack playerAttack;
    public PlayerMovement playerMovement;
    public PlayerExperience playerExperience;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (optionPanel == null)
            CreateDefaultUI();

        if (optionPanel != null)
            optionPanel.SetActive(false);
    }

    void CreateDefaultUI()
    {
        var canvasGO = new GameObject("LevelUpCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        optionPanel = new GameObject("OptionsPanel");
        optionPanel.transform.SetParent(canvasGO.transform);
        var rt = optionPanel.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(200, 200);
        optionPanel.AddComponent<Image>();

        optionButtonPrefab = new GameObject("OptionButton").AddComponent<Button>();
        var txt = new GameObject("Text").AddComponent<Text>();
        txt.transform.SetParent(optionButtonPrefab.transform);
        txt.alignment = TextAnchor.MiddleCenter;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        optionButtonPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 40);
        optionButtonPrefab.gameObject.SetActive(false);
    }

    public void ShowLevelUpOptions()
    {
        if (optionPanel == null || optionButtonPrefab == null)
            return;

        optionPanel.SetActive(true);

        foreach (Transform child in optionPanel.transform)
            Destroy(child.gameObject);

        var upgrades = new List<(string label, System.Action apply)>
        {
            ("Increase Damage", () => { if (playerAttack != null) playerAttack.damage++; }),
            ("Increase XP Gain", () => { if (playerExperience != null) playerExperience.xpMultiplier++; }),
            ("Increase Move Speed", () => { if (playerMovement != null) playerMovement.moveSpeed += 1f; })
        };

        var chosen = new List<(string label, System.Action apply)>();
        while (chosen.Count < 3 && upgrades.Count > 0)
        {
            int index = Random.Range(0, upgrades.Count);
            chosen.Add(upgrades[index]);
            upgrades.RemoveAt(index);
        }

        foreach (var up in chosen)
        {
            var btn = Instantiate(optionButtonPrefab, optionPanel.transform);
            var text = btn.GetComponentInChildren<Text>();
            if (text != null) text.text = up.label;
            btn.onClick.AddListener(() => {
                up.apply();
                optionPanel.SetActive(false);
            });
        }
    }
}

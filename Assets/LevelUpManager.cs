using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpManager : MonoBehaviour
{
    public static LevelUpManager Instance { get; private set; }

    public GameObject levelUpPanel;
    public Transform buttonContainer;
    public GameObject levelUpButtonPrefab;
    public List<string> upgrades;

    private List<GameObject> currentButtons = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        CreateDefaultUI();
    }

    void CreateDefaultUI()
    {
        // Ensure we use a valid built-in font
        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // Create a simple panel if none assigned
        if (levelUpPanel == null)
        {
            levelUpPanel = new GameObject("LevelUpPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var panelImage = levelUpPanel.GetComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.5f);
            var rt = (RectTransform)levelUpPanel.transform;
            rt.SetParent(this.transform);
            rt.anchorMin = new Vector2(0.25f, 0.25f);
            rt.anchorMax = new Vector2(0.75f, 0.75f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        // Create a vertical layout group container for buttons
        if (buttonContainer == null)
        {
            GameObject container = new GameObject("ButtonContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
            buttonContainer = container.transform;
            var rt = (RectTransform)container.transform;
            rt.SetParent(levelUpPanel.transform);
            rt.anchorMin = new Vector2(0.1f, 0.1f);
            rt.anchorMax = new Vector2(0.9f, 0.9f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        // Create sample buttons
        foreach (var upgrade in upgrades)
        {
            GameObject btnObj = Instantiate(levelUpButtonPrefab, buttonContainer);
            Text btnText = btnObj.GetComponentInChildren<Text>();
            if (btnText != null)
            {
                btnText.font = defaultFont;
                btnText.text = upgrade;
            }
            currentButtons.Add(btnObj);
        }
    }

    public void ShowLevelUpOptions(List<string> options)
    {
        // Clear old buttons
        foreach (var btn in currentButtons)
            Destroy(btn);
        currentButtons.Clear();

        // Create new buttons for options
        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        foreach (var opt in options)
        {
            GameObject btnObj = Instantiate(levelUpButtonPrefab, buttonContainer);
            Text btnText = btnObj.GetComponentInChildren<Text>();
            if (btnText != null)
            {
                btnText.font = defaultFont;
                btnText.text = opt;
            }
            currentButtons.Add(btnObj);
        }

        levelUpPanel.SetActive(true);
    }

    public void HideLevelUpPanel()
    {
        levelUpPanel.SetActive(false);
    }
}

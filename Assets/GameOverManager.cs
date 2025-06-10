using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    public GameObject gameOverUI;
    public bool IsGameOver { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (gameOverUI == null)
            CreateDefaultUI();
        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    void CreateDefaultUI()
    {
        var canvasGO = new GameObject("GameOverCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        gameOverUI = new GameObject("GameOverText");
        gameOverUI.transform.SetParent(canvasGO.transform);
        var text = gameOverUI.AddComponent<Text>();
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.text = "Game Over";
        RectTransform rt = gameOverUI.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(200, 100);
    }

    public void GameOver()
    {
        if (IsGameOver)
            return;
        IsGameOver = true;
        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }
}

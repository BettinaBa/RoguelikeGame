using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("UI")]
    public GameObject gameOverPanel;     // Drag your GameOverPanel here
    public Text pupText;                 // Drag the PUP Text UI here
    public Text recommendationText;      // Drag RecommendationText here
    public Button restartButton;         // Drag RestartButton here
    public Button menuButton;            // Drag MenuButton here

    [Header("PUP Settings")]
    [Tooltip("Seconds of play required to earn 1 PUP")]
    public float secondsPerPUP = 90f;    // was 30f
    [Tooltip("Number of kills required to earn 1 PUP")]
    public int killsPerPUP = 25;     // was 10

    public bool IsGameOver { get; private set; } = false;
    private float startTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // Hide on start
        gameOverPanel.SetActive(false);
        recommendationText.gameObject.SetActive(false);

        startTime = Time.time;
        restartButton.onClick.AddListener(RestartGame);
        menuButton.onClick.AddListener(ReturnToMenu);
    }

    public void GameOver()
    {
        IsGameOver = true;
        Time.timeScale = 0f;

        // --- Calculate how many PUPs you earn ---
        float timeSurvived = Time.time - startTime;
        int kills = DifficultyManager.Instance?.EnemiesDefeated ?? 0;

        int timePUP = Mathf.FloorToInt(timeSurvived / secondsPerPUP);
        int killPUP = kills / killsPerPUP;
        int earned = timePUP + killPUP;

        // Save total PUPs
        int total = PlayerPrefs.GetInt("PUP", 0) + earned;
        PlayerPrefs.SetInt("PUP", total);
        PlayerPrefs.Save();

        // Update the Game-Over panel text
        pupText.text = $"You earned {earned} PUP(s)\nTotal: {total}";

        // Show a quick recommendation as before
        float killsPerMinute = kills / Mathf.Max(1f, timeSurvived / 60f);
        string rec;
        if (timeSurvived < secondsPerPUP * 2) rec = "+Health";
        else if (killsPerMinute < 1f) rec = "+Damage";
        else rec = "+Move Speed";

        recommendationText.text = $"Recommended next: {rec}";
        recommendationText.gameObject.SetActive(true);

        // Log run metrics before showing the panel
        RunMetricsLogger.WriteMetrics(earned);

        // Show the panel
        gameOverPanel.SetActive(true);
    }

    void RestartGame()
    {
        IsGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ReturnToMenu()
    {
        IsGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

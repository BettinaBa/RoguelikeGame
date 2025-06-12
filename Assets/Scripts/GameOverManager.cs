using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("UI")]
    public GameObject gameOverPanel;  // Drag “GameOverPanel” here
    public Text pupText;        // Drag “PUPText” here
    public Button restartButton;  // Drag “RestartButton” here
    public Button menuButton;     // Drag “MenuButton” here

    [Header("PUP Settings")]
    public float secondsPerPUP = 30f;
    public int killsPerPUP = 10;

    public bool IsGameOver { get; private set; } = false;
    private float startTime;

    void Awake()
    {
        // Singleton—but do NOT persist across scenes
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Hide panel at start
        gameOverPanel.SetActive(false);

        startTime = Time.time;
        restartButton.onClick.AddListener(RestartGame);
        menuButton.onClick.AddListener(ReturnToMenu);
    }

    public void GameOver()
    {
        IsGameOver = true;
        Time.timeScale = 0f;

        // compute PUPs
        int timePUP = Mathf.FloorToInt((Time.time - startTime) / secondsPerPUP);
        int killPUP = DifficultyManager.Instance != null
                     ? DifficultyManager.Instance.EnemiesDefeated / killsPerPUP
                     : 0;
        int earned = timePUP + killPUP;

        // save
        int total = PlayerPrefs.GetInt("PUP", 0) + earned;
        PlayerPrefs.SetInt("PUP", total);
        PlayerPrefs.Save();

        // update UI & show
        pupText.text = $"You earned {earned} PUP(s)\nTotal: {total}";
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

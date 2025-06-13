using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [Tooltip("Drag your TimerText UI element here (UI→Text).")]
    public Text timerText;

    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        // don’t tick during Game Over
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        float elapsed = Time.time - startTime;
        int minutes = (int)(elapsed / 60f);
        int seconds = (int)(elapsed % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}

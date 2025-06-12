using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MetaUpgradeManager : MonoBehaviour
{
    public Text pupDisplay;
    public Button playButton;
    public Button quitButton;

    void Start()
    {
        RefreshPUP();
        playButton.onClick.AddListener(OnPlay);
        quitButton.onClick.AddListener(OnQuit);
    }

    void RefreshPUP()
    {
        int pup = PlayerPrefs.GetInt("PUP", 0);
        pupDisplay.text = $"PUPs: {pup}";
    }

    void OnPlay()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void OnQuit()
    {
        Application.Quit();
    }
}

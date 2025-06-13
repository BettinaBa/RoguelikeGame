using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MetaUpgradeManager : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;

    void Start()
    {
        playButton.onClick.AddListener(OnPlay);
        quitButton.onClick.AddListener(OnQuit);
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

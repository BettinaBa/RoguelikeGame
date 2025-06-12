using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuActions : MonoBehaviour
{
    public void PlayGame() => SceneManager.LoadScene("SampleScene");
    public void OpenUpgrades() => SceneManager.LoadScene("UpgradeMenu");
    public void QuitGame() => Application.Quit();
}
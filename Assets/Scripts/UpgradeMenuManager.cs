using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeMenuManager : MonoBehaviour
{
    [Header("UI")]
    public Text pupDisplay;
    public Button buyHPButton;
    public Button buySpeedButton;
    public Button backButton;

    const int hpCost = 1;
    const int speedCost = 1;

    void Start()
    {
        RefreshPUP();
        buyHPButton.onClick.AddListener(BuyMaxHP);
        buySpeedButton.onClick.AddListener(BuySpeed);
        backButton.onClick.AddListener(BackToMain);
    }

    void RefreshPUP()
    {
        int pup = PlayerPrefs.GetInt("PUP", 0);
        pupDisplay.text = $"PUPs: {pup}";
    }

    void BuyMaxHP()
    {
        int pup = PlayerPrefs.GetInt("PUP", 0);
        if (pup >= hpCost)
        {
            pup -= hpCost;
            PlayerPrefs.SetInt("PUP", pup);
            PlayerPrefs.SetInt("Meta_MaxHP",
                PlayerPrefs.GetInt("Meta_MaxHP", 5) + 1);
            PlayerPrefs.Save();
            RefreshPUP();
        }
    }

    void BuySpeed()
    {
        int pup = PlayerPrefs.GetInt("PUP", 0);
        if (pup >= speedCost)
        {
            pup -= speedCost;
            PlayerPrefs.SetInt("PUP", pup);
            PlayerPrefs.SetFloat("Meta_SpeedMult",
                PlayerPrefs.GetFloat("Meta_SpeedMult", 1f) * 1.1f);
            PlayerPrefs.Save();
            RefreshPUP();
        }
    }

    void BackToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
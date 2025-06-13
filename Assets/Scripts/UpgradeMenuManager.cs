using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeMenuManager : MonoBehaviour
{
    [Header("UI")]
    public Text pupDisplay;     // your PUP counter
    public Button buyHPButton;
    public Button buySpeedButton;
    public Button resetButton;     // ← new
    public Button backButton;

    // costs
    const int hpCost = 1;
    const int speedCost = 1;

    // “factory defaults” for your meta-upgrades
    const int defaultMetaMaxHP = 5;
    const float defaultMetaSpeedMult = 1f;

    void Start()
    {
        RefreshPUP();

        buyHPButton.onClick.AddListener(BuyMaxHP);
        buySpeedButton.onClick.AddListener(BuySpeed);
        resetButton.onClick.AddListener(ResetUpgrades);   // ← hook up
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

            // bump stored max-HP
            int prev = PlayerPrefs.GetInt("Meta_MaxHP", defaultMetaMaxHP);
            PlayerPrefs.SetInt("Meta_MaxHP", prev + 1);

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

            // multiply stored speed
            float prev = PlayerPrefs.GetFloat("Meta_SpeedMult", defaultMetaSpeedMult);
            PlayerPrefs.SetFloat("Meta_SpeedMult", prev * 1.1f);

            PlayerPrefs.Save();
            RefreshPUP();
        }
    }

    void ResetUpgrades()
    {
        // 🧹 wipe everything back to your “fresh install” defaults
        PlayerPrefs.SetInt("Meta_MaxHP", defaultMetaMaxHP);
        PlayerPrefs.SetFloat("Meta_SpeedMult", defaultMetaSpeedMult);
        PlayerPrefs.Save();

        RefreshPUP();
    }

    void BackToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UpgradeMenuManager : MonoBehaviour
{
    [Header("UI")]
    public Text pupDisplay;      // your PUP counter
    public Button buyHPButton;
    public Button buySpeedButton;
    public Button resetButton;     // your Reset button
    public Button backButton;

    // costs per purchase
    const int hpCost = 1;
    const int speedCost = 1;

    // factory defaults
    const int defaultMetaMaxHP = 5;
    const float defaultMetaSpeedMult = 1f;

    void Start()
    {
        RefreshPUP();
        buyHPButton.onClick.AddListener(BuyMaxHP);
        buySpeedButton.onClick.AddListener(BuySpeed);
        resetButton.onClick.AddListener(ResetUpgrades);
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
        if (pup < hpCost) return;

        // spend 1 PUP
        pup -= hpCost;
        PlayerPrefs.SetInt("PUP", pup);

        // bump stored max HP
        int prevHP = PlayerPrefs.GetInt("Meta_MaxHP", defaultMetaMaxHP);
        PlayerPrefs.SetInt("Meta_MaxHP", prevHP + 1);

        // record that we bought one more HP‐upgrade
        int hpCount = PlayerPrefs.GetInt("Meta_HPCount", 0);
        PlayerPrefs.SetInt("Meta_HPCount", hpCount + 1);

        PlayerPrefs.Save();
        RefreshPUP();
    }

    void BuySpeed()
    {
        int pup = PlayerPrefs.GetInt("PUP", 0);
        if (pup < speedCost) return;

        // spend 1 PUP
        pup -= speedCost;
        PlayerPrefs.SetInt("PUP", pup);

        // multiply stored speed
        float prevMult = PlayerPrefs.GetFloat("Meta_SpeedMult", defaultMetaSpeedMult);
        PlayerPrefs.SetFloat("Meta_SpeedMult", prevMult * 1.1f);

        // record one more Speed‐upgrade purchased
        int spdCount = PlayerPrefs.GetInt("Meta_SpeedCount", 0);
        PlayerPrefs.SetInt("Meta_SpeedCount", spdCount + 1);

        PlayerPrefs.Save();
        RefreshPUP();
    }

    void ResetUpgrades()
    {
        // figure out how many total PUPs were spent:
        int hpCount = PlayerPrefs.GetInt("Meta_HPCount", 0);
        int spdCount = PlayerPrefs.GetInt("Meta_SpeedCount", 0);
        int refund = hpCount + spdCount;

        // refund them
        int pup = PlayerPrefs.GetInt("PUP", 0);
        pup += refund;
        PlayerPrefs.SetInt("PUP", pup);

        // wipe meta‐upgrades back to defaults
        PlayerPrefs.SetInt("Meta_MaxHP", defaultMetaMaxHP);
        PlayerPrefs.SetFloat("Meta_SpeedMult", defaultMetaSpeedMult);

        // reset our purchase counters
        PlayerPrefs.SetInt("Meta_HPCount", 0);
        PlayerPrefs.SetInt("Meta_SpeedCount", 0);

        PlayerPrefs.Save();
        RefreshPUP();
    }

    void BackToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

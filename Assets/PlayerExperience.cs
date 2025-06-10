using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerExperience : MonoBehaviour
{
    public int level = 1;
    public int experience;
    public int experienceToNextLevel = 5;
    public float xpMultiplier = 1f;

    private bool choosingUpgrade;
    private List<Upgrade> upgrades;
    private List<Upgrade> currentChoices;

    private PlayerAttack attack;
    private PlayerMovement movement;

    void Start()
    {
        attack = GetComponent<PlayerAttack>();
        movement = GetComponent<PlayerMovement>();
        SetupUpgrades();
    }

    void SetupUpgrades()
    {
        upgrades = new List<Upgrade>
        {
            new Upgrade {
                description = "Increase Damage",
                action = () => { if (attack != null) attack.damage += 1; }
            },
            new Upgrade {
                description = "Increase XP Gain",
                action = () => { xpMultiplier += 0.5f; }
            },
            new Upgrade {
                description = "Increase Move Speed",
                action = () => { if (movement != null) movement.moveSpeed += 1f; }
            }
        };
    }

    public void AddExperience(int amount)
    {
        if (choosingUpgrade)
            return;
        experience += Mathf.RoundToInt(amount * xpMultiplier);
        if (experience >= experienceToNextLevel)
        {
            experience -= experienceToNextLevel;
            level++;
            experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.5f);
            ShowUpgradeOptions();
        }
    }

    void ShowUpgradeOptions()
    {
        choosingUpgrade = true;
        Time.timeScale = 0f;
        currentChoices = new List<Upgrade>();
        List<int> pool = new List<int>();
        for (int i = 0; i < upgrades.Count; i++)
            pool.Add(i);
        for (int i = 0; i < 3 && pool.Count > 0; i++)
        {
            int idx = UnityEngine.Random.Range(0, pool.Count);
            currentChoices.Add(upgrades[pool[idx]]);
            pool.RemoveAt(idx);
        }
    }

    void ApplyUpgrade(int index)
    {
        if (currentChoices == null || index >= currentChoices.Count)
            return;
        currentChoices[index].action.Invoke();
        choosingUpgrade = false;
        Time.timeScale = 1f;
    }

    void OnGUI()
    {
        if (!choosingUpgrade)
            return;
        int width = 200;
        int height = 40;
        for (int i = 0; i < currentChoices.Count; i++)
        {
            if (GUI.Button(new Rect(10, 10 + i * (height + 10), width, height), currentChoices[i].description))
            {
                ApplyUpgrade(i);
            }
        }
    }

    class Upgrade
    {
        public string description;
        public Action action;
    }
}

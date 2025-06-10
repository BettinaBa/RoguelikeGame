using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 5;
    public int xpMultiplier = 1;

    public void AddExperience(int amount)
    {
        experience += amount * xpMultiplier;
        Debug.Log("Player XP: " + experience);

        if (experience >= experienceToNextLevel)
        {
            if (LevelUpManager.Instance != null)
                LevelUpManager.Instance.ShowLevelUpOptions();

            experience -= experienceToNextLevel;
            level++;
            experienceToNextLevel += 5;
        }
    }
}

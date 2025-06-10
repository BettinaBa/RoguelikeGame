using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    public int experience;

    public void AddExperience(int amount)
    {
        experience += amount;
        Debug.Log("Player XP: " + experience);
    }
}

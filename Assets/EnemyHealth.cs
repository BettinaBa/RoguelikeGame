using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public int experienceReward = 1;
    public FloatingDamageText damageTextPrefab;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Enemy Health: " + currentHealth);
        if (damageTextPrefab != null)
        {
            var text = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
            text.Setup(amount);
        }

        if (currentHealth <= 0)
        {
            PlayerExperience pe = FindAnyObjectByType<PlayerExperience>();
            if (pe != null)
                pe.AddExperience(experienceReward);

            Destroy(gameObject);
            if (DifficultyManager.Instance != null)
                DifficultyManager.Instance.RegisterKill();
            RoomGenerator rg = FindAnyObjectByType<RoomGenerator>();
            if (rg != null)
                rg.RegenerateRoom();
        }
    }
}
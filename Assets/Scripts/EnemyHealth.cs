using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public int experienceReward = 1;
    public FloatingDamageText damageTextPrefab;
    public GameObject xpPickupPrefab;  // assign your XP_Pickup prefab here

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
            // Spawn XP pickup
            if (xpPickupPrefab != null)
            {
                GameObject pickupObj = Instantiate(
                    xpPickupPrefab,
                    transform.position,
                    Quaternion.identity
                );
                // Set amount on the Pickup component
                var pickup = pickupObj.GetComponent<Pickup>();
                if (pickup != null)
                    pickup.amount = experienceReward;
            }

            Destroy(gameObject);

            if (DifficultyManager.Instance != null)
                DifficultyManager.Instance.RegisterKill();

            // Regenerate room if using standalone RoomGenerator
            RoomGenerator rg = FindAnyObjectByType<RoomGenerator>();
            if (rg != null)
                rg.RegenerateRoom();
        }
    }
}

using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Reward Pickups")]
    [Tooltip("How much XP this enemy gives")]
    public int experienceReward = 1;
    [Tooltip("Prefab for the XP orb")]
    public GameObject xpPickupPrefab;
    [Tooltip("Prefab for the health pickup (e.g. heart)")]
    public GameObject healthPickupPrefab;
    [Range(0f, 1f), Tooltip("Chance to drop health instead of XP")]
    public float healthDropChance = 0.2f;

    [Header("Feedback")]
    public FloatingDamageText damageTextPrefab;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Enemy Health: {currentHealth}");

        // Show floating damage text
        if (damageTextPrefab != null)
        {
            var text = Instantiate(
                damageTextPrefab,
                transform.position,
                Quaternion.identity);
            text.Setup(amount);
        }

        if (currentHealth <= 0)
        {
            SpawnPickup();
            Destroy(gameObject);

            if (DifficultyManager.Instance != null)
                DifficultyManager.Instance.RegisterKill();

            // If you’re using RoomGenerator directly:
            var rg = FindAnyObjectByType<RoomGenerator>();
            if (rg != null)
                rg.RegenerateRoom();
        }
    }

    private void SpawnPickup()
    {
        // Randomly choose heart or XP
        if (Random.value < healthDropChance && healthPickupPrefab != null)
        {
            Instantiate(
                healthPickupPrefab,
                transform.position,
                Quaternion.identity);
        }
        else if (xpPickupPrefab != null)
        {
            var pickupObj = Instantiate(
                xpPickupPrefab,
                transform.position,
                Quaternion.identity);
            var pickup = pickupObj.GetComponent<Pickup>();
            if (pickup != null)
                pickup.amount = experienceReward;
        }
    }
}

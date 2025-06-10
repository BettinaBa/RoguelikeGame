using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerExperience))]
public class PickupCollector : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerExperience playerXP;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerXP = GetComponent<PlayerExperience>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Pickup pickup = other.GetComponent<Pickup>();
        if (pickup == null)
            return;

        switch (pickup.type)
        {
            case Pickup.PickupType.Health:
                if (playerHealth != null)
                    playerHealth.Heal(pickup.amount);
                break;
            case Pickup.PickupType.XP:
                if (playerXP != null)
                    playerXP.AddExperience(pickup.amount);
                break;
        }

        Destroy(other.gameObject);
    }
}

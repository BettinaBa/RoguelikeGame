using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LootItem : MonoBehaviour
{
    [Tooltip("Bonus damage multiplier added to player stats")] public float damageBonus;
    [Tooltip("Bonus fire rate multiplier added to player stats")] public float speedBonus;
    [Tooltip("Name of special effect this item grants")] public string specialEffect;
    public string[] synergyTags = new string[0];

    public void Setup(float dmg, float spd, string effect, string[] tags)
    {
        damageBonus = dmg;
        speedBonus = spd;
        specialEffect = effect;
        synergyTags = tags ?? new string[0];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var stats = other.GetComponent<PlayerStats>();
        if (stats != null)
        {
            stats.damageMultiplier += damageBonus;
            stats.fireRateMultiplier += speedBonus;
            if (!string.IsNullOrEmpty(specialEffect))
            {
                switch (specialEffect)
                {
                    case "Piercing":
                        stats.piercingBullets = true;
                        break;
                    case "Stun":
                        stats.stunChance += 0.1f;
                        break;
                }
            }
        }

        if (LootSynergyManager.Instance != null)
            LootSynergyManager.Instance.RegisterTags(synergyTags);

        Destroy(gameObject);
    }
}

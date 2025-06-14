using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Tooltip("Base damage BEFORE multipliers")]
    public int damage = 1;

    // reference back to the player that fired this bullet
    public PlayerHealth owner;

    [HideInInspector] public float critChance = 0f;
    [HideInInspector] public float critMultiplier = 1f;
    [HideInInspector] public float lifeStealFrac = 0f;
    [HideInInspector] public float stunChance = 0f;
    [HideInInspector] public bool piercing = false;
    [HideInInspector] public int shieldHits = 0;

    [Tooltip("Seconds before bullet auto-destructs")]
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        var eh = other.GetComponent<EnemyHealth>();
        if (eh == null) return;

        // 1) Calculate crit
        int finalDmg = damage;
        if (Random.value < critChance)
            finalDmg = Mathf.RoundToInt(damage * critMultiplier);

        // 2) Deal damage
        eh.TakeDamage(finalDmg);

        // 3) Life steal
        if (lifeStealFrac > 0f)
            owner?.Heal(Mathf.RoundToInt(finalDmg * lifeStealFrac));

        // 4) Chance to stun
        if (stunChance > 0f && Random.value < stunChance)
        {
            var ai = other.GetComponent<EnemyAI>();
            ai?.ApplyStun(0.5f);
        }

        // 5) ShieldHits on player bullets (rarely used)
        if (shieldHits > 0)
        {
            shieldHits--;
            return;
        }

        // 6) Destroy if not piercing
        if (!piercing)
            Destroy(gameObject);
    }
}

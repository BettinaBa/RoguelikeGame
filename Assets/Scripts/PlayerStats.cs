using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Projectile")]
    public int extraProjectiles = 0;     // +1 Extra Projectile
    public bool piercingBullets = false; // Piercing Bullets
    public int shieldHits = 0;           // Shield (2 hits)

    [Header("Damage / Crit")]
    public float damageMultiplier = 1f;  // +10% Damage
    public float critChance = 0f;        // +15% Crit Chance
    public float critMultiplier = 2f;    // +150% Crit Damage
    public float lifeStealFraction = 0f; // 5% Life Steal
    public float stunChance = 0f;        // Chance to stun on hit

    [Header("Fire Rate")]
    public float fireRateMultiplier = 1f; // +10% Attack Speed

    [Header("Dash / Regen")]
    public bool dashUnlocked = false;     // Unlock Dash Ability
    public float healthRegenPerSec = 0f;  // +20% Health Regen

    // (Optional) you can tick “Apply regen per second” in Update elsewhere
}

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;

    [Header("Shooting Settings")]
    public float baseFireRate = 3f;
    public float bulletSpeed = 15f;
    public float spawnOffset = 0.6f;
    [Tooltip("Spread angle between extra projectiles")]
    public float spreadAngle = 15f;

    float nextFireTime;
    PlayerStats stats;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        var mouse = Mouse.current;
        if (mouse != null && mouse.rightButton.isPressed)
        {
            float effectiveRate = baseFireRate * (stats?.fireRateMultiplier ?? 1f);
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / effectiveRate;
                ShootTowardsMouse();
            }
        }
    }

    void ShootTowardsMouse()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;
        Vector2 dir = ((Vector2)mouseWorld - (Vector2)transform.position).normalized;

        // how many bullets?
        int count = 1 + (stats?.extraProjectiles ?? 0);
        float halfSpread = spreadAngle * (count - 1) * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float angle = -halfSpread + spreadAngle * i;
            Quaternion rot = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f + angle);
            Vector3 spawnPos = transform.position + rot * Vector3.up * spawnOffset;
            spawnPos.z = 2f;

            GameObject b = Instantiate(bulletPrefab, spawnPos, rot);
            var rb = b.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = rot * Vector3.up * bulletSpeed;

            // pass stats into the bullet
            var pb = b.GetComponent<PlayerBullet>();
            if (pb != null && stats != null)
            {
                pb.damage = Mathf.RoundToInt(pb.damage * stats.damageMultiplier);
                pb.critChance = stats.critChance;
                pb.critMultiplier = stats.critMultiplier;
                pb.lifeStealFrac = stats.lifeStealFraction;
                pb.stunChance = stats.stunChance;
                pb.piercing = stats.piercingBullets;
                pb.shieldHits = stats.shieldHits;
            }
        }
    }
}

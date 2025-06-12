using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Bullet prefab for the player")]
    public GameObject bulletPrefab;

    [Header("Shooting Settings")]
    [Tooltip("Shots per second")]
    public float fireRate = 3f;
    [Tooltip("Speed of the player bullet")]
    public float bulletSpeed = 15f;
    [Tooltip("How far from the player to spawn the bullet")]
    public float spawnOffset = 0.6f;

    private float nextFireTime;

    void Update()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        // Right-click (1) to shoot
        if (Input.GetMouseButton(1) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            ShootTowardsMouse();
        }
    }

    private void ShootTowardsMouse()
    {
        Debug.Log("Shoot!");

        // Get the mouse world position
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Direction from player to mouse
        Vector2 dir = ((Vector2)mouseWorld - (Vector2)transform.position).normalized;

        // Compute spawn position just outside the player
        Vector3 spawnPos = transform.position + (Vector3)(dir * spawnOffset);

        // **Force Z to 2 so it's drawn over floor (Z=1)**
        spawnPos.z = 2f;

        // Instantiate and orient the bullet
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, dir);
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, rot);

        // Give it velocity
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * bulletSpeed;
    }
}

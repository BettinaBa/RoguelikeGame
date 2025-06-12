using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShooterAI : MonoBehaviour
{
    [Tooltip("Player transform (auto-found at Start)")]
    public Transform player;

    [Tooltip("Prefab for the bullet")]
    public GameObject bulletPrefab;

    [Tooltip("Shots per second")]
    public float fireRate = 1f;

    [Tooltip("Maximum distance at which to fire")]
    public float shootRange = 8f;

    // How far from the shooter’s center to spawn the bullet (just outside its collider)
    private const float spawnOffset = 1.2f;

    private float nextFireTime;

    void Start()
    {
        // Find the player by tag at runtime
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= shootRange && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            ShootAt(player.position);
        }
    }

    private void ShootAt(Vector3 target)
    {
        // Compute direction toward the target
        Vector2 dir = (target - transform.position).normalized;

        // Rotate the bullet so its "up" faces the player
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, dir);

        // Spawn the bullet just outside the shooter’s own collider
        Vector3 spawnPos = (Vector2)transform.position + dir * spawnOffset;

        // Instantiate and orient
        Instantiate(bulletPrefab, spawnPos, rot);
    }
}

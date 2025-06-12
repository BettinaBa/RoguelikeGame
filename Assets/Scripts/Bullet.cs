using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Tooltip("Speed in units per second")]
    public float speed = 10f;

    [Tooltip("Damage dealt to the player on hit")]
    public int damage = 1;

    [Tooltip("Seconds before the bullet auto-destroys")]
    public float lifetime = 5f;

    void Start()
    {
        // Automatically remove the bullet after its lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Move forward in world space along the bullet’s up-vector
        transform.position += transform.up * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only destroy and damage on hitting the player
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

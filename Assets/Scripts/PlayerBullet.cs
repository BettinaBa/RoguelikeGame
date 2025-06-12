using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Tooltip("Seconds before auto-destroy")]
    public float lifetime = 3f;
    [Tooltip("Damage applied to enemies")]
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only hit objects tagged "Enemy"
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

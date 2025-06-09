using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float detectionRange = 4f;
    public float wanderChangeInterval = 2f;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 wanderDirection;
    private float wanderTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (detectionRange <= 0f)
            detectionRange = 4f;
        if (wanderChangeInterval <= 0f)
            wanderChangeInterval = 2f;

        wanderTimer = wanderChangeInterval;
        wanderDirection = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(player.position, transform.position);
            if (distance <= detectionRange)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                movement = direction;
            }
            else
            {
                wanderTimer -= Time.deltaTime;
                if (wanderTimer <= 0f)
                {
                    wanderDirection = Random.insideUnitCircle.normalized;
                    wanderTimer = wanderChangeInterval;
                }
                movement = wanderDirection;
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(1);
            }
        }
    }
}

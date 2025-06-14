using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TeleportingEnemyAI : MonoBehaviour
{
    [Header("Teleport Settings")]
    public float teleportInterval = 3f;
    public float teleportRange = 5f;

    [Header("Combat")]
    public float attackCooldown = 1f;
    public int damage = 1;
    public LayerMask playerLayer;

    private Transform player;
    private float nextTeleportTime;
    private float lastAttackTime;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        nextTeleportTime = Time.time + teleportInterval;
    }

    void Update()
    {
        if (player == null) return;
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        if (Time.time >= nextTeleportTime)
        {
            TeleportNearPlayer();
            nextTeleportTime = Time.time + teleportInterval;
        }
    }

    void TeleportNearPlayer()
    {
        Vector2 offset = Random.insideUnitCircle.normalized * teleportRange;
        rb.position = (Vector2)player.position + offset;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        TryDealMelee(col.collider);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        TryDealMelee(col.collider);
    }

    void TryDealMelee(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & playerLayer) == 0)
            return;
        if (Time.time < lastAttackTime + attackCooldown)
            return;
        lastAttackTime = Time.time;

        var ph = col.GetComponent<PlayerHealth>();
        if (ph != null)
            ph.TakeDamage(damage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, teleportRange);
    }
}

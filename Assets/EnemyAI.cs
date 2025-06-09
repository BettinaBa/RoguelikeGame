using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public float chaseRange = 5f;
    public float attackRange = 1f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1f;

    private Transform player;
    private Rigidbody2D rb;
    private float lastAttackTime;

    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(player.position, transform.position);
        switch (currentState)
        {
            case State.Idle:
                if (distance <= chaseRange)
                    currentState = State.Chase;
                break;
            case State.Chase:
                if (distance <= attackRange)
                    currentState = State.Attack;
                else if (distance > chaseRange)
                    currentState = State.Idle;
                break;
            case State.Attack:
                if (distance > attackRange)
                    currentState = State.Chase;
                break;
        }
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        switch (currentState)
        {
            case State.Chase:
                Vector2 dir = ((Vector2)player.position - rb.position).normalized;
                rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
                break;
            case State.Attack:
                TryAttack();
                break;
        }
    }

    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth ph = hit.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(1);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

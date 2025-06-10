using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public float chaseRange = 5f;
    public float attackRange = 1f;
    public float moveSpeed = 2f;
    public float attackCooldown = 1f;
    public int damage = 1;
    public float patrolChangeInterval = 2f;

    private Transform player;
    private Rigidbody2D rb;
    private float lastAttackTime;

    private enum State { Idle, Patrol, Chase, Attack }
    private State currentState = State.Idle;
    private Vector2 patrolDirection;
    private float nextPatrolChange;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        ChooseNewPatrolDirection();
    }

    void Update()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;
        if (player == null)
            return;

        float distance = Vector2.Distance(player.position, transform.position);
        switch (currentState)
        {
            case State.Idle:
                currentState = State.Patrol;
                break;
            case State.Patrol:
                if (distance <= chaseRange)
                    currentState = State.Chase;
                if (Time.time >= nextPatrolChange)
                    ChooseNewPatrolDirection();
                break;
            case State.Chase:
                if (distance <= attackRange)
                    currentState = State.Attack;
                else if (distance > chaseRange)
                    currentState = State.Patrol;
                break;
            case State.Attack:
                if (distance > attackRange)
                    currentState = State.Chase;
                break;
        }
    }

    void FixedUpdate()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;
        if (player == null)
            return;

        switch (currentState)
        {
            case State.Patrol:
                rb.MovePosition(rb.position + patrolDirection * moveSpeed * Time.fixedDeltaTime);
                break;
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
                    ph.TakeDamage(damage);
            }
        }
    }

    void ChooseNewPatrolDirection()
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        patrolDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        nextPatrolChange = Time.time + patrolChangeInterval;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

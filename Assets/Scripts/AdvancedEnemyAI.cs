using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class AdvancedEnemyAI : MonoBehaviour
{
    [Header("Movement & Detection")]
    public float chaseRange = 8f;
    public float attackRange = 1.5f;
    public float retreatDistance = 3f;
    public float moveSpeed = 2f;
    public float patrolChangeInterval = 2f;
    public float idleDuration = 2f;
    public float retreatDuration = 1f;

    [Header("Combat")]
    public float attackCooldown = 1f;
    public int damage = 1;
    public LayerMask playerLayer;

    Rigidbody2D rb;

    Transform player;
    Vector2 patrolDir;
    float lastAttackTime;
    float stateEndTime;

    enum State { Idle, Patrol, Chase, Attack, Retreat }
    State currentState = State.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        stateEndTime = Time.time + idleDuration;
    }

    void Update()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver) return;
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                if (dist <= chaseRange)
                    EnterState(State.Chase);
                else if (Time.time >= stateEndTime)
                    EnterState(State.Patrol);
                break;

            case State.Patrol:
                if (dist <= chaseRange)
                    EnterState(State.Chase);
                else if (Time.time >= stateEndTime)
                    EnterState(State.Idle);
                break;

            case State.Chase:
                if (dist <= attackRange)
                    EnterState(State.Attack);
                else if (dist > chaseRange)
                    EnterState(State.Patrol);
                else
                {
                    Vector2 dir = ((Vector2)player.position - rb.position).normalized;
                    rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
                }
                break;

            case State.Attack:
                PerformAttack(dist);
                EnterState(State.Retreat);
                break;

            case State.Retreat:
                if (Time.time >= stateEndTime)
                {
                    if (dist > chaseRange)
                        EnterState(State.Patrol);
                    else
                        EnterState(State.Chase);
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (currentState == State.Patrol)
        {
            rb.MovePosition(rb.position + patrolDir * moveSpeed * Time.fixedDeltaTime);
        }
        else if (currentState == State.Retreat)
        {
            Vector2 dir = ((Vector2)transform.position - (Vector2)player.position).normalized;
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void EnterState(State newState)
    {
        currentState = newState;
        switch (newState)
        {
            case State.Idle:
                stateEndTime = Time.time + idleDuration;
                break;
            case State.Patrol:
                stateEndTime = Time.time + patrolChangeInterval;
                ChooseNewPatrolDirection();
                break;
            case State.Chase:
                break;
            case State.Attack:
                break;
            case State.Retreat:
                stateEndTime = Time.time + retreatDuration;
                break;
        }
    }

    void ChooseNewPatrolDirection()
    {
        float ang = Random.Range(0f, Mathf.PI * 2f);
        patrolDir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
    }

    void PerformAttack(float dist)
    {
        if (Time.time < lastAttackTime + attackCooldown) return;
        if (dist > attackRange + 0.1f) return;
        lastAttackTime = Time.time;
        if (player.TryGetComponent<PlayerHealth>(out var ph))
            ph.TakeDamage(damage);
    }
}

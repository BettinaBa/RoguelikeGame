using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Movement & Detection")]
    public float chaseRange = 8f;
    public float moveSpeed = 2f;
    public float patrolChangeInterval = 2f;

    [Header("Combat")]
    public float attackCooldown = 1f;
    public int damage = 1;

    [Header("Collision Layers")]
    public LayerMask playerLayer;  // set this to only include your Player layer

    Rigidbody2D rb;
    Transform player;
    Vector2 patrolDir;
    float nextPatrolChange;
    float lastAttackTime;
    float stunTimer;

    enum State { Patrol, Chase }
    State currentState = State.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ChooseNewPatrolDirection();
    }

    void Update()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver) return;
        if (player == null) return;

        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            return;
        }

        float dist = Vector2.Distance(player.position, transform.position);

        // switch states
        if (currentState == State.Patrol && dist <= chaseRange)
            currentState = State.Chase;
        else if (currentState == State.Chase && dist > chaseRange)
            currentState = State.Patrol;

        // record chase time
        if (currentState == State.Chase)
            RunMetrics.Instance?.RecordChase(Time.deltaTime);

        // change patrol direction occasionally
        if (currentState == State.Patrol && Time.time >= nextPatrolChange)
            ChooseNewPatrolDirection();
    }

    void FixedUpdate()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver) return;
        if (player == null) return;

        if (stunTimer > 0f)
            return;

        if (currentState == State.Patrol)
            rb.MovePosition(rb.position + patrolDir * moveSpeed * Time.fixedDeltaTime);
        else // Chase
        {
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void ChooseNewPatrolDirection()
    {
        float ang = Random.Range(0f, Mathf.PI * 2f);
        patrolDir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
        nextPatrolChange = Time.time + patrolChangeInterval;
    }

    // Called once when we begin touching
    void OnCollisionEnter2D(Collision2D col)
    {
        TryDealMelee(col.collider);
    }

    // Called every physics frame while touching
    void OnCollisionStay2D(Collision2D col)
    {
        // record attack time while in contact
        if (((1 << col.gameObject.layer) & playerLayer) != 0)
            RunMetrics.Instance?.RecordAttack(Time.fixedDeltaTime);

        TryDealMelee(col.collider);
    }

    void TryDealMelee(Collider2D col)
    {
        // only if this collider is on our playerLayer
        if (((1 << col.gameObject.layer) & playerLayer) == 0)
            return;

        // enforce cooldown
        if (Time.time < lastAttackTime + attackCooldown)
            return;
        lastAttackTime = Time.time;

        // deal damage
        var ph = col.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            Debug.Log($"[{name}] melee hit for {damage}");
            ph.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    public void ApplyStun(float duration)
    {
        stunTimer = Mathf.Max(stunTimer, duration);
    }
}

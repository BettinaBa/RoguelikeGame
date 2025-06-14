using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(NavMeshAgent))]
[RequireComponent(typeof(BehaviorTreeRunner))]
public class BehaviorEnemyAI : MonoBehaviour
{
    [Header("Movement & Detection")]
    public float chaseRange = 8f;
    public float attackRange = 1.5f;
    public float retreatDuration = 1f;
    public float moveSpeed = 2f;
    public float patrolChangeInterval = 2f;

    [Header("Combat")]
    public float attackCooldown = 1f;
    public int damage = 1;
    public LayerMask playerLayer;

    Rigidbody2D rb;
    NavMeshAgent agent;
    Transform player;
    Vector2 patrolDir;
    float nextPatrolChange;
    float lastAttackTime;
    float retreatEnd;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = moveSpeed;
            agent.isStopped = true;
        }
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ChooseNewPatrolDirection();

        var runner = GetComponent<BehaviorTreeRunner>();
        runner.root = BuildTree();
    }

    BTNode BuildTree()
    {
        return new Sequence(new BTNode[] {
            new ActionNode(Patrol),
            new ActionNode(Chase),
            new ActionNode(Attack),
            new ActionNode(Retreat)
        });
    }

    BTNode.State Patrol()
    {
        if (player == null) return BTNode.State.Failure;
        float dist = Vector2.Distance(player.position, transform.position);
        if (dist <= chaseRange)
            return BTNode.State.Success;

        if (Time.time >= nextPatrolChange)
            ChooseNewPatrolDirection();

        if (agent != null)
            agent.isStopped = true;
        rb.MovePosition(rb.position + patrolDir * moveSpeed * Time.deltaTime);
        return BTNode.State.Running;
    }

    BTNode.State Chase()
    {
        if (player == null) return BTNode.State.Failure;
        float dist = Vector2.Distance(player.position, transform.position);
        if (dist > chaseRange)
            return BTNode.State.Failure;
        if (dist <= attackRange)
            return BTNode.State.Success;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
        }
        return BTNode.State.Running;
    }

    BTNode.State Attack()
    {
        if (player == null) return BTNode.State.Failure;
        float dist = Vector2.Distance(player.position, transform.position);
        if (dist > attackRange)
            return BTNode.State.Failure;
        if (Time.time < lastAttackTime + attackCooldown)
            return BTNode.State.Running;
        lastAttackTime = Time.time;
        if (player.TryGetComponent<PlayerHealth>(out var ph))
            ph.TakeDamage(damage);
        retreatEnd = Time.time + retreatDuration;
        return BTNode.State.Success;
    }

    BTNode.State Retreat()
    {
        if (player == null) return BTNode.State.Failure;
        if (Time.time >= retreatEnd)
            return BTNode.State.Success;
        if (agent != null)
            agent.isStopped = true;
        Vector2 dir = ((Vector2)transform.position - (Vector2)player.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
        return BTNode.State.Running;
    }

    void ChooseNewPatrolDirection()
    {
        float ang = Random.Range(0f, Mathf.PI * 2f);
        patrolDir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
        nextPatrolChange = Time.time + patrolChangeInterval;
    }
}

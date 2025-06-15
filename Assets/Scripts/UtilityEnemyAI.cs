using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public abstract class UtilityAction
{
    public abstract float Evaluate();
    public abstract void Act();
}

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(NavMeshAgent))]
public class UtilityEnemyAI : MonoBehaviour
{
    [Header("Movement & Detection")]
    public float chaseRange = 8f;
    public float attackRange = 1.5f;
    public float moveSpeed = 2f;

    [Header("Combat")]
    public float attackCooldown = 1f;
    public int damage = 1;
    public LayerMask playerLayer;

    Rigidbody2D rb;
    NavMeshAgent agent;
    Transform player;
    List<UtilityAction> actions;
    float lastAttackTime;

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

        actions = new List<UtilityAction>
        {
            new RoamAction(this),
            new ChaseAction(this),
            new AttackAction(this),
            new RetreatAction(this)
        };
    }

    void Update()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver) return;
        if (player == null) return;

        UtilityAction best = null;
        float bestScore = float.MinValue;
        foreach (var act in actions)
        {
            float score = act.Evaluate();
            if (score > bestScore)
            {
                bestScore = score;
                best = act;
            }
        }
        best?.Act();
    }

    // --- Utility Actions ---
    class RoamAction : UtilityAction
    {
        UtilityEnemyAI ai;
        Vector2 roamDir;
        float nextChange;
        const float changeInterval = 2f;

        public RoamAction(UtilityEnemyAI ai)
        {
            this.ai = ai;
            ChooseNewDir();
        }

        void ChooseNewDir()
        {
            float ang = Random.Range(0f, Mathf.PI * 2f);
            roamDir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
            nextChange = Time.time + changeInterval;
        }

        public override float Evaluate()
        {
            float dist = Vector2.Distance(ai.player.position, ai.transform.position);
            if (dist <= ai.chaseRange)
                return 0.1f; // less desirable when player nearby
            return 0.6f;
        }

        public override void Act()
        {
            if (Time.time >= nextChange)
                ChooseNewDir();
            if (ai.agent != null)
                ai.agent.isStopped = true;
            ai.rb.MovePosition(ai.rb.position + roamDir * ai.moveSpeed * Time.deltaTime);
        }
    }

    class ChaseAction : UtilityAction
    {
        UtilityEnemyAI ai;
        public ChaseAction(UtilityEnemyAI ai) { this.ai = ai; }

        public override float Evaluate()
        {
            float dist = Vector2.Distance(ai.player.position, ai.transform.position);
            return dist <= ai.chaseRange && dist > ai.attackRange ? 0.8f : 0f;
        }

        public override void Act()
        {
            if (ai.agent != null)
            {
                ai.agent.isStopped = false;
                ai.agent.SetDestination(ai.player.position);
            }
            else
            {
                Vector2 dir = ((Vector2)ai.player.position - ai.rb.position).normalized;
                ai.rb.MovePosition(ai.rb.position + dir * ai.moveSpeed * Time.deltaTime);
            }
        }
    }

    class AttackAction : UtilityAction
    {
        UtilityEnemyAI ai;
        public AttackAction(UtilityEnemyAI ai) { this.ai = ai; }

        public override float Evaluate()
        {
            float dist = Vector2.Distance(ai.player.position, ai.transform.position);
            if (dist <= ai.attackRange && Time.time >= ai.lastAttackTime + ai.attackCooldown)
                return 1f;
            return 0f;
        }

        public override void Act()
        {
            ai.lastAttackTime = Time.time;
            if (ai.player.TryGetComponent<PlayerHealth>(out var ph))
                ph.TakeDamage(ai.damage);
        }
    }

    class RetreatAction : UtilityAction
    {
        UtilityEnemyAI ai;
        float retreatEnd;
        const float duration = 1f;

        public RetreatAction(UtilityEnemyAI ai) { this.ai = ai; }

        public override float Evaluate()
        {
            float dist = Vector2.Distance(ai.player.position, ai.transform.position);
            if (Time.time < retreatEnd) return 0.9f;
            return dist <= ai.attackRange ? 0.5f : 0f;
        }

        public override void Act()
        {
            if (Time.time >= retreatEnd)
                retreatEnd = Time.time + duration;
            if (ai.agent != null)
                ai.agent.isStopped = true;
            Vector2 dir = ((Vector2)ai.transform.position - (Vector2)ai.player.position).normalized;
            ai.rb.MovePosition(ai.rb.position + dir * ai.moveSpeed * Time.deltaTime);
        }
    }
}


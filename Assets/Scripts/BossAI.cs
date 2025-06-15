using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyHealth))]
public class BossAI : MonoBehaviour
{
    [Header("Prefabs")] 
    public GameObject minionPrefab;
    public GameObject areaAttackPrefab;

    [Header("Cooldowns")]
    public float summonCooldown = 5f;
    public float areaCooldown = 8f;

    [Header("Phase Settings")]
    [Tooltip("Fraction of max HP when the boss becomes enraged")] 
    public float enragedThreshold = 0.5f;
    [Tooltip("Multiplier applied to cooldowns when enraged")] 
    public float enragedRate = 0.5f;

    private EnemyHealth health;
    private float nextSummon;
    private float nextArea;
    private bool enraged;

    void Start()
    {
        health = GetComponent<EnemyHealth>();
        nextSummon = Time.time + summonCooldown;
        nextArea = Time.time + areaCooldown;
    }

    void Update()
    {
        if (GameOverManager.Instance != null && GameOverManager.Instance.IsGameOver)
            return;

        if (health != null && !enraged && health.CurrentHealth <= health.maxHealth * enragedThreshold)
        {
            enraged = true;
            summonCooldown *= enragedRate;
            areaCooldown *= enragedRate;
        }

        if (Time.time >= nextSummon)
        {
            SummonMinions();
            nextSummon = Time.time + summonCooldown;
        }

        if (Time.time >= nextArea)
        {
            AreaAttack();
            nextArea = Time.time + areaCooldown;
        }
    }

    void SummonMinions()
    {
        if (minionPrefab == null) return;
        const int count = 3;
        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 1.5f;
            Vector3 target = transform.position + (Vector3)offset;
            if (NavMesh.SamplePosition(target, out var hit, 1f, NavMesh.AllAreas))
                target = hit.position;
            Instantiate(minionPrefab, target, Quaternion.identity);
        }
    }

    void AreaAttack()
    {
        if (areaAttackPrefab == null) return;
        Instantiate(areaAttackPrefab, transform.position, Quaternion.identity);
    }
}

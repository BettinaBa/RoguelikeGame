using UnityEngine;

/// <summary>
/// Tracks simple per-run stats: kills, damage taken,
/// time spent in Chase state vs. Attack state.
/// Must exist in the scene as a DontDestroyOnLoad singleton.
/// </summary>
public class RunMetrics : MonoBehaviour
{
    public static RunMetrics Instance { get; private set; }

    [HideInInspector] public int kills = 0;
    [HideInInspector] public int damageTaken = 0;
    [HideInInspector] public float timeInChase = 0f;
    [HideInInspector] public float timeInAttack = 0f;

    // New metrics for accuracy and kill time
    [HideInInspector] public int shotsFired = 0;
    [HideInInspector] public int shotsHit = 0;
    [HideInInspector] public float totalKillTime = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Call when an enemy is killed.
    /// </summary>
    public void RegisterKill()
    {
        kills++;
    }

    /// <summary>
    /// Call when the player takes damage.
    /// </summary>
    public void RegisterDamage(int amount)
    {
        damageTaken += amount;
    }

    /// <summary>
    /// Call each frame (or in FixedUpdate) when an EnemyAI is in the Chase state.
    /// Pass in Time.deltaTime.
    /// </summary>
    public void RecordChase(float deltaSeconds)
    {
        timeInChase += deltaSeconds;
    }

    /// <summary>
    /// Call each frame when an EnemyAI is in the Attack state.
    /// Pass in Time.deltaTime.
    /// </summary>
    public void RecordAttack(float deltaSeconds)
    {
        timeInAttack += deltaSeconds;
    }

    /// <summary>
    /// Call when the player fires a projectile.
    /// </summary>
    public void RegisterShot()
    {
        shotsFired++;
    }

    /// <summary>
    /// Call when a projectile successfully hits an enemy.
    /// </summary>
    public void RegisterHit()
    {
        shotsHit++;
    }

    /// <summary>
    /// Record the time from spawn to kill for an enemy.
    /// </summary>
    public void RegisterKillTime(float seconds)
    {
        totalKillTime += seconds;
    }

    /// <summary>
    /// Current shot accuracy ratio (0-1).
    /// </summary>
    public float Accuracy => shotsFired > 0 ? (float)shotsHit / shotsFired : 0f;

    /// <summary>
    /// Average time it takes to kill an enemy.
    /// </summary>
    public float AverageKillTime => kills > 0 ? totalKillTime / kills : 0f;

    /// <summary>
    /// Optional hook to snapshot or process metrics before sampling.
    /// </summary>
    public void FinalizeMetrics()
    {
        // no-op by default
    }
}

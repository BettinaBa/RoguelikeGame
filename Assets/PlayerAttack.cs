using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRadius = 1f;
    public LayerMask enemyLayers;
    public float cooldownTime = 0.5f;
    private float lastAttackTime = -999f;
    private Vector3 originalScale;
    public float scaleMultiplier = 1.2f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastAttackTime + cooldownTime)
            Swing();
    }

    void Swing()
    {
        lastAttackTime = Time.time;
        // Quick pulse effect on placeholder circle
        transform.localScale = originalScale * scaleMultiplier;
        Invoke(nameof(ResetScale), cooldownTime / 2);

        // Damage logic
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayers);
        Debug.Log($"Attack detected {hits.Length} hit(s)");
        foreach (var hit in hits)
        {
            var enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(1);
        }
    }

    void ResetScale()
    {
        transform.localScale = originalScale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
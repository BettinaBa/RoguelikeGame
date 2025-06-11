using UnityEngine;
using UnityEngine.InputSystem;  // ← new

public class PlayerAttack : MonoBehaviour
{
    public float attackRadius = 1f;
    public LayerMask enemyLayers;
    public float cooldownTime = 0.5f;
    private float lastAttackTime = -999f;
    private Vector3 originalScale;
    public float scaleMultiplier = 1.2f;
    public int damage = 1;

    void Start()
    {
        originalScale = transform.localScale;
    }

    // ← new callback, replaces Update polling
    public void OnAttack(InputValue value)
    {
        if (!value.isPressed) return;
        if (Time.time < lastAttackTime + cooldownTime) return;
        Swing();
    }

    void Swing()
    {
        lastAttackTime = Time.time;
        transform.localScale = originalScale * scaleMultiplier;
        Invoke(nameof(ResetScale), cooldownTime / 2);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, enemyLayers);
        foreach (var hit in hits)
        {
            var enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Damaged {hit.name}");
            }
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
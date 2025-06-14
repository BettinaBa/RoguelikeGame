using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AoEBurst : MonoBehaviour
{
    [Tooltip("Damage dealt to enemies inside the radius")] public int damage = 1;
    [Tooltip("Radius of the AoE burst")] public float radius = 1f;
    [Tooltip("Layer mask for enemies")] public LayerMask enemyLayers;

    void Start()
    {
        DamageEnemies();
        // auto-destroy after particle duration if any
        float life = 1f;
        var ps = GetComponent<ParticleSystem>();
        if (ps != null)
            life = ps.main.duration;
        Destroy(gameObject, life);
    }

    private void DamageEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayers);
        foreach (var hit in hits)
        {
            var eh = hit.GetComponent<EnemyHealth>();
            if (eh != null)
                eh.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

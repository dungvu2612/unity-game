using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [SerializeField] private float damage = 30f;
    [SerializeField] private float lifeTime = 0.4f;
    private bool hasDamaged = false;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasDamaged) return;

        if (collision.CompareTag("Player"))
        {
            Player p = collision.GetComponent<Player>() ?? collision.GetComponentInParent<Player>();
            if (p != null)
            {
                p.TakeDamage(damage);
                hasDamaged = true;
            }
        }
    }
}

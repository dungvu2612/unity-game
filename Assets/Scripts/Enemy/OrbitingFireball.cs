using UnityEngine;

public class OrbitingFireball : MonoBehaviour
{
    [Header("Orbit Settings")]
    [SerializeField] private float orbitRadius = 3f;
    [SerializeField] private float orbitSpeed = 40f;

    [Header("Lifetime")]
    [SerializeField] private float lifeAfterShot = 3f;
    [SerializeField] private float explosionDuration = 0.4f;

    private Transform center;
    private float angleDeg;

    private float moveSpeed;   // được truyền từ skill
    private float damage;      // được truyền từ skill

    private bool isOrbiting = true;
    public bool IsOrbiting => isOrbiting;

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D col;

    public int SlotIndex { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    // === Skill truyền TẤT CẢ tham số vào đây ===
    public void InitOrbit(
        Transform center,
        float startAngleDeg,
        float orbitRadius,
        float orbitSpeed,
        float moveSpeed,
        float damage)
    {
        this.center = center;
        this.angleDeg = startAngleDeg;
        this.orbitRadius = orbitRadius;
        this.orbitSpeed = orbitSpeed;
        this.moveSpeed = moveSpeed;
        this.damage = damage;
    }

    private void Update()
    {
        if (isOrbiting)
            UpdateOrbit();
    }

    private void UpdateOrbit()
    {
        if (center == null)
        {
            Destroy(gameObject);
            return;
        }

        angleDeg += orbitSpeed * Time.deltaTime;

        float rad = angleDeg * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * orbitRadius;

        transform.position = (Vector2)center.position + offset;
    }

    public void ShootAt(Vector2 targetPos)
    {
        if (!isOrbiting) return;

        isOrbiting = false;
        transform.SetParent(null);

        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;

        rb.linearVelocity = dir * moveSpeed;

        if (anim != null)
            anim.SetTrigger("Attack");

        Invoke(nameof(SelfExplode), lifeAfterShot);
    }

    private void SelfExplode()
    {
        if (!isOrbiting)
            Explode();
    }

    private void Explode()
    {
        if (anim != null)
            anim.SetTrigger("Explosion");

        if (col != null)
            col.enabled = false;

        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, explosionDuration);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOrbiting) return;

        if (collision.CompareTag("Player"))
        {
            Player p = collision.GetComponent<Player>()
                       ?? collision.GetComponentInParent<Player>();

            if (p != null)
                p.TakeDamage(damage);

            Explode();
        }
    }
}

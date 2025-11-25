using UnityEngine;

public class OrbitingFireball : MonoBehaviour
{
    [Header("Orbit Settings")]
    [SerializeField] private float orbitRadius = 3f;     // có thể chỉnh trong Enemy_4 thay
    [SerializeField] private float orbitSpeed = 40f;

    [Header("Attack Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float damage = 30f;
    [SerializeField] private float lifeAfterShot = 3f;
    [SerializeField] private float explosionDuration = 0.4f;

    private Transform center;
    private float angleDeg;
    private bool isOrbiting = true;

    private Animator anim;
    private Rigidbody2D rb;
    private Collider2D col;

    // 🔴 CÁI NÀY LÀ MỚI: Enemy_4 dùng để biết quả này thuộc slot nào
    public int SlotIndex { get; set; }

    public bool IsOrbiting => isOrbiting;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Init(Transform center,
                     float startAngleDeg,
                     float orbitRadius,
                     float orbitSpeed,
                     float moveSpeed,
                     float damage)
    {
        this.center = center;
        this.angleDeg = startAngleDeg;  // độ
        this.orbitRadius = orbitRadius;
        this.orbitSpeed = orbitSpeed;
        this.moveSpeed = moveSpeed;
        this.damage = damage;

        //UpdateOrbitPosition();
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

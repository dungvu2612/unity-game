using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 20f;       // damage mỗi nhát
    [SerializeField] private float attackCooldown = 0.4f;    // delay giữa 2 nhát chém
    [SerializeField] private Transform attackOrigin;         // điểm gốc hit (thường đặt ở đầu kiếm)
    [SerializeField] private float attackRadius = 1.2f;      // bán kính hit
    [SerializeField] private LayerMask enemyLayer;           // layer Enemy

    [Header("Sound")]
    [SerializeField] private AudioClip swingSFX;             // tiếng vung kiếm

    private float nextAttackTime = 0f;

    private AudioSource audioSource;
    private Animator animator;
    private Player player; // để biết hướng nhìn (otherDirection)

    private void Awake()
    {
        // Script này đặt trên SwordWeapon (con của Knight)
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();          // nếu Animator nằm trên kiếm
        if (animator == null)
        {
            // Nếu Animator nằm trên nhân vật thì lấy ở cha
            animator = GetComponentInParent<Animator>();
        }

        player = GetComponentInParent<Player>();

        // Fallback: nếu quên gán attackOrigin thì dùng chính transform của kiếm
        if (attackOrigin == null)
        {
            attackOrigin = transform;
        }
    }

    private void Update()
    {
        // Chuột trái tấn công – giống style WitchWeapon
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        nextAttackTime = Time.time + attackCooldown;

        // Gọi animation chém (state Attack 90° bạn đã làm sẵn)
        if (animator != null)
        {
            animator.SetTrigger("attack");   // nhớ tạo trigger "attack"
        }

        // Tiếng vung kiếm
        if (audioSource != null && swingSFX != null)
        {
            audioSource.PlayOneShot(swingSFX);
        }

        // Gây damage có 2 cách:
        // 1) Gọi luôn ở đây (hit tức thời)
        // 2) Gọi từ Animation Event đúng frame chém
        // Ở đây mình tách ra hàm riêng để bạn dễ gọi từ Animation Event:
        // PerformHit();
    }

    /// <summary>
    /// Hàm này sẽ được gọi khi lưỡi kiếm "đi qua" kẻ địch.
    /// Bạn có thể gọi trực tiếp HOẶC gọi từ Animation Event.
    /// </summary>
    public void PerformHit()
    {
        if (attackOrigin == null) return;

        // Tìm tất cả collider enemy trong bán kính
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackOrigin.position,
            attackRadius,
            enemyLayer
        );

        // Hướng nhân vật đang nhìn
        Vector2 facingDir = Vector2.right;
        if (player != null)
        {
            facingDir = new Vector2(player.otherDirection, 0f);
        }

        foreach (Collider2D hit in hits)
        {
            // Giới hạn trong 90° phía trước (±45°)
            Vector2 dirToTarget = (hit.transform.position - attackOrigin.position).normalized;
            float angle = Vector2.Angle(facingDir, dirToTarget);

            if (angle <= 45f)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                }
            }
        }
    }

    // Để thấy vùng hit trong Scene
    private void OnDrawGizmosSelected()
    {
        if (attackOrigin == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
    }
}

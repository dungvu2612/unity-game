using System.Collections;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("Swing Settings")]
    [SerializeField] private float swingAngle = 90f;
    [SerializeField] private float swingDuration = 0.12f;
    [SerializeField] private float attackCooldown = 0.25f;

    [Header("Hit Settings")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private float attackRadius = 1.2f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Sound")]
    [SerializeField] private AudioClip swingSFX;

    private AudioSource audioSource;
    private bool isSwinging = false;
    private bool hasHit = false;
    private float nextAttackTime = 0f;

    private Quaternion normalRotation; 
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (attackOrigin == null)
            attackOrigin = transform;
    }

    private void Update()
    {
        RotateTowardMouse();   

        if (Input.GetMouseButtonDown(0) && !isSwinging && Time.time >= nextAttackTime)
        {
            StartCoroutine(SwingCoroutine());
        }
    }

    // ⭐ LUÔN xoay kiếm hướng đến con trỏ chuột
    private void RotateTowardMouse()
    {
        if (isSwinging) return;  // khi đang vung, KHÔNG cập nhật hướng chuột

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mouse - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        normalRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = normalRotation;
    }

    // ⭐ Vung kiếm theo hướng chuột
    private IEnumerator SwingCoroutine()
    {
        isSwinging = true;
        hasHit = false;
        nextAttackTime = Time.time + attackCooldown;

        // góc hiện tại (đang hướng theo chuột)
        Quaternion startRot = normalRotation;

        // vung 90° theo hướng chuột
        Quaternion endRot = startRot * Quaternion.Euler(0, 0, -swingAngle);

        // tiếng chém
        if (audioSource != null && swingSFX != null)
            audioSource.PlayOneShot(swingSFX);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / swingDuration;
            float lerpT = Mathf.Clamp01(t);

            transform.rotation = Quaternion.Slerp(startRot, endRot, lerpT);

            // hit enemy giữa nhát chém
            if (!hasHit && lerpT >= 0.5f)
            {
                PerformHit();
                hasHit = true;
            }

            yield return null;
        }

        // trả về hướng chuột
        transform.rotation = normalRotation;

        isSwinging = false;
    }

    private void PerformHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackOrigin.position,
            attackRadius,
            enemyLayer);

        Debug.Log($"[MeleeWeapon] Hit check: {hits.Length} colliders found.");

        foreach (var hit in hits)
        {
            // Thử lấy Enemy trên chính object
            Enemy enemy = hit.GetComponent<Enemy>();

            // Nếu không có, thử lấy trên cha (trường hợp collider nằm ở child)
            if (enemy == null)
            {
                enemy = hit.GetComponentInParent<Enemy>();
            }

            if (enemy != null)
            {
                Debug.Log($"[MeleeWeapon] Hit enemy: {enemy.name}");
                enemy.TakeDamage(20f);
            }
            else
            {
                Debug.Log($"[MeleeWeapon] Hit something without Enemy: {hit.name}");
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (attackOrigin != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
        }
    }
}

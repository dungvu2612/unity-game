using System.Collections;
using UnityEngine;

public class Enemy_Bomber : Enemy
{
    [Header("Ranges")]
    [SerializeField] private float chaseRange = 8f;     // tầm dí theo player
    [SerializeField] private float explodeRange = 2f;   // tầm bắt đầu nhấp nháy / chuẩn bị nổ

    [Header("Explosion Settings")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float fuseTime = 2f;         // thời gian chờ trước khi nổ
    [SerializeField] private float blinkInterval = 0.15f; // tốc độ nhấp nháy

    private Animator anim;
    private SpriteRenderer sr;

    private bool isChasing = false;
    private bool isPrimed = false;       // đang đếm giờ nổ
    private bool hasExploded = false;    // đã nổ rồi (phòng double)
    private Coroutine fuseCo;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    protected override void FixedUpdate()
    {
        if (player == null || hasExploded)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.transform.position);

        // ĐANG CHUẨN BỊ NỔ (nhấp nháy)
        if (isPrimed)
        {
            // Player chạy ra khỏi vùng nổ → huỷ chuẩn bị, quay lại dí
            if (dist > explodeRange)
            {
                CancelFuse();
            }
            else
            {
                rb.linearVelocity = Vector2.zero;  // đứng im khi chờ nổ
            }

            UpdateAnimation();
            return;
        }

        // CHƯA CHUẨN BỊ NỔ

        // 1. Nếu player vào vùng nổ → bắt đầu nhấp nháy & đếm giờ
        if (dist <= explodeRange)
        {
            StartFuse();
        }
        // 2. Nếu player trong tầm chaseRange → đuổi
        else if (dist <= chaseRange)
        {
            isChasing = true;
            RunToPlayer(); // dùng hàm có sẵn từ Enemy
        }
        // 3. Quá xa → đứng yên
        else
        {
            isChasing = false;
            rb.linearVelocity = Vector2.zero;
        }

        UpdateAnimation();
    }

    private void StartFuse()
    {
        if (isPrimed) return;

        isPrimed = true;
        isChasing = false;
        rb.linearVelocity = Vector2.zero;

        fuseCo = StartCoroutine(FuseRoutine());
    }

    private IEnumerator FuseRoutine()
    {
        float timer = 0f;
        bool visible = true;

        while (timer < fuseTime)
        {
            // Player có thể bị destroy giữa chừng
            if (player == null)
            {
                CancelFuse();
                yield break;
            }

            // Mỗi tick nhấp nháy kiểm tra lại khoảng cách
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist > explodeRange)
            {
                CancelFuse();
                yield break;
            }

            if (sr != null)
            {
                visible = !visible;
                sr.enabled = visible;
            }

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        // Hết fuseTime mà player vẫn trong vùng → NỔ
        Explode();
    }

    private void CancelFuse()
    {
        isPrimed = false;

        if (fuseCo != null)
        {
            StopCoroutine(fuseCo);
            fuseCo = null;
        }

        if (sr != null) sr.enabled = true; // đảm bảo không bị tắt sprite

        isChasing = true;                  // quay lại mode dí
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;
        isPrimed = false;
        isChasing = false;

        if (sr != null) sr.enabled = true;

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Die();  // enemy chết sau khi kích nổ
    }

    private void UpdateAnimation()
    {
        if (anim == null) return;
        anim.SetBool("IsRunning", isChasing);   // Idle = false, Run = true
    }

    // Vẽ vòng tầm chase / tầm nổ để bạn dễ chỉnh trong Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRange);
    }
}

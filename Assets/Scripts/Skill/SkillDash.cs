using System.Collections;
using UnityEngine;

public class SkillDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashDistance = 4f;
    public float dashDuration = 0.12f;
    public float cooldown = 1.5f;
    public float dashManaCost = 15f;

    [Header("Invincibility")]
    public bool invincibleDuringDash = true;
    [Tooltip("Các layer muốn bỏ va chạm tạm thời (Enemy, EnemyBullet, v.v.)")]
    public LayerMask ignoreCollisionLayer;

    [Header("Sound")]
    public AudioClip dashSFX;

    private float lastDashTime = -999f;
    private bool isDashing = false;

    private Rigidbody2D rb;
    private Player player;
    private AudioSource audioSource;

    int playerLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();

        playerLayer = gameObject.layer;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            TryDash();
    }

    private void TryDash()
    {
        if (Time.time < lastDashTime + cooldown) return;
        if (isDashing) return;

        // Nếu Player không đủ mana → hủy skilld
        if (!player.TrySpendMana(dashManaCost))
        {
            Debug.Log("Not enough mana to Dash!");
            return;
        }

        StartCoroutine(DashRoutine());
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        lastDashTime = Time.time;

        if (dashSFX != null && audioSource != null)
            audioSource.PlayOneShot(dashSFX);

        Vector2 dashDirection = GetDashDirection();
        Vector2 startPos = rb.position;
        Vector2 endPos = startPos + dashDirection * dashDistance;

        float t = 0f;

        // 🔹 Bỏ va chạm với các layer trong ignoreCollisionLayer (không đụng tới tường)
        if (invincibleDuringDash)
            SetIgnoreLayers(true);

        while (t < dashDuration)
        {
            t += Time.deltaTime;
            float lerpT = t / dashDuration;
            rb.MovePosition(Vector2.Lerp(startPos, endPos, lerpT));
            yield return null;
        }

        // 🔹 Bật lại va chạm bình thường
        if (invincibleDuringDash)
            SetIgnoreLayers(false);

        isDashing = false;
    }

    // Bật / tắt IgnoreCollision giữa PlayerLayer và các layer trong ignoreCollisionLayer
    private void SetIgnoreLayers(bool ignore)
    {
        int mask = ignoreCollisionLayer.value;

        for (int layer = 0; layer < 32; layer++)
        {
            if ((mask & (1 << layer)) != 0)
            {
                Physics2D.IgnoreLayerCollision(playerLayer, layer, ignore);
            }
        }
    }

    // Dash theo hướng di chuyển, nếu đứng yên thì dash theo hướng chuột
    private Vector2 GetDashDirection()
    {
        Vector2 moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (moveInput.sqrMagnitude > 0.1f)
            return moveInput;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouse - transform.position).normalized;
        return dir;
    }
}

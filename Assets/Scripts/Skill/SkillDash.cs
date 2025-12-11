using System.Collections;
using UnityEngine;

public class SkillDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashDistance = 4f;          // quãng đường tốc biến
    public float dashDuration = 0.12f;       // thời gian dash
    public float cooldown = 1.5f;            // hồi chiêu
    public float dashManaCost = 15f;         // mana tiêu tốn

    [Header("Invincibility")]
    public bool invincibleDuringDash = true;
    public LayerMask ignoreCollisionLayer;

    [Header("Sound")]
    public AudioClip dashSFX;

    private float lastDashTime = -999f;
    private bool isDashing = false;

    private Rigidbody2D rb;
    private Player player;
    private AudioSource audioSource;
    private Collider2D playerCollider;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        audioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<Collider2D>();
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

        // Nếu Player không đủ mana → hủy skill
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

        // Tắt va chạm nếu bật invincible
        if (invincibleDuringDash && playerCollider != null)
            playerCollider.enabled = false;

        while (t < dashDuration)
        {
            t += Time.deltaTime;
            float lerpT = t / dashDuration;
            rb.MovePosition(Vector2.Lerp(startPos, endPos, lerpT));
            yield return null;
        }

        // Bật lại va chạm
        if (invincibleDuringDash && playerCollider != null)
            playerCollider.enabled = true;

        isDashing = false;
    }

    // Dash theo hướng di chuyển nếu có, còn không thì dash theo hướng chuột
    private Vector2 GetDashDirection()
    {
        Vector2 moveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (moveInput.sqrMagnitude > 0.1f)
            return moveInput;

        // Nếu không di chuyển, dash theo hướng chuột
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouse - transform.position).normalized;

        return dir;
    }
}

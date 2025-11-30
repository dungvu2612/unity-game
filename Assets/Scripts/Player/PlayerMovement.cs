using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Footstep Sound")]
    [SerializeField] private AudioClip footstepSFX;
    [SerializeField] private float footstepInterval = 0.25f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioSource audioSource;

    private float nextFootstepTime = 0f;

    public int OtherDirection { get; private set; } = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        MovePlayer();
        UpdateAnimation();
        HandleFootstepSound();
    }

    private Vector2 GetInput()
    {
        return new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
    }

    private void MovePlayer()
    {
        Vector2 input = GetInput().normalized;

        rb.linearVelocity = input * moveSpeed;

        if (input.x < 0)
        {
            OtherDirection = -1;
            spriteRenderer.flipX = true;
        }
        else if (input.x > 0)
        {
            OtherDirection = 1;
            spriteRenderer.flipX = false;
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        bool isMoving = rb.linearVelocity.sqrMagnitude > 0.001f;
        animator.SetBool("isWalk", isMoving);
    }

    private void HandleFootstepSound()
    {
        if (audioSource == null || footstepSFX == null || rb == null) return;

        bool isMoving = rb.linearVelocity.sqrMagnitude > 0.01f;

        if (isMoving && Time.time >= nextFootstepTime)
        {
            audioSource.PlayOneShot(footstepSFX);
            nextFootstepTime = Time.time + footstepInterval;
        }
    }

    // ==== Buff tốc chạy trong 1 khoảng thời gian ====
    public void AddSpeedBuff(float bonusSpeed, float duration)
    {
        StartCoroutine(SpeedBuffRoutine(bonusSpeed, duration));
    }

    private System.Collections.IEnumerator SpeedBuffRoutine(float bonusSpeed, float duration)
    {
        moveSpeed += bonusSpeed;
        yield return new WaitForSeconds(duration);
        moveSpeed -= bonusSpeed;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Player : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] protected float moveSpeed = 10f;

    [Header("Stats")]
    [SerializeField] protected float maxHP = 100f;
    [SerializeField] protected float maxArmor = 50f;
    [SerializeField] protected float maxMana = 100f;

    protected float currentHP;
    protected float currentArmor;
    protected float currentMana;

    [Header("UI Bars")]
    [SerializeField] private Image hpBar;      // HeartBar
    [SerializeField] private Image armorBar;   // ArmorBar
    [SerializeField] private Image manaBar;    // ManaBar

    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;

    public int otherDirection { get; protected set; } = 1;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        currentHP = maxHP;
        currentArmor = maxArmor;
        currentMana = maxMana;

        UpdateBars();
    }

    protected virtual void Update()
    {
        MovePlayer();
        UpdateAnimation();
    }

    protected virtual Vector2 GetInput()
    {
        return new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
    }

    protected virtual void MovePlayer()
    {
        Vector2 input = GetInput().normalized;

        rb.linearVelocity = input * moveSpeed;

        if (input.x < 0)
        {
            otherDirection = -1;
            spriteRenderer.flipX = true;
        }
        else if (input.x > 0)
        {
            otherDirection = 1;
            spriteRenderer.flipX = false;
        }
    }

    protected virtual void UpdateAnimation()
    {
        if (animator == null) return;

        bool isMoving = rb.linearVelocity.sqrMagnitude > 0.001f;
        animator.SetBool("isWalk", isMoving);
    }

    // ================== COMBAT / STATS ==================

    // Enemy gọi hàm này khi gây sát thương
    public virtual void TakeDamage(float damage)
    {
        if (damage <= 0f) return;

        // 1. Trừ giáp trước
        if (currentArmor > 0f)
        {
            float armorDamage = Mathf.Min(damage, currentArmor);
            currentArmor -= armorDamage;
            damage -= armorDamage;
        }

        // 2. Nếu còn damage thì trừ vào máu
        if (damage > 0f)
        {
            currentHP -= damage;
        }

        // 3. Clamp giá trị & update UI
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
        currentArmor = Mathf.Clamp(currentArmor, 0f, maxArmor);

        UpdateBars();

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    public virtual void RestoreMana(float amount)
    {
        if (amount <= 0f) return;

        currentMana = Mathf.Clamp(currentMana + amount, 0f, maxMana);
        UpdateBars();
    }

    public virtual void RestoreArmor(float amount)
    {
        if (amount <= 0f) return;

        currentArmor = Mathf.Clamp(currentArmor + amount, 0f, maxArmor);
        UpdateBars();
    }
    public virtual void RestoreHP(float amount)
    {
        if (amount <= 0f) return;

        currentHP = Mathf.Clamp(currentHP + amount, 0f, maxHP);
        UpdateBars();
    }

    // ==== NEW: Buff tốc chạy trong 1 khoảng thời gian ====
    public virtual void AddSpeedBuff(float bonusSpeed, float duration)
    {
        StartCoroutine(SpeedBuffRoutine(bonusSpeed, duration));
    }

    private IEnumerator SpeedBuffRoutine(float bonusSpeed, float duration)
    {
        moveSpeed += bonusSpeed;
        yield return new WaitForSeconds(duration);
        moveSpeed -= bonusSpeed;
    }

    // Trả về true nếu đủ mana & đã trừ
    protected virtual bool SpendMana(float amount)
    {
        if (amount <= 0f) return true;

        if (currentMana < amount)
            return false;

        currentMana = Mathf.Clamp(currentMana - amount, 0f, maxMana);
        UpdateBars();
        return true;
    }

    protected virtual void UpdateBars()
    {
        if (hpBar != null && maxHP > 0f)
            hpBar.fillAmount = currentHP / maxHP;

        if (armorBar != null && maxArmor > 0f)
            armorBar.fillAmount = currentArmor / maxArmor;

        if (manaBar != null && maxMana > 0f)
            manaBar.fillAmount = currentMana / maxMana;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHP = 100f;
    [SerializeField] protected float maxArmor = 50f;
    [SerializeField] protected float maxMana = 100f;

    protected float currentHP;
    protected float currentArmor;
    protected float currentMana;

    [Header("UI Bars")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image armorBar;
    [SerializeField] private Image manaBar;

    [Header("Sound")]
    [SerializeField] private AudioClip buffPickupSFX;    // tiếng khi được buff
    [SerializeField] private AudioClip hitSFX;           // tiếng khi nhận damage
    [SerializeField] private AudioClip deathSFX;         // tiếng khi chết

    private AudioSource audioSource;
    private PlayerMovement movement;                     // tham chiếu sang script movement

    public void SetupUIBars(Image hp, Image armor, Image mana)
    {
        hpBar = hp;
        armorBar = armor;
        manaBar = mana;
        UpdateBars();
    }

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        movement = GetComponent<PlayerMovement>();    // để buff tốc độ
    }

    protected virtual void Start()
    {
        currentHP = maxHP;
        currentArmor = maxArmor;
        currentMana = maxMana;

        UpdateBars();
    }
    public bool TrySpendMana(float amount)
    {
        return SpendMana(amount);   // dùng hàm protected bên trong
    }
    // ================== SOUND ==================

    public virtual void PlayBuffSound()
    {
        if (audioSource != null && buffPickupSFX != null)
        {
            audioSource.PlayOneShot(buffPickupSFX);
        }
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

        if (audioSource != null && hitSFX != null)
        {
            audioSource.PlayOneShot(hitSFX);
        }

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
        PlayBuffSound();
    }

    public virtual void RestoreArmor(float amount)
    {
        if (amount <= 0f) return;

        currentArmor = Mathf.Clamp(currentArmor + amount, 0f, maxArmor);
        UpdateBars();
        PlayBuffSound();
    }

    public virtual void RestoreHP(float amount)
    {
        if (amount <= 0f) return;

        currentHP = Mathf.Clamp(currentHP + amount, 0f, maxHP);
        UpdateBars();
        PlayBuffSound();
    }

    // ==== Buff tốc chạy: Player vẫn có API cũ, nhưng ủy quyền cho PlayerMovement ====
    public virtual void AddSpeedBuff(float bonusSpeed, float duration)
    {
        if (movement != null)
        {
            movement.AddSpeedBuff(bonusSpeed, duration);
            PlayBuffSound();
        }
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
        if (deathSFX != null)
        {
            AudioSource.PlayClipAtPoint(deathSFX, transform.position);
        }

        if (PauseMenuManager.Instance != null)
            PauseMenuManager.Instance.ShowGameOver();

        Destroy(gameObject);
    }
}

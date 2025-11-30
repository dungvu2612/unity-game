using UnityEngine;
using UnityEngine.UI;
using System;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHP = 100f;
    [SerializeField] protected float speedOfEnemy = 20f;
    protected float currentHP;

    [SerializeField] private Image HpBar;
    protected Player player;
    protected Rigidbody2D rb;
    public event Action OnDeath;

    [Header("Loot Settings")]
    [SerializeField] private GameObject manaPickupPrefab;
    [SerializeField] private GameObject hpPickupPrefab;
    [SerializeField] private GameObject speedPickupPrefab;

    [Header("IconSkull")]
    [SerializeField] private GameObject deathIconPrefab;
    [SerializeField] private float deathIconDuration = 5f;

    // 👉 Cho script khác đọc Player
    public Player TargetPlayer => player;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        player = FindAnyObjectByType<Player>();
        currentHP = maxHP;
        UpdateHpBar();
    }

    protected virtual void Update() { }

    protected virtual void FixedUpdate()
    {
        RunToPlayer();
    }

    protected void RunToPlayer()
    {
        if (player == null || rb == null) return;

        Vector2 target = player.transform.position;
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speedOfEnemy * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        FlipEnemy();
    }

    protected void FlipEnemy()
    {
        if (player != null)
        {
            transform.localScale =
                new Vector3(player.transform.position.x < transform.position.x ? -1 : 1, 1, 1);
        }
    }

    protected void UpdateHpBar()
    {
        if (HpBar != null)
        {
            HpBar.fillAmount = currentHP / maxHP;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);
        UpdateHpBar();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void ShowDeathIcon()
    {
        if (deathIconPrefab == null) return;

        GameObject icon = Instantiate(
            deathIconPrefab,
            transform.position + new Vector3(0, 1f, 0),
            Quaternion.identity
        );

        Destroy(icon, deathIconDuration);
    }

    protected virtual void Die()
    {
        DropLoot();
        ShowDeathIcon();

        OnDeath?.Invoke();   // 👉 báo cho skill, v.v.

        Destroy(gameObject);
    }

    protected virtual void DropLoot()
    {
        float roll = UnityEngine.Random.value;

        if (roll < 0.20f)
        {
            if (manaPickupPrefab != null)
                Instantiate(manaPickupPrefab, transform.position, Quaternion.identity);
        }
        else if (roll < 0.30f)
        {
            if (hpPickupPrefab != null)
                Instantiate(hpPickupPrefab, transform.position, Quaternion.identity);
        }
        else if (roll < 0.50f)
        {
            if (speedPickupPrefab != null)
                Instantiate(speedPickupPrefab, transform.position, Quaternion.identity);
        }
    }
}

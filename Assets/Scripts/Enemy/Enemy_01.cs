using UnityEngine;

public class Enemy_01 : Enemy
{
    [Header("Enemy 01 Settings")]
    [SerializeField] private float touchDamage = 10f;

    [Tooltip("Thời gian giữa 2 lần gây damage")]
    [SerializeField] private float attackInterval = 1f;

    [Tooltip("Khoảng cách enemy cần để gây damage")]
    [SerializeField] private float attackDistance = 0.8f;

    private float attackTimer = 0f;

    protected override void Start()
    {
        base.Start();
        attackTimer = 0f;
    }

    private void Update()
    {
        TryAttackPlayer();
    }

    private void TryAttackPlayer()
    {
        if (player == null) return;

        // Tính khoảng cách giữa enemy và player
        float distance = Vector2.Distance(transform.position, player.transform.position);

        // Nếu quá xa thì không tấn công
        if (distance > attackDistance) return;

        // Đếm thời gian hồi chiêu
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            // Gây damage
            player.TakeDamage(touchDamage);

            // Reset hồi chiêu
            attackTimer = attackInterval;
        }
    }
}

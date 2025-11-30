using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrbitFireballSkill : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private int fireballCount = 4;      // số quả pháp cầu orbit
    [SerializeField] private float orbitRadius = 3f;     // bán kính quay
    [SerializeField] private float orbitSpeed = 40f;
    [SerializeField] private float fireballMoveSpeed = 10f;
    [SerializeField] private float fireballDamage = 30f;

    [Header("Attack Logic")]
    [SerializeField] private float attackCooldown = 1f;  // thời gian giữa các lần bắn
    [SerializeField] private float respawnDelay = 2f;    // delay sinh lại quả mới

    private readonly List<OrbitingFireball> fireballs = new();

    private float[] slotAngles;
    private bool[] slotOccupied;

    private float attackTimer;

    private Enemy enemy;     // tham chiếu Enemy chung
    private Player player;   // target

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("[EnemyOrbitFireballSkill] Không tìm thấy Enemy trên GameObject!");
            return;
        }

        // đăng ký khi Enemy chết thì dọn fireball
        enemy.OnDeath += HandleEnemyDeath;
    }

    private void Start()
    {
        if (enemy == null) return;

        // lấy player từ Enemy
        player = enemy.TargetPlayer;
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }

        // khởi tạo slot
        slotAngles = new float[fireballCount];
        slotOccupied = new bool[fireballCount];

        float step = 360f / fireballCount;
        for (int i = 0; i < fireballCount; i++)
        {
            slotAngles[i] = step * i;
            slotOccupied[i] = false;
        }

        SpawnInitialFireballs();
    }

    private void FixedUpdate()
    {
        if (enemy == null || player == null) return;

        attackTimer -= Time.fixedDeltaTime;

        if (attackTimer <= 0f)
        {
            ShootOneFireballAtPlayer();
            attackTimer = attackCooldown;
        }
    }

    // ==== Spawn ban đầu: chiếm hết các slot ====
    private void SpawnInitialFireballs()
    {
        fireballs.Clear();

        for (int i = 0; i < fireballCount; i++)
        {
            SpawnFireballInSlot(i);
        }
    }

    private void SpawnFireballInSlot(int slotIndex)
    {
        if (fireballPrefab == null) return;
        if (slotOccupied[slotIndex]) return;

        GameObject fbObj = Instantiate(
            fireballPrefab,
            transform.position,
            Quaternion.identity,
            transform   // quay quanh chính Enemy
        );

        OrbitingFireball orb = fbObj.GetComponent<OrbitingFireball>();
        if (orb != null)
        {
            float angleDeg = slotAngles[slotIndex];

            orb.Init(transform,
                     angleDeg,
                     orbitRadius,
                     orbitSpeed,
                     fireballMoveSpeed,
                     fireballDamage);

            orb.SlotIndex = slotIndex;
            slotOccupied[slotIndex] = true;

            fireballs.Add(orb);
        }
        else
        {
            Debug.LogWarning("[EnemyOrbitFireballSkill] Fireball không có OrbitingFireball component!");
        }
    }

    // ==== Bắn 1 quả về phía Player ====
    private void ShootOneFireballAtPlayer()
    {
        if (player == null) return;

        Vector2 targetPos = player.transform.position;

        OrbitingFireball chosen = null;

        foreach (var fb in fireballs)
        {
            if (fb != null && fb.IsOrbiting)
            {
                chosen = fb;
                break;
            }
        }

        if (chosen == null) return;

        int slotIndex = chosen.SlotIndex;

        chosen.ShootAt(targetPos);

        slotOccupied[slotIndex] = false;
        fireballs.Remove(chosen);

        StartCoroutine(RespawnOneFireball(slotIndex));
    }

    private IEnumerator RespawnOneFireball(int slotIndex)
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnFireballInSlot(slotIndex);
    }

    // Enemy chết → dọn fireball
    private void HandleEnemyDeath()
    {
        foreach (var fb in fireballs)
        {
            if (fb != null)
                Destroy(fb.gameObject);
        }
        fireballs.Clear();
    }

    private void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnDeath -= HandleEnemyDeath;
        }
    }
}

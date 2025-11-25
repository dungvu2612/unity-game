using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_4 : Enemy
{
    [Header("Fireball Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private int fireballCount = 4;      // 4 slot
    [SerializeField] private float orbitRadius = 3f;     // chỉnh rộng ở đây
    [SerializeField] private float orbitSpeed = 40f;
    [SerializeField] private float fireballMoveSpeed = 10f;
    [SerializeField] private float fireballDamage = 30f;

    [Header("Attack Logic")]
    [SerializeField] private float attackCooldown = 1f;  // 2s bắn 1 quả
    [SerializeField] private float respawnDelay = 2f;    // 1s sau sinh lại 1 quả

    private readonly List<OrbitingFireball> fireballs = new();

    private float[] slotAngles;     // góc của từng slot
    private bool[] slotOccupied;   // slot đang có fireball hay chưa

    private float attackTimer;

    protected override void Start()
    {
        base.Start();

        // khởi tạo slot
        slotAngles = new float[fireballCount];
        slotOccupied = new bool[fireballCount];

        float step = 360f / fireballCount;      // 4 → 0,90,180,270
        for (int i = 0; i < fireballCount; i++)
        {
            slotAngles[i] = step * i;
            slotOccupied[i] = false;
        }

        SpawnInitialFireballs();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate(); // vẫn giữ logic đuổi player nếu có

        if (player == null) return;

        attackTimer -= Time.fixedDeltaTime;

        if (attackTimer <= 0f)
        {
            ShootOneFireballAtPlayer();
            attackTimer = attackCooldown;
        }
    }

    // --- SPAWN BAN ĐẦU: chiếm hết 4 slot ---
    private void SpawnInitialFireballs()
    {
        fireballs.Clear();

        for (int i = 0; i < fireballCount; i++)
        {
            SpawnFireballInSlot(i);
        }
    }

    // spawn 1 quả cụ thể vào slotIndex (0..3)
    private void SpawnFireballInSlot(int slotIndex)
    {
        if (fireballPrefab == null) return;
        if (slotOccupied[slotIndex]) return;   // slot đang bận

        GameObject fbObj = Instantiate(
            fireballPrefab,
            transform.position,
            Quaternion.identity,
            transform
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
    }

    // --- BẮN 1 QUẢ ---
    private void ShootOneFireballAtPlayer()
    {
        if (player == null) return;

        Vector2 targetPos = player.transform.position;

        OrbitingFireball chosen = null;

        // tìm 1 quả đang orbit
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

        // bắn
        chosen.ShootAt(targetPos);

        // slot này trống
        slotOccupied[slotIndex] = false;

        // xóa khỏi list (nó tự Destroy sau explosion)
        fireballs.Remove(chosen);

        // sau 1s sinh lại 1 quả ngay slot trống đó
        StartCoroutine(RespawnOneFireball(slotIndex));
    }

    private IEnumerator RespawnOneFireball(int slotIndex)
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnFireballInSlot(slotIndex);
    }

    protected override void Die()
    {
        foreach (var fb in fireballs)
        {
            if (fb != null)
                Destroy(fb.gameObject);
        }
        fireballs.Clear();

        base.Die();
    }
}

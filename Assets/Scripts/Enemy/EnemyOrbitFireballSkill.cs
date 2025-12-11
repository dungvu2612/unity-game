using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOrbitFireballSkill : MonoBehaviour
{
    [Header("Fireball Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private int fireballCount = 4;
    [SerializeField] private float orbitRadius = 3f;
    [SerializeField] private float orbitSpeed = 40f;

    // === 2 biến QUAN TRỌNG: DAMAGE + SPEED ===
    [SerializeField] private float fireballMoveSpeed = 10f;
    [SerializeField] private float fireballDamage = 30f;

    [Header("Attack Logic")]
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float respawnDelay = 2f;

    private readonly List<OrbitingFireball> fireballs = new();

    private float[] slotAngles;
    private bool[] slotOccupied;

    private float attackTimer;

    private Enemy enemy;
    private Player player;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("[EnemyOrbitFireballSkill] Missing Enemy component!");
            return;
        }

        enemy.OnDeath += HandleEnemyDeath;
    }

    private void Start()
    {
        if (enemy == null) return;

        player = enemy.TargetPlayer;
        if (player == null)
            player = FindAnyObjectByType<Player>();

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
        attackTimer -= Time.fixedDeltaTime;

        if (attackTimer <= 0f)
        {
            ShootOneFireballAtPlayer();
            attackTimer = attackCooldown;
        }
    }

    private void SpawnInitialFireballs()
    {
        fireballs.Clear();

        for (int i = 0; i < fireballCount; i++)
            SpawnFireballInSlot(i);
    }

    private void SpawnFireballInSlot(int slotIndex)
    {
        if (fireballPrefab == null || slotOccupied[slotIndex])
            return;

        GameObject fbObj = Instantiate(
            fireballPrefab,
            transform.position,
            Quaternion.identity,
            transform
        );

        OrbitingFireball orb = fbObj.GetComponent<OrbitingFireball>();
        if (orb != null)
        {
            orb.InitOrbit(
                transform,
                slotAngles[slotIndex],
                orbitRadius,
                orbitSpeed,
                fireballMoveSpeed,
                fireballDamage
            );

            orb.SlotIndex = slotIndex;
            slotOccupied[slotIndex] = true;

            fireballs.Add(orb);
        }
        else
        {
            Debug.LogWarning("[EnemyOrbitFireballSkill] Fireball missing OrbitingFireball script!");
        }
    }

    private void ShootOneFireballAtPlayer()
    {
        if (player == null) return;

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

        chosen.ShootAt(player.transform.position);
        slotOccupied[slotIndex] = false;
        fireballs.Remove(chosen);

        StartCoroutine(RespawnOneFireball(slotIndex));
    }

    private IEnumerator RespawnOneFireball(int slotIndex)
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnFireballInSlot(slotIndex);
    }

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
            enemy.OnDeath -= HandleEnemyDeath;
    }
}

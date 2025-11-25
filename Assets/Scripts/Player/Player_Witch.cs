using System.Collections;
using UnityEngine;

public class Witch : Player
{
    [Header("Witch Skill Settings")]
    [SerializeField] private GameObject skillBulletPrefab;   // prefab đạn (WandBullet)
    [SerializeField] private Transform firePoint;            // vị trí bắn, nếu null sẽ dùng transform player

    [SerializeField] private KeyCode skillKey = KeyCode.Space;

    [Tooltip("Mana tiêu tốn khi dùng skill")]
    [SerializeField] private float skillManaCost = 30f;

    [Tooltip("Thời gian skill hoạt động (giây)")]
    [SerializeField] private float skillDuration = 3f;

    [Tooltip("Thời gian giữa mỗi lần bắn (giây)")]
    [SerializeField] private float skillFireInterval = 0.3f;

    [Tooltip("Số hướng bắn (ví dụ 8 = 360/8 = 45 độ mỗi viên)")]
    [SerializeField] private int skillDirections = 8;

    [Tooltip("Mỗi lần bắn sẽ hồi bao nhiêu giáp")]
    [SerializeField] private float armorRestorePerBurst = 5f;

    private bool isCastingSkill;

    protected override void Start()
    {
        base.Start();
        isCastingSkill = false;
    }

    protected override void Update()
    {
        base.Update();
        HandleSkillInput();
    }

    private void HandleSkillInput()
    {
        if (Input.GetKeyDown(skillKey) && !isCastingSkill)
        {
            TryCastSkill();
        }
    }

    private void TryCastSkill()
    {
        // tốn mana 1 lần khi bắt đầu skill
        if (!SpendMana(skillManaCost))
        {
            Debug.Log("Not enough mana for Witch skill!");
            return;
        }

        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        isCastingSkill = true;
        float elapsed = 0f;

        while (elapsed < skillDuration)
        {
            FireInMultipleDirections();
            RestoreArmor(armorRestorePerBurst);

            yield return new WaitForSeconds(skillFireInterval);
            elapsed += skillFireInterval;
        }

        isCastingSkill = false;
    }

    private void FireInMultipleDirections()
    {
        if (skillBulletPrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        if (skillDirections <= 0) skillDirections = 1;

        float stepAngle = 360f / skillDirections;

        for (int i = 0; i < skillDirections; i++)
        {
            float angle = stepAngle * i;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            Instantiate(skillBulletPrefab, spawnPos, rot);
        }
    }
}

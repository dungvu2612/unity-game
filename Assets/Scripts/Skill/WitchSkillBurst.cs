using System.Collections;
using UnityEngine;

public class WitchSkillBurst : MonoBehaviour
{
    [Header("Witch Skill Settings")]
    [SerializeField] private GameObject skillBulletPrefab;   // prefab đạn SKILL (khác đạn thường)
    [SerializeField] private Transform firePoint;            // vị trí bắn skill

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
    private Witch witch;   // tham chiếu đến Witch (Player con)

    private void Awake()
    {
        witch = GetComponent<Witch>();
        if (witch == null)
        {
            Debug.LogError("[WitchSkillBurst] Không tìm thấy Witch trên GameObject!");
        }
    }

    private void Update()
    {
        if (!isCastingSkill && Input.GetKeyDown(skillKey))
        {
            TryCastSkill();
        }
    }

    private void TryCastSkill()
    {
        if (witch == null) return;

        // tốn mana 1 lần khi bắt đầu skill
        if (!witch.TrySpendMana(skillManaCost))
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
            witch.RestoreArmor(armorRestorePerBurst);

            yield return new WaitForSeconds(skillFireInterval);
            elapsed += skillFireInterval;
        }

        isCastingSkill = false;
    }

    private void FireInMultipleDirections()
    {
        if (skillBulletPrefab == null)
        {
            Debug.LogWarning("[WitchSkillBurst] skillBulletPrefab chưa được gán!");
            return;
        }

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

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
    private Player player;   // dùng Player chung

    private void Awake()
    {
        // lấy Player trên chính object này, nếu không có thì thử trên cha
        player = GetComponent<Player>();
        if (player == null)
        {
            player = GetComponentInParent<Player>();
        }

        if (player == null)
        {
            Debug.LogError("[WitchSkillBurst] Không tìm thấy Player trên GameObject hoặc cha!");
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
        if (player == null) return;

        // tốn mana 1 lần khi bắt đầu skill
        if (!player.TrySpendMana(skillManaCost))
        {
            Debug.Log("[WitchSkillBurst] Not enough mana for skill!");
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
            player.RestoreArmor(armorRestorePerBurst);

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

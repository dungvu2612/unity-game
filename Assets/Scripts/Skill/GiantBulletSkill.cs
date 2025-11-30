using UnityEngine;

public class GiantBulletSKill : MonoBehaviour
{
    [Header("Big Shot Settings")]
    [SerializeField] private GameObject bigBulletPrefab;   // prefab đạn ulti (damage to)
    [SerializeField] private Transform firePoint;          // vị trí bắn (giống chỗ bắn đạn thường)

    [SerializeField] private KeyCode skillKey = KeyCode.E; // phím bấm để dùng skill

    [Tooltip("Mana tiêu tốn khi bắn skill")]
    [SerializeField] private float manaCost = 40f;

    [Tooltip("Thời gian hồi chiêu (giây)")]
    [SerializeField] private float cooldown = 5f;

    [Header("Sound")]
    [SerializeField] private AudioClip skillSFX;

    private float nextCastTime = 0f;
    private Player player;
    private AudioSource audioSource;

    private void Awake()
    {
        // Tìm Player ở trên cùng GameObject hoặc cha
        player = GetComponentInParent<Player>();
        if (player == null)
        {
            Debug.LogError("[WitchSkillBigShot] Không tìm thấy Player trong parent!");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Nếu script gắn trên weapon con, lấy AudioSource từ Player
            audioSource = GetComponentInParent<AudioSource>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(skillKey))
        {
            TryCastSkill();
        }
    }

    private void TryCastSkill()
    {
        if (player == null) return;

        // Check cooldown
        if (Time.time < nextCastTime)
        {
            Debug.Log("[WitchSkillBigShot] Skill đang hồi, chưa bắn được!");
            return;
        }

        // Check mana
        if (!player.TrySpendMana(manaCost))
        {
            Debug.Log("[WitchSkillBigShot] Không đủ mana để bắn skill!");
            return;
        }

        CastSkill();
    }

    private void CastSkill()
    {
        nextCastTime = Time.time + cooldown;

        if (bigBulletPrefab == null)
        {
            Debug.LogWarning("[WitchSkillBigShot] Chưa gán bigBulletPrefab!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("[WitchSkillBigShot] Chưa gán firePoint, dùng vị trí Player tạm.");
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Quaternion spawnRot = firePoint != null ? firePoint.rotation : transform.rotation;

        Instantiate(bigBulletPrefab, spawnPos, spawnRot);

        if (audioSource != null && skillSFX != null)
        {
            audioSource.PlayOneShot(skillSFX);
        }

        Debug.Log("[WitchSkillBigShot] Bắn đạn ulti!");
    }
}

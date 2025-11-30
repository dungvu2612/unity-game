using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightSwordOrbitSkill : MonoBehaviour
{
    [Header("Knight Skill - Sword Orbit")]
    [SerializeField] private KeyCode skillKey = KeyCode.Space;      // phím bấm để dùng skill
    [SerializeField] private float skillManaCost = 30f;              // MP tốn khi dùng skill
    [SerializeField] private float skillCooldown = 5f;               // thời gian hồi chiêu

    [Header("Orbit Settings")]
    [SerializeField] private GameObject swordPrefab;                 // prefab kiếm bay quanh người
    [SerializeField] private int swordCount = 7;                     // số lượng kiếm
    [SerializeField] private float orbitRadius = 3f;                 // bán kính quỹ đạo
    [SerializeField] private float orbitDuration = 1.5f;             // kiếm bay quanh bao lâu (giây)
    [SerializeField] private float orbitAngularSpeed = 360f;         // tốc độ quay (độ/giây)

    [Header("Sound")]
    [SerializeField] private AudioClip summonSound;

    private float lastSkillTime = -999f;
    private bool isCastingSkill = false;

    private AudioSource knightAudioSource;
    private Player player;   

    private void Awake()
    {
        player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("[KnightSwordOrbitSkill] Không tìm thấy Knight trên cùng GameObject!");
        }

        knightAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
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
        if (player == null) return;

        // Check cooldown
        if (Time.time < lastSkillTime + skillCooldown)
        {
            Debug.Log("Knight skill on cooldown!");
            return;
        }

        // player mana (dùng TrySpendMana trong Knight)
        if (!player.TrySpendMana(skillManaCost))
        {
            Debug.Log("Not enough mana for Knight skill!");
            return;
        }

        lastSkillTime = Time.time;
        StartCoroutine(SwordOrbitRoutine());
    }

    private IEnumerator SwordOrbitRoutine()
    {
        if (swordPrefab == null)
        {
            Debug.LogError("[KnightSwordOrbitSkill] Chưa gán swordPrefab cho skill Sword Orbit!");
            yield break;
        }

        isCastingSkill = true;

      
        if (knightAudioSource != null && summonSound != null)
        {
            knightAudioSource.PlayOneShot(summonSound);
        }

     
        List<GameObject> swords = new List<GameObject>();
        List<float> angles = new List<float>();  

        for (int i = 0; i < swordCount; i++)
        {
            float angle = i * Mathf.PI * 2f / swordCount; // chia đều 360 độ
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * orbitRadius;
            Vector3 spawnPos = transform.position + offset;

            GameObject sword = Instantiate(swordPrefab, spawnPos, Quaternion.identity);

         
            SpriteRenderer sr = sword.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = "Player";    
                sr.sortingOrder = 10;
            }

            swords.Add(sword);
            angles.Add(angle);
        }

        float elapsed = 0f;

        // Giai đoạn kiếm bay quanh người
        while (elapsed < orbitDuration)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < swords.Count; i++)
            {
                GameObject sword = swords[i];
                if (sword == null) continue;

               
                angles[i] += orbitAngularSpeed * Mathf.Deg2Rad * Time.deltaTime;

                Vector3 offset = new Vector3(Mathf.Cos(angles[i]), Mathf.Sin(angles[i]), 0f) * orbitRadius;
                sword.transform.position = transform.position + offset;

                // xoay mũi kiếm hướng ra ngoài
                Vector3 dir = (sword.transform.position - transform.position).normalized;
                float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                sword.transform.rotation = Quaternion.Euler(0f, 0f, angleDeg);
            }

            yield return null;
        }

         foreach (var s in swords)
        {
            if (s != null) Destroy(s);
        }
        swords.Clear();

        isCastingSkill = false;
    }
}

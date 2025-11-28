using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public Room room;

    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;   // nhiều loại enemy
    public int enemyCount = 5;          // tổng số enemy trong phòng

    [Header("Spawn Control")]
    public bool spawnOnStart = false;   // phòng đầu thì true
    public bool useFixedSpawnPoints = false;
    public Transform[] spawnPoints;     // nếu muốn tự đặt điểm spawn

    int aliveCount = 0;
    bool hasSpawned = false;

    BoxCollider2D area;   // collider bao cả phòng (dùng trigger để detect Player)

    void Awake()
    {
        area = GetComponent<BoxCollider2D>();

        // AUTO: nếu quên gán room trong Inspector thì tự tìm Room cha
        if (room == null)
        {
            room = GetComponentInParent<Room>();
            if (room == null)
            {
                Debug.LogWarning($"[MonsterSpawner] Không tìm thấy Room cha cho {name}!");
            }
        }
    }

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnEnemies();
            // phòng start: hiện tại KHÔNG đóng cửa
        }
    }

    void SpawnEnemies()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        aliveCount = 0;

        

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPos = GetSpawnPosition(i);
            GameObject prefab = GetRandomEnemyPrefab();
            if (prefab == null) continue;

            GameObject e = Instantiate(prefab, spawnPos, Quaternion.identity);
            Enemy enemy = e.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnDeath += OnMonsterDie;
                aliveCount++;
            }
            else
            {
                Debug.LogError($"[MonsterSpawner] Prefab {prefab.name} không có component Enemy!");
            }
        }

        

        // Đóng cửa khi vào phòng (trừ phòng đầu)
        if (!spawnOnStart && room != null && aliveCount > 0)
        {
            room.CloseAllDoors();
            
        }
    }

    Vector3 GetSpawnPosition(int index)
    {
        // Nếu muốn dùng các điểm spawn cố định
        if (useFixedSpawnPoints && spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform p = spawnPoints[index % spawnPoints.Length];
            return p.position;
        }

        // Random trong BoxCollider2D của Spawner
        if (area != null)
        {
            Vector2 size = area.size;
            Vector2 center = (Vector2)transform.position + area.offset;

            float rx = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
            float ry = Random.Range(center.y - size.y / 2f, center.y + size.y / 2f);

            return new Vector3(rx, ry, 0f);
        }

        // fallback
        return transform.position;
    }

    GameObject GetRandomEnemyPrefab()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
           
            return null;
        }

        int idx = Random.Range(0, enemyPrefabs.Length);
        return enemyPrefabs[idx];
    }

    void OnMonsterDie()
    {
        aliveCount--;
       

        if (aliveCount <= 0 && room != null)
        {
      
            room.OnRoomCleared();  
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           
            if (enemyPrefabs.Length == 1)
            {
                MusicManager.Instance.PlayBossMusic();
            }
            else
            {
                MusicManager.Instance.PlayGameplayMusic(); 
            }

            if (!hasSpawned)
                SpawnEnemies();
        }
    }
}

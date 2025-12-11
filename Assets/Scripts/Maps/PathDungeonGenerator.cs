using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathDungeonGenerator : MonoBehaviour
{
    [Header("Room Settings")]
    public GameObject roomPrefab;
    public int roomCount = 7;

    [Header("Grid Settings")]
    public bool autoDetectRoomSize = true;   
    public int roomWidth = 20;              
    public int roomHeight = 20;             

    [Header("Boss Settings")]
    public GameObject bossPrefab;   

    [Header("Enemy Scaling")]
    public int baseEnemyCount = 5;         
    public int enemyIncrementPerRoom = 3;  

    Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();
    Room startRoom;
    Room lastRoom;

    void Start()
    {
        CalculateRoomSize();
        GenerateDungeon();
    }

    void CalculateRoomSize()
    {
     
        if (!autoDetectRoomSize)
        {
            if (roomWidth == 0) roomWidth = 1;
            if (roomHeight == 0) roomHeight = 1;
            return;
        }

        if (roomPrefab == null)
        {
            Debug.LogError("PathDungeonGenerator: Chưa gán roomPrefab!");
            return;
        }

        Tilemap tilemap = roomPrefab.GetComponentInChildren<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("PathDungeonGenerator: Không tìm thấy Tilemap trong RoomPrefab!");
            return;
        }

        var size = tilemap.cellBounds.size;
        roomWidth = size.x;
        roomHeight = size.y;
    }

    void GenerateDungeon()
    {
        rooms.Clear();

        Vector2Int currentIndex = Vector2Int.zero;
        startRoom = CreateRoom(currentIndex);

        // Phòng start là phòng thứ 0 trên đường đi
        ConfigureMonsterSpawnerForPathIndex(startRoom, 0);

        lastRoom = startRoom;

        // spawner phòng start (nếu có) – vẫn để spawnOnStart false, chỉ spawn khi player bước vào
        MonsterSpawner startSpawner = startRoom.GetComponentInChildren<MonsterSpawner>();
        if (startSpawner != null)
        {
            startSpawner.spawnOnStart = false;
        }

        // Tạo các phòng còn lại trên đường đi
        for (int i = 1; i < roomCount; i++)
        {
            bool placed = false;
            int safety = 100;

            while (!placed && safety-- > 0)
            {
                Vector2Int dir = GetRandomDirection();
                Vector2Int nextIndex = currentIndex + dir;

                if (rooms.ContainsKey(nextIndex))
                    continue;

                Room nextRoom = CreateRoom(nextIndex);

                // nối 2 phòng với nhau theo cả 2 chiều
                ConnectRooms(rooms[currentIndex], nextRoom, dir);

                currentIndex = nextIndex;
                lastRoom = nextRoom;
                placed = true;

                // i = thứ tự phòng trên đường đi (0 = start, 1 = phòng thứ 2, ...)
                ConfigureMonsterSpawnerForPathIndex(nextRoom, i);
            }

            if (!placed)
            {
                Debug.LogWarning("PathDungeonGenerator: không thể đặt thêm phòng (bị kẹt).");
                break;
            }
        }

        SetupBossRoom();

   
        if (startRoom != null)
            startRoom.OpenAllConnectedDoors();
    }

    Room CreateRoom(Vector2Int index)
    {
        Vector3 pos = new Vector3(index.x * roomWidth, index.y * roomHeight, 0f);

        GameObject roomObj = Instantiate(roomPrefab, pos, Quaternion.identity);
        Room room = roomObj.GetComponent<Room>();
        room.RoomIndex = index;

        rooms[index] = room;
        return room;
    }

    void ConnectRooms(Room a, Room b, Vector2Int dir)
    {
        // a ---dir---> b
        a.OpenDoor(dir);          // đánh dấu connected...
        b.OpenDoor(-dir);

        // ...và gán neighbor để đóng/mở cửa 2 phía
        a.SetNeighbor(b, dir);
        b.SetNeighbor(a, -dir);
    }

    Vector2Int GetRandomDirection()
    {
        int r = Random.Range(0, 4);
        switch (r)
        {
            case 0: return Vector2Int.up;
            case 1: return Vector2Int.down;
            case 2: return Vector2Int.left;
            default: return Vector2Int.right;
        }
    }

    void SetupBossRoom()
    {
        if (bossPrefab == null || lastRoom == null)
            return;

        MonsterSpawner sp = lastRoom.GetComponentInChildren<MonsterSpawner>();
        if (sp == null)
        {
            GameObject spObj = new GameObject("BossSpawner");
            spObj.transform.SetParent(lastRoom.transform, false);
            spObj.transform.localPosition = Vector3.zero;
            sp = spObj.AddComponent<MonsterSpawner>();
            sp.room = lastRoom;

            BoxCollider2D col = spObj.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(roomWidth, roomHeight);
        }

        // Phòng cuối là Boss: chỉ spawn boss, 1 con
        sp.spawnOnStart = false;
        sp.enemyPrefabs = new GameObject[] { bossPrefab };
        sp.enemyCount = 1;
    }

    /// <summary>
    /// Cấu hình số lượng quái cho phòng dựa trên thứ tự trên đường đi.
    /// pathIndex = 0 → phòng đầu = 5 quái
    /// pathIndex = 1 → 5 + 3
    /// pathIndex = 2 → 5 + 3*2 ...
    /// </summary>
    void ConfigureMonsterSpawnerForPathIndex(Room room, int pathIndex)
    {
        if (room == null) return;

        MonsterSpawner sp = room.GetComponentInChildren<MonsterSpawner>();
        if (sp == null) return;

        // ===== Phòng đầu tiên: KHÔNG có quái =====
        if (pathIndex == 0)
        {
            sp.enemyCount = 0;          // không spawn con nào
            sp.spawnOnStart = false;    // vẫn để false, chỉ spawn khi player bước vào
            return;
        }

        // ===== Các phòng còn lại: tăng dần số quái =====
        int count = baseEnemyCount + enemyIncrementPerRoom * pathIndex;
        if (count < 0) count = 0;

        sp.enemyCount = count;
        sp.spawnOnStart = false;
    }

}


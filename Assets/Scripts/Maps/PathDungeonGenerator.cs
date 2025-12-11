using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathDungeonGenerator : MonoBehaviour
{
    [Header("Room Settings")]
    public GameObject roomPrefab;

    [Tooltip("Số phòng tối thiểu muốn tạo")]
    public int minRoomCount = 5;

    [Tooltip("Số phòng tối đa muốn tạo")]
    public int maxRoomCount = 10;

   
    private int roomCount;

    [Header("Grid Settings")]
 
    public int roomWidth = 85;
    public int roomHeight = 85;

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
        // 1) Tính kích thước room
        CalculateRoomSize();

        // 2) Random số phòng trong khoảng [min, max]
        roomCount = GetRandomRoomCount();

        Debug.Log($"[PathDungeonGenerator] Generate dungeon với {roomCount} rooms");

        // 3) Generate
        GenerateDungeon();
    }

    int GetRandomRoomCount()
    {
        // Đảm bảo min <= max và >= 1
        int min = Mathf.Max(1, minRoomCount);
        int max = Mathf.Max(min, maxRoomCount);

        // Random.Range với int: max là exclusive → +1
        return Random.Range(min, max + 1);
    }

    void CalculateRoomSize()
    {
        if (roomWidth <= 0) roomWidth = 1;
        if (roomHeight <= 0) roomHeight = 1;

        Debug.Log($"[PathDungeonGenerator] RoomSize = {roomWidth} x {roomHeight}");
    }

    void GenerateDungeon()
    {
        rooms.Clear();

        Vector2Int currentIndex = Vector2Int.zero;
        startRoom = CreateRoom(currentIndex);

        // Phòng start là phòng thứ 0 trên đường đi
        ConfigureMonsterSpawnerForPathIndex(startRoom, 0);
        lastRoom = startRoom;

        // spawner phòng start – chỉ spawn khi player bước vào
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

        // Phòng đầu mở cửa sẵn
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
        a.OpenDoor(dir);
        b.OpenDoor(-dir);

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

        sp.spawnOnStart = false;
        sp.enemyPrefabs = new GameObject[] { bossPrefab };
        sp.enemyCount = 1;
    }

    void ConfigureMonsterSpawnerForPathIndex(Room room, int pathIndex)
    {
        if (room == null) return;

        MonsterSpawner sp = room.GetComponentInChildren<MonsterSpawner>();
        if (sp == null) return;

        // Phòng đầu tiên: không có quái
        if (pathIndex == 0)
        {
            sp.enemyCount = 0;
            sp.spawnOnStart = false;
            return;
        }

        // Các phòng còn lại: số quái tăng dần
        int count = baseEnemyCount + enemyIncrementPerRoom * pathIndex;
        if (count < 0) count = 0;

        sp.enemyCount = count;
        sp.spawnOnStart = false;
    }
}

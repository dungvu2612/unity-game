using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int RoomIndex;

    public GameObject doorUp;
    public GameObject doorDown;
    public GameObject doorLeft;
    public GameObject doorRight;

    // hướng nào THỰC SỰ có phòng bên cạnh
    public bool connectedUp;
    public bool connectedDown;
    public bool connectedLeft;
    public bool connectedRight;

    // Phòng bên cạnh
    [HideInInspector] public Room upRoom;
    [HideInInspector] public Room downRoom;
    [HideInInspector] public Room leftRoom;
    [HideInInspector] public Room rightRoom;

    bool cleared = false;

    // Được gọi bởi PathDungeonGenerator khi tạo hành lang giữa 2 phòng
    // ❗ Chỉ đánh dấu kết nối, KHÔNG mở hay đóng cửa ở đây
    public void OpenDoor(Vector2Int dir)
    {
        if (dir == Vector2Int.up)
        {
            connectedUp = true;
        }
        else if (dir == Vector2Int.down)
        {
            connectedDown = true;
        }
        else if (dir == Vector2Int.left)
        {
            connectedLeft = true;
        }
        else if (dir == Vector2Int.right)
        {
            connectedRight = true;
        }
    }

    // Gán reference sang phòng bên cạnh – dùng trong PathDungeonGenerator.ConnectRooms
    public void SetNeighbor(Room neighbor, Vector2Int dir)
    {
        if (dir == Vector2Int.up)
        {
            upRoom = neighbor;
        }
        else if (dir == Vector2Int.down)
        {
            downRoom = neighbor;
        }
        else if (dir == Vector2Int.left)
        {
            leftRoom = neighbor;
        }
        else if (dir == Vector2Int.right)
        {
            rightRoom = neighbor;
        }
    }

    // Đóng TẤT CẢ các cửa có nối phòng bên cạnh (cả 2 phía hành lang)
    public void CloseAllDoors()
    {
        // UP
        if (connectedUp)
        {
            if (doorUp != null) doorUp.SetActive(true);
            if (upRoom != null && upRoom.doorDown != null)
                upRoom.doorDown.SetActive(true);
        }

        // DOWN
        if (connectedDown)
        {
            if (doorDown != null) doorDown.SetActive(true);
            if (downRoom != null && downRoom.doorUp != null)
                downRoom.doorUp.SetActive(true);
        }

        // LEFT
        if (connectedLeft)
        {
            if (doorLeft != null) doorLeft.SetActive(true);
            if (leftRoom != null && leftRoom.doorRight != null)
                leftRoom.doorRight.SetActive(true);
        }

        // RIGHT
        if (connectedRight)
        {
            if (doorRight != null) doorRight.SetActive(true);
            if (rightRoom != null && rightRoom.doorLeft != null)
                rightRoom.doorLeft.SetActive(true);
        }
    }

    // Mở lại các cửa có nối phòng bên cạnh (cả 2 phía hành lang)
    public void OpenAllConnectedDoors()
    {
        // UP
        if (connectedUp)
        {
            if (doorUp != null) doorUp.SetActive(false);
            if (upRoom != null && upRoom.doorDown != null)
                upRoom.doorDown.SetActive(false);
        }

        // DOWN
        if (connectedDown)
        {
            if (doorDown != null) doorDown.SetActive(false);
            if (downRoom != null && downRoom.doorUp != null)
                downRoom.doorUp.SetActive(false);
        }

        // LEFT
        if (connectedLeft)
        {
            if (doorLeft != null) doorLeft.SetActive(false);
            if (leftRoom != null && leftRoom.doorRight != null)
                leftRoom.doorRight.SetActive(false);
        }

        // RIGHT
        if (connectedRight)
        {
            if (doorRight != null) doorRight.SetActive(false);
            if (rightRoom != null && rightRoom.doorLeft != null)
                rightRoom.doorLeft.SetActive(false);
        }
    }

    // Gọi khi quái trong phòng chết hết
    public void OnRoomCleared()
    {
        if (cleared) return;
        cleared = true;

        OpenAllConnectedDoors();
    }
}

using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorShoot;
    [SerializeField] private Texture2D cursorReload;
    [SerializeField] private Texture2D cursorNormal;
    private Vector2 hotspot = new Vector2(16, 48);
    void Start()
    {
        Cursor.SetCursor(cursorNormal, hotspot, CursorMode.Auto);
    }

    
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Cursor.SetCursor(cursorShoot, hotspot, CursorMode.Auto);
        }
        else if (Input.GetMouseButton(1))
        {
            Cursor.SetCursor(cursorReload, hotspot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(cursorNormal, hotspot, CursorMode.Auto);
        }

    }
}

using UnityEngine;

public class ChestWinPickup : MonoBehaviour
{
    [SerializeField] private bool destroyOnPickup = true;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // chỉ xử lý lần đầu
        if (collected) return;

        if (collision.CompareTag("Player"))
        {
            collected = true;

            // Gọi màn hình Win
            if (PauseMenuManager.Instance != null)
            {
                PauseMenuManager.Instance.ShowWinScreen();
            }
            else
            {
                Debug.LogWarning("[ChestWinPickup] Không tìm thấy PauseMenuManager.Instance!");
            }

            // Nếu muốn rương biến mất sau khi nhặt
            if (destroyOnPickup)
            {
                Destroy(gameObject);
            }
        }
    }
}

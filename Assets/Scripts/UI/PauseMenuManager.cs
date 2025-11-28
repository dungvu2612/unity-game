using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Roots")]
    [SerializeField] private GameObject hudRoot;        // HUD: máu, mana, ammo...
    [SerializeField] private GameObject pauseMenuRoot;  // GamePause (màn hình pause)
    [SerializeField] private GameObject gameOverRoot;   // (nếu có, có thể để null)
    public static PauseMenuManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    // ========== NÚT PAUSE TRÊN HUD ==========
    public void OnPauseButtonClicked()
    {
        // Dừng game
        Time.timeScale = 0f;

        // Bật màn pause, tắt HUD
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(true);
        if (hudRoot != null) hudRoot.SetActive(false);
    }

    // ========== NÚT RESUME TRÊN MÀN PAUSE ==========
    public void OnResumeButtonClicked()
    {
        // Chạy lại game
        Time.timeScale = 1f;

        // Tắt màn pause, bật HUD
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
        if (hudRoot != null) hudRoot.SetActive(true);
    }

    // ========== NÚT RETRY (PAUSE HOẶC GAMEOVER) ==========
    public void OnRetryButtonClicked()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
    // ========== NÚT QUIT ==========
    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ========== HÀM GỌI KHI PLAYER CHẾT (GAME OVER) ==========
    public void ShowGameOver()
    {
        Time.timeScale = 0f;
            
        if (hudRoot != null) hudRoot.SetActive(false);
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
        if (gameOverRoot != null) gameOverRoot.SetActive(true);
    }
}

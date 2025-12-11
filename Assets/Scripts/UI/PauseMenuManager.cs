using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Roots")]
    [SerializeField] private GameObject hudRoot;      
    [SerializeField] private GameObject pauseMenuRoot;  
    [SerializeField] private GameObject gameOverRoot;   
    [SerializeField] private GameObject winRoot;
    [Header("Summary UI")]
    [SerializeField] private TMP_Text timePlayedText;
    public static PauseMenuManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    public void OnPauseButtonClicked()
    {
        // Dừng game
        Time.timeScale = 0f;

        // Bật màn pause, tắt HUD
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(true);
        if (hudRoot != null) hudRoot.SetActive(false);
    }

    
    public void OnResumeButtonClicked()
    {
        // Chạy lại game
        Time.timeScale = 1f;

        // Tắt màn pause, bật HUD
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
        if (hudRoot != null) hudRoot.SetActive(true);
    }

    public void OnRetryButtonClicked()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }
    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

   
    public void ShowGameOver()
    {
        UpdateTimePlayedText();

        Time.timeScale = 0f;
            
        if (hudRoot != null) hudRoot.SetActive(false);
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
        if (gameOverRoot != null) gameOverRoot.SetActive(true);
    }
    public void ShowWinScreen()
    {
        UpdateTimePlayedText(); 
        Time.timeScale = 0f;

        if (hudRoot != null) hudRoot.SetActive(false);
        if (pauseMenuRoot != null) pauseMenuRoot.SetActive(false);
        if (gameOverRoot != null) gameOverRoot.SetActive(false);
        if (winRoot != null) winRoot.SetActive(true);
    }
    private void UpdateTimePlayedText()
    {
        if (timePlayedText == null) return;

        // thời gian đã chơi từ lúc vào scene gameplay
        float t = Time.timeSinceLevelLoad;

        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);

        // format kiểu 01:23
        timePlayedText.text = $"{minutes:00}:{seconds:00}";
    }

}


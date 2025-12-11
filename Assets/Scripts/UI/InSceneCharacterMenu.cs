using UnityEngine;

public class InSceneMenuManager : MonoBehaviour
{
    [Header("Menu Roots")]
    [SerializeField] private GameObject homeMenuRoot;
    [SerializeField] private GameObject settingMenuRoot;
    [SerializeField] private GameObject gameplayRoot;

    [Header("References")]
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private CharacterSelectionMenu characterSelectionMenu;

    private void Awake()
    {
        // Trạng thái khi vào scene
        if (homeMenuRoot != null) homeMenuRoot.SetActive(true);
        if (settingMenuRoot != null) settingMenuRoot.SetActive(false);
        if (gameplayRoot != null) gameplayRoot.SetActive(false);
    }

    private void Start()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMenuMusic();
    }


    public void OnPlayButtonClicked()
    {
        if (characterSelectionMenu == null)
        {
            Debug.LogError("[InSceneMenuManager] Chưa gán CharacterSelectionMenu!");
            return;
        }

        MenuCharacterLink link = characterSelectionMenu.CurrentLink;
        if (link == null || link.gameplayPrefab == null)
        {
            Debug.LogError("[InSceneMenuManager] Current character chưa có MenuCharacterLink / gameplayPrefab!");
            return;
        }

        if (playerSpawner == null)
        {
            Debug.LogError("[InSceneMenuManager] Chưa kéo PlayerSpawner!");
            return;
        }

        GameObject player = playerSpawner.Spawn(link.gameplayPrefab);
        if (player == null)
        {
            Debug.LogError("[InSceneMenuManager] Spawn player thất bại!");
            return;
        }

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayGameplayMusic();

        if (homeMenuRoot != null) homeMenuRoot.SetActive(false);
        if (settingMenuRoot != null) settingMenuRoot.SetActive(false);
        if (gameplayRoot != null) gameplayRoot.SetActive(true);
    }

 
    public void OnOpenSettingClicked()
    {
        if (homeMenuRoot != null) homeMenuRoot.SetActive(false);
        if (settingMenuRoot != null) settingMenuRoot.SetActive(true);
    }

    public void OnCloseSettingClicked()
    {
        if (settingMenuRoot != null) settingMenuRoot.SetActive(false);
        if (homeMenuRoot != null) homeMenuRoot.SetActive(true);
    }


    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InSceneMenuManager : MonoBehaviour
{
    [Header("Menu Root & Gameplay Root")]
    [SerializeField] private GameObject homeMenuRoot;   // Panel menu (HomeMenu)
    [SerializeField] private GameObject gameplayRoot;   // GamePlayRoot (map, enemy, spawner...)

    [Header("Character List (UI)")]
    [SerializeField] private Transform charactersParent;   // Panel chứa các nhân vật (Witch, Knight,...)
    private GameObject[] characters;
    private int currentIndex = 0;

    [Header("UI Texts")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text spText;
    [SerializeField] private TMP_Text mpText;

   

    [Header("Spawner")]
    [SerializeField] private PlayerSpawner playerSpawner;  // object PlayerSpawner trong GamePlayRoot

    private void Awake()
    {
        // Lấy list nhân vật trong panel
        if (charactersParent == null)
        {
            Debug.LogError("Chưa gán charactersParent cho InSceneMenuManager!");
            return;
        }

        int childCount = charactersParent.childCount;
        characters = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            characters[i] = charactersParent.GetChild(i).gameObject;
        }

        // Khi mới vào scene: bật menu, tắt gameplay
        if (homeMenuRoot != null) homeMenuRoot.SetActive(true);
        if (gameplayRoot != null) gameplayRoot.SetActive(false);
    }

    private void Start()
    { 
        ShowCharacter(currentIndex);
        MusicManager.Instance.PlayMenuMusic();

    }

    // ======================= HIỂN THỊ NHÂN VẬT =======================

    private void ShowCharacter(int index)
    {
        if (characters == null || characters.Length == 0) return;

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == index);
        }

        UpdateUI(index);
    }

    private void UpdateUI(int index)
    {
        if (characters == null || index < 0 || index >= characters.Length)
            return;

        GameObject character = characters[index];

        MenuCharacterStats stats = character.GetComponent<MenuCharacterStats>();

        if (stats != null)
        {
            if (nameText != null) nameText.text = stats.characterName;
            if (hpText != null) hpText.text = stats.HP.ToString();
            if (spText != null) spText.text = stats.SP.ToString();
            if (mpText != null) mpText.text = stats.MP.ToString();
        }
        else
        {
            Debug.LogWarning("MenuCharacterStats chưa được gắn vào " + character.name);
        }

        // Gán avatar nhân vật vào Image trong panel
       
    }

    public void OnNextButtonClicked()
    {
        if (characters == null || characters.Length == 0) return;

        currentIndex = (currentIndex + 1) % characters.Length;
        ShowCharacter(currentIndex);
    }

    public void OnPrevButtonClicked()
    {
        if (characters == null || characters.Length == 0) return;

        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        ShowCharacter(currentIndex);
    }

    // ======================= NÚT PLAY =======================

    public void OnPlayButtonClicked()
    {
        if (characters == null || characters.Length == 0) return;

        GameObject menuCharacter = characters[currentIndex];

        // Lấy stats từ MenuCharacterStats
        MenuCharacterStats stats = menuCharacter.GetComponent<MenuCharacterStats>();

        if (stats == null || stats.gameplayPrefab == null)
        {
            Debug.LogError("Chưa gán MenuCharacterStats hoặc gameplayPrefab cho " + menuCharacter.name);
            return;
        }

        if (playerSpawner == null)
        {
            Debug.LogError("Chưa kéo PlayerSpawner vào InSceneMenuManager!");
            return;
        }

        // Spawn nhân vật gameplay
        GameObject player = playerSpawner.Spawn(stats.gameplayPrefab);

        if (player == null)
        {
            Debug.LogError("Spawn player thất bại!");
            return;
        }
        MusicManager.Instance.PlayGameplayMusic();
        // Bật gameplay, tắt menu
        if (homeMenuRoot != null) homeMenuRoot.SetActive(false);
        if (gameplayRoot != null) gameplayRoot.SetActive(true);
    }

    // ======================= NÚT QUIT =======================

    public void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

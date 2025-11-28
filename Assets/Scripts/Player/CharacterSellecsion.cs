// CharacterSelection.cs
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    [Header("Characters Parent")]
    [SerializeField] private Transform charactersParent;   // Panel/Characters

    private GameObject[] characters;
    private int currentIndex = 0;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;   // HP
    [SerializeField] private TMP_Text spText;   // Armor
    [SerializeField] private TMP_Text mpText;   // Mana

    [Header("Menus")]
    [SerializeField] private GameObject homeMenu;    // panel chính (Play / Setting / Quit + khung stats)
    [SerializeField] private GameObject settingMenu; // panel setting (nếu có)

    private void Start()
    {
        if (charactersParent == null)
        {
            Debug.LogError("CharacterSelection: charactersParent chưa gán!");
            return;
        }

        // Lấy các child trong Characters làm danh sách nhân vật
        int count = charactersParent.childCount;
        characters = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            characters[i] = charactersParent.GetChild(i).gameObject;
        }

        // Lấy index đã chọn lần trước (nếu có) và clamp cho an toàn
        currentIndex = Mathf.Clamp(CharacterData.SelectedCharacterIndex, 0, characters.Length - 1);

        ShowCharacter(currentIndex);

        if (homeMenu != null) homeMenu.SetActive(true);
        if (settingMenu != null) settingMenu.SetActive(false);
    }

    // Bật đúng 1 nhân vật và update UI
    private void ShowCharacter(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == index);
        }

        UpdateUI(index);
    }

    // Cập nhật Name, HP, SP, MP trên panel
    private void UpdateUI(int index)
    {
        GameObject character = characters[index];
        MenuCharacterStats stats = character.GetComponent<MenuCharacterStats>();

        if (stats == null)
        {
            Debug.LogWarning($"CharacterSelection: {character.name} chưa có MenuCharacterStats!");
            return;
        }

        if (nameText != null)
            nameText.text = stats.characterName;

        if (hpText != null)
            hpText.text = stats.HP.ToString();

        if (spText != null)
            spText.text = stats.MP.ToString();

        if (mpText != null)
            mpText.text = stats.SP.ToString();
    }

    // ========== Nút Next / Prev trên UI ==========

    public void NextCharacter()
    {
        if (characters == null || characters.Length == 0) return;

        currentIndex = (currentIndex + 1) % characters.Length;
        ShowCharacter(currentIndex);
    }

    public void PreviousCharacter()
    {
        if (characters == null || characters.Length == 0) return;

        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        ShowCharacter(currentIndex);
    }

    // ========== Nút Play ==========

    public void Play()
    {
        // Lưu lại index nhân vật đã chọn
        CharacterData.SelectedCharacterIndex = currentIndex;

        // Load sang scene game (đổi tên cho đúng scene của bạn)
        SceneManager.LoadScene("SampleScene");   // hoặc "GamePlay" tuỳ bạn
    }

    // ========== Nút Setting / Back ==========

    public void OpenSetting()
    {
        if (settingMenu != null) settingMenu.SetActive(true);
        if (homeMenu != null) homeMenu.SetActive(false);
    }

    public void CloseSetting()
    {
        if (settingMenu != null) settingMenu.SetActive(false);
        if (homeMenu != null) homeMenu.SetActive(true);
    }

    // ========== Nút Quit ==========

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

using TMPro;
using UnityEngine;

public class CharacterSelectionMenu : MonoBehaviour
{
    [Header("Characters Parent")]
    [SerializeField] private Transform charactersParent;   // parent chứa các model nhân vật trong menu

    private GameObject[] characters;
    private int currentIndex = 0;

    [Header("UI Texts")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text spText;
    [SerializeField] private TMP_Text mpText;

    
    public MenuCharacterLink CurrentLink
    {
        get
        {
            if (characters == null || characters.Length == 0) return null;

            GameObject character = characters[currentIndex];
            return character.GetComponent<MenuCharacterLink>();
        }
    }

    private void Awake()
    {
        if (charactersParent == null)
        {
            Debug.LogError("[CharacterSelectionMenu] Chưa gán charactersParent!");
            return;
        }
        //so luong characters trong panel
        int count = charactersParent.childCount;
        characters = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            characters[i] = charactersParent.GetChild(i).gameObject;
        }
    }

    private void Start()
    {
        if (characters == null || characters.Length == 0) return;

        ShowCharacter(currentIndex);
    }

  
    private void ShowCharacter(int index)
    {
        if (characters == null || characters.Length == 0) return;

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(i == index);
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        MenuCharacterLink link = CurrentLink;
        if (link == null || link.gameplayPrefab == null)
        {
            Debug.LogWarning("[CharacterSelectionMenu] MenuCharacterLink / gameplayPrefab chưa gán!");
            return;
        }

        Player stats = link.gameplayPrefab.GetComponent<Player>();
        if (stats == null)
        {
            Debug.LogWarning("[CharacterSelectionMenu] Prefab " + link.gameplayPrefab.name + " không có Player!");
            return;
        }

        if (nameText != null) nameText.text = stats.CharacterName;
        if (hpText != null) hpText.text = stats.MaxHP.ToString();
        if (spText != null) spText.text = stats.MaxSP.ToString(); // SP = Armor
        if (mpText != null) mpText.text = stats.MaxMP.ToString();
    }

    // ==== Nút Next / Prev gắn trực tiếp từ Button OnClick ====
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
}

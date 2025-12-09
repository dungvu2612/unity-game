using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterInfo
    {
        public string id;
        public string displayName;

        [Header("Stats")]
        public int maxHP;
        public int maxMP;
        public int armor;

        [Header("Visual / Prefab")]
        public Sprite portrait;
        public GameObject playerPrefab;
    }

    [Header("Character Selection")]
    [SerializeField] private CharacterInfo[] characters;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI spText;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private Image portraitImage;

    private int selectedIndex = 0;

    public static GameObject SelectedPlayerPrefab { get; private set; }

    private void Start()
    {
        if (characters == null || characters.Length == 0)
        {
            Debug.LogWarning("No characters configured in GameManager!");
            return;
        }

        selectedIndex = Mathf.Clamp(selectedIndex, 0, characters.Length - 1);
        UpdateUI();
    }

    public void OnClickNext()
    {
        if (characters == null || characters.Length == 0) return;

        selectedIndex++;
        if (selectedIndex >= characters.Length)
            selectedIndex = 0;

        UpdateUI();
    }

    public void OnClickPrev()
    {
        if (characters == null || characters.Length == 0) return;

        selectedIndex--;
        if (selectedIndex < 0)
            selectedIndex = characters.Length - 1;

        UpdateUI();
    }

    public void OnClickPlay()
    {
        if (characters == null || characters.Length == 0) return;

        CharacterInfo c = characters[selectedIndex];

        if (c.playerPrefab == null)
        {
            Debug.LogError("Selected character has no playerPrefab assigned!");
            return;
        }

        SelectedPlayerPrefab = c.playerPrefab;

        // ĐỔI tên scene này thành scene gameplay thật của bạn
        SceneManager.LoadScene("SimpleScene");
    }

    private void UpdateUI()
    {
        CharacterInfo c = characters[selectedIndex];

        if (nameText != null)
            nameText.text = c.displayName;

        if (hpText != null)
            hpText.text = c.maxHP.ToString();

        if (spText != null)
            spText.text = c.armor.ToString();

        if (mpText != null)
            mpText.text = c.maxMP.ToString();

        if (portraitImage != null)
            portraitImage.sprite = c.portrait;
    }
}

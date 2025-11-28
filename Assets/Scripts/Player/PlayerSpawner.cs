using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerSpawner : MonoBehaviour
{
    [Header("Vị trí spawn (object rỗng)")]
    [SerializeField] private Transform spawnPoint;   // PlayerSpawnPoint

    [Header("Camera Follow Target")]
    [SerializeField] private Transform cameraFollowTarget;   // CameraFollowTarget (Cinemachine follow cái này)

    [Header("HUD (UI Bars)")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image armorBar;
    [SerializeField] private Image manaBar;
    [Header("HUD (Ammo)")]
    [SerializeField] private TextMeshProUGUI ammoText;
    private void Awake()
    {
        // Nếu quên gán spawnPoint trong Inspector thì dùng transform hiện tại
        if (spawnPoint == null)
        {
            spawnPoint = transform;
            Debug.Log("[PlayerSpawner] spawnPoint null, dùng transform của PlayerSpawner");
        }

        // Nếu quên gán CameraFollowTarget thì tự tìm theo tên trong scene
        if (cameraFollowTarget == null)
        {
            GameObject go = GameObject.Find("CameraFollowTarget");
            if (go != null)
            {
                cameraFollowTarget = go.transform;
                Debug.Log("[PlayerSpawner] Tự tìm CameraFollowTarget trong scene");
            }
            else
            {
                Debug.LogWarning("[PlayerSpawner] KHÔNG tìm thấy object 'CameraFollowTarget'");
            }
        }
    }

    public GameObject Spawn(GameObject playerPrefab)
    {
        Debug.Log("[PlayerSpawner] Spawn() được gọi");

        if (playerPrefab == null)
        {
            Debug.LogError("[PlayerSpawner] playerPrefab = null!");
            return null;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("[PlayerSpawner] spawnPoint null trong Spawn!");
            return null;
        }

        // 1. Spawn player làm con của spawnPoint
        GameObject player = Instantiate(
            playerPrefab,
            spawnPoint.position,
            Quaternion.identity,
            spawnPoint
        );

        Debug.Log("[PlayerSpawner] Đã spawn player: " + player.name);

        // 2. CameraFollowTarget -> làm con của player
        if (cameraFollowTarget != null)
        {
            cameraFollowTarget.SetParent(player.transform);
            cameraFollowTarget.localPosition = Vector3.zero;

            Debug.Log("[PlayerSpawner] ĐÃ set CameraFollowTarget làm con của " + player.name);
        }
        else
        {
            Debug.LogWarning("[PlayerSpawner] cameraFollowTarget = NULL trong Spawn!");
        }

        // 3. GÁN UI BARS CHO PLAYER (DÙNG BASE CLASS Player)
        Player playerComp = player.GetComponent<Player>();

        if (playerComp != null)
        {
            playerComp.SetupUIBars(hpBar, armorBar, manaBar);
            Debug.Log("[PlayerSpawner] Đã gán UI Bars cho " + player.name);
        }
        else
        {
            Debug.LogWarning("[PlayerSpawner] Player không có component kế thừa class Player!");
        }
        WitchWeapon weapon = player.GetComponentInChildren<WitchWeapon>();
        if (weapon != null)
        {
            weapon.SetAmmoText(ammoText);
            Debug.Log("[PlayerSpawner] Đã gán AmmoText cho " + weapon.name);
        }
        else
        {
            Debug.LogWarning("[PlayerSpawner] Không tìm thấy WitchWeapon trong player " + player.name);
        }

        return player;

    }
}

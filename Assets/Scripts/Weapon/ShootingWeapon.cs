using TMPro;
using UnityEngine;
using System.Collections;

public class WitchWeapon : MonoBehaviour
{
    private float rotateOffset = 180f;

    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shotDelay = 0.15f;
    private float nextShot;

    [SerializeField] private int maxAmmo = 24;
    public int currentAmmo;

    // Chỉ để xem trong Inspector, sẽ được gán bằng code
    [SerializeField] private TextMeshProUGUI ammoText;

    [SerializeField] private float reloadTime = 1.5f;
    private bool isReloading = false;
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private AudioClip reloadSFX;   
    [SerializeField] private AudioClip emptySFX;
    private AudioSource audioSource;

    // === Cho Spawner gán UI Text từ bên ngoài ===
    public void SetAmmoText(TextMeshProUGUI text)
    {
        ammoText = text;
        UpdateAmmoText();
    }

    void Start()
    {
        currentAmmo = maxAmmo;
        audioSource = GetComponent<AudioSource>();
        UpdateAmmoText();
       
    }

    void Update()
    {
        RotateWeapon();
        Shooting();

        if (Input.GetMouseButtonDown(1))
        {
            StartReload();
        }
    }

    void RotateWeapon()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width ||
            Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
        {
            return;
        }

        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void Shooting()
    {
        if (isReloading) return;

        if (Input.GetMouseButton(0) && Time.time > nextShot && currentAmmo > 0)
        {
            if (currentAmmo <= 0)
            {
                
                if (emptySFX != null)
                    audioSource.PlayOneShot(emptySFX);
                return;
            }
            nextShot = Time.time + shotDelay;
            Instantiate(bulletPrefab, firePos.position, firePos.rotation);
            currentAmmo--;
            UpdateAmmoText();

            if (shootSFX != null)
                audioSource.PlayOneShot(shootSFX);
        }
    }

    private void StartReload()
    {
        if (isReloading) return;
        if (currentAmmo == maxAmmo) return;

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        if (reloadSFX != null)
            audioSource.PlayOneShot(reloadSFX);
        if (ammoText != null)
            ammoText.text = "...";
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoText();
     
    }

    private void UpdateAmmoText()
    {
        if (ammoText == null) return;

        if (currentAmmo > 0)
            ammoText.text = currentAmmo.ToString();
        else
            ammoText.text = "Empty";
    }
}

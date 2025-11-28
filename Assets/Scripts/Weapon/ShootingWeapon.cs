using TMPro;
using UnityEngine;
using System.Collections;

public class WitchWeapon : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField] private float rotationOffset = -90f;      // offset vì sprite súng mặc định quay lên
    [SerializeField] private float bulletRotationOffset = 0f;  // offset cho đạn
    [SerializeField] private SpriteRenderer gunSprite;         // SpriteRenderer của khẩu súng
    [SerializeField] private bool enableFlip = true;           // <--- ô tick trong Inspector

    [Header("Shoot")]
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shotDelay = 0.15f;
    private float nextShot;

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 24;
    public int currentAmmo;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private float reloadTime = 1.5f;
    private bool isReloading = false;

    [Header("SFX")]
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private AudioClip reloadSFX;
    [SerializeField] private AudioClip emptySFX;
    private AudioSource audioSource;

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

        if (gunSprite == null)
            gunSprite = GetComponent<SpriteRenderer>();
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

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = mouseWorld - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Xoay súng
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);

        // Chỉ lật nếu enableFlip = true
        if (gunSprite != null)
        {
            if (enableFlip)
            {
                float normalized = Mathf.DeltaAngle(0f, angle);   // -180 → 180
                bool aimingLeft = normalized > 90f || normalized < -90f;
                gunSprite.flipY = aimingLeft;
            }
            else
            {
                // Weapon này không dùng flip → luôn giữ nguyên
                gunSprite.flipY = false;
            }
        }
    }

    void Shooting()
    {
        if (isReloading) return;

        if (Input.GetMouseButton(0) && Time.time > nextShot)
        {
            if (currentAmmo <= 0)
            {
                if (emptySFX != null)
                    audioSource.PlayOneShot(emptySFX);
                return;
            }

            nextShot = Time.time + shotDelay;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = mouseWorld - firePos.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Quaternion bulletRot = Quaternion.Euler(0, 0, angle + bulletRotationOffset);
            Instantiate(bulletPrefab, firePos.position, bulletRot);

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

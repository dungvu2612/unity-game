using Unity.VisualScripting;
using UnityEngine;

public class WitchWeapon : MonoBehaviour
{
    private float rotateOffset = 180f;
    [SerializeField] private Transform firePos;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float shotDelay = 0.15f;
    private float nextShot;
    [SerializeField] private int maxAmmo = 24;
    public int currentAmmo;
    void Start()
    {
        currentAmmo = maxAmmo;   
    }
    void Update()
    {
        RotateWeapon();
        Shooting();
        ReLoadAmmo();
    }
    void RotateWeapon()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)
        { return; }
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; //tinhtoado 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //tinhgocxoay
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f); //quayvuvattheothegoc
    }    
    void Shooting()
    {
        if(Input.GetMouseButton(0) && Time.time > nextShot && currentAmmo > 0)
        {
            nextShot = Time.time + shotDelay;
            Instantiate(bulletPrefab, firePos.position, firePos.rotation);
            currentAmmo--;
        }
    }
    void ReLoadAmmo()
    {
        if(Input.GetMouseButtonDown(1) && currentAmmo < maxAmmo)
        {
            currentAmmo = maxAmmo;
        }
    }
}

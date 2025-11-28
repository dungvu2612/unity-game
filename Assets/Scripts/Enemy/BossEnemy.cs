using UnityEngine;

public class BossEnemy : Enemy
{
    [Header("BossEnemy Settings:")]
    [SerializeField] private float touchDamage = 10f;
    [SerializeField] private float stayDamage = 2f;
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private GameObject bulletSkillPrefabs;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float SpeedShooting=20f;
    [SerializeField] private float SpeedShootingSkill = 15f;
    [SerializeField] private float hpvalue = 100f;
    [SerializeField] private GameObject MiniEnemy;
    [SerializeField] private float SkillWaitTime = 2f;
    private float NextSkillTime = 0f ;
    [SerializeField] private GameObject Reward ;
    protected override void Update()
    {
        base.Update(); 
        if (Time.time >= NextSkillTime){
       UseSkill();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(touchDamage);
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(stayDamage);
        }

    }

    private void Shooting()
    {
        if(player != null)
        {
            Vector3 directionToPlayer = (player.transform.position - firePoint.position);
            directionToPlayer.Normalize();
            GameObject bullet = Instantiate(bulletPrefabs, firePoint.position, Quaternion.identity);
            EnemyBullet enemyBullet = bullet.AddComponent<EnemyBullet>();
            enemyBullet.SetDirection(directionToPlayer*SpeedShooting);
        }
    }
    private void ShootSkill()
    {
        const int bulletCount = 12;
        float angleStep = 360f / bulletCount;   
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
            GameObject bullet = Instantiate(bulletSkillPrefabs, firePoint.position, Quaternion.identity);
            EnemyBullet enemyBullet = bullet.AddComponent<EnemyBullet>();
            enemyBullet.SetDirection(direction * SpeedShootingSkill);
        }
    }
    private void Healing(float hpAmount)
    {
        currentHP = Mathf.Min(currentHP + hpAmount, maxHP);
        UpdateHpBar();
    }
    private void CreateMiniEnemy()
    {
        Instantiate(MiniEnemy, transform.position, Quaternion.identity);
    } 
    private void SpeedMove()
    {
        if (player != null)
        {
            transform.position = player.transform.position;
        }
    }
    private void RandomSkill()
    {
        int randomSkill = Random.Range(0, 5);
        switch(randomSkill)
        {
            case 0:
                Shooting();
                break;
            case 1:
                ShootSkill();
                break;
            case 2:
                CreateMiniEnemy();
                break;
            case 3:
                Healing(hpvalue);
                break;
            case 4:
                SpeedMove();
                break;
        }
    }
    protected override void Die()
    {
        Instantiate(Reward, transform.position, Quaternion.identity);
        base.Die();
    }
    private void UseSkill()
    {NextSkillTime = Time.time + SkillWaitTime;
        RandomSkill();
    }
}







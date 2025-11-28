using UnityEngine;

public class MiniEnemy : Enemy

{
    [Header("Enemy 01 Settings")]
    [SerializeField] private float touchDamage = 10f;
    [SerializeField] private float stayDame = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            player.TakeDamage(touchDamage);
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            player.TakeDamage(stayDame);
        }

    }
}

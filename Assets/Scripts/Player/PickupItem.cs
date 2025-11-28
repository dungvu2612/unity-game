using UnityEngine;


public enum PickupType
{
    Mana,
    Health,
    Speed
}

public class PickupItem : MonoBehaviour
{
    public PickupType type;

    [Header("Common Value")]
    public float amount = 20f;

    [Header("Speed Buff Time")]
    public float speedDuration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>()
                      ?? collision.GetComponentInParent<Player>();

        if (player == null) return;

        switch (type)
        {
            case PickupType.Mana:
                player.RestoreMana(amount);
                break;

            case PickupType.Health:
                player.RestoreHP(amount);
                break;

            case PickupType.Speed:
                player.AddSpeedBuff(amount, speedDuration);
                break;
        }

        Destroy(gameObject);
    }
}

using UnityEngine;

public class Witch : Player
{
    // Cho skill bên ngoài xài SpendMana của Player
    public bool TrySpendMana(float amount)
    {
        return SpendMana(amount);
    }
}

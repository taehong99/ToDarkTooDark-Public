using Tae;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHealingPotion", menuName = "Item/HealingPotion")]
public class HealingPotion : Tae.Inventory.BasePotion
{
    public float amountToHeal;
    public float duration;
    public bool isPercent;

    protected override void UsePotion(GameObject user)
    {
        if (!user.TryGetComponent(out PlayerHealth player))
            return;

        if (duration == 0)
        {
            player.Heal(amountToHeal, isPercent);
        }
        else
        {
            player.PotionHealOverTime(amountToHeal, duration);
        }
    }
}

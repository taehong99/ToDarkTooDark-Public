using UnityEngine;

[CreateAssetMenu(fileName = "NewStatIncreasePotion", menuName = "Item/StatIncreasePotion")]
public class StatIncreasePotion : Tae.Inventory.BasePotion
{
    public StatType statType;
    public float amount;
    public float duration;
    public bool isPercent;

    protected override void UsePotion(GameObject user)
    {
        if (!user.TryGetComponent(out StatusEffectManager player))
            return;

        Buff buff = new Buff("포션", duration, statType, amount, isPercent);
        player.AddEffect(buff);
    }
}
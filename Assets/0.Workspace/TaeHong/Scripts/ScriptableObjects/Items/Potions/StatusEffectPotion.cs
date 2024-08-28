using UnityEngine;

[CreateAssetMenu(fileName = "NewStatusEffectPotion", menuName = "Item/StatusEffectPotion")]
public class StatusEffectPotion : Tae.Inventory.BasePotion
{
    public string statusEffectName;
    public float duration;
    public float amount;

    protected override void UsePotion(GameObject user)
    {
        if (!user.TryGetComponent(out StatusEffectManager player))
            return;

        StatusEffect statusEffect;
        switch (statusEffectName)
        {
            case "Blind":
                statusEffect = new BlindnessEffect("", duration);
                break;
            case "Silence":
                statusEffect = new SilenceDebuff("", duration);
                break;
            case "Burn":
            	statusEffect = new DotDamage("", 5, .01f, true);
                break;
            case "Poison":
                statusEffect = new DotDamage("", 5, .02f, true);
                break;
            case "Shield":
                statusEffect = new Shield("", duration, amount);
                break;
            default:
                statusEffect = new BlindnessEffect("", 0);
                break;
        }

        player.AddEffect(statusEffect);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "NewTransformPotion", menuName = "Item/TransformPotion")]
public class TransformPotion : Tae.Inventory.BasePotion
{
    public TransformTarget transformTarget;
    public float duration;

    protected override void UsePotion(GameObject user)
    {
        if (!user.TryGetComponent(out StatusEffectManager player))
            return;

        StatusEffect statusEffect;
        switch (transformTarget)
        {
            case TransformTarget.Tree:
                statusEffect = new TreeForm("Tree", duration);
                break;
            case TransformTarget.Stone:
                statusEffect = new StoneForm("Stone", duration);
                break;
            default:
                statusEffect = new TreeForm("", 0);
                break;
        }

        player.AddEffect(statusEffect);
    }
}

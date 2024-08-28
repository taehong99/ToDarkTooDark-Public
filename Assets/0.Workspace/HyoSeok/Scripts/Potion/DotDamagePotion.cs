using UnityEngine;

public class DotDamagePotion : BasePotion
{
    [SerializeField] string name;
    [SerializeField] float precent;
    [SerializeField] GameObject particle;
    public override void UseItem()
    {
        if (isCool)
        {
        base.UseItem();
            DotDamage dot = new DotDamage(name, potionSO.duration, precent, true);
            gameObject.GetComponentInParent<StatusEffectManager>().AddEffect(dot);
        }
    }

}

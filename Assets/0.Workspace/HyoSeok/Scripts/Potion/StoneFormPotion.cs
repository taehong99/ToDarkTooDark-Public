using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneFormPotion : BasePotion
{
    [ContextMenu("Use Potion")]
    public override void UseItem()
    {
        if (isCool)
        {
        base.UseItem();
            StoneForm stoneForm = new StoneForm("돌 변신", potionSO.duration);
            gameObject.GetComponentInParent<StatusEffectManager>().AddEffect(stoneForm);
        }
    }
}

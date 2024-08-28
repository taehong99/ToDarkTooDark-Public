using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeFormPotion : BasePotion
{

    [ContextMenu("Use Potion")]
    public override void UseItem()
    {
        if (isCool)
        {
            base.UseItem();
            TreeForm treeForm = new TreeForm("나무 변신", potionSO.duration);
            gameObject.GetComponentInParent<StatusEffectManager>().AddEffect(treeForm);
        }
    }

}

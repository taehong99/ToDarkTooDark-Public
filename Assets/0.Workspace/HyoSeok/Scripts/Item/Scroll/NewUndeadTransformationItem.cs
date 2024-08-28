using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewUndeadTransformationItem", menuName = "Item/UndeadTransformation")]
public class NewUndeadTransformationItem : BaseScrollItem
{
    UndeadTransformationItem Undead;
    protected override void UseScroll(GameObject user)
    {
        Undead = user.GetComponentInChildren<UndeadTransformationItem>();
        if (Undead == null)
            return;

        Undead.UseItem();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tae;

[CreateAssetMenu(menuName = "Item/CleansePotion")]
public class CleansePotion : Tae.Inventory.BasePotion
{
    protected override void UsePotion(GameObject user)
    {
        if (!user.TryGetComponent(out StatusEffectManager player))
            return;

        player.DebuffRemoveEfffect();
    }
}

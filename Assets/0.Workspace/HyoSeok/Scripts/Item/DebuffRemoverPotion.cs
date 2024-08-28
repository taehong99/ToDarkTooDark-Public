using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuffRemoverPotion : Tae.Inventory.BasePotion
{
    protected override void UsePotion(GameObject user)
    {
        if (!user.TryGetComponent(out StatusEffectManager player))
            return;

        player.DebuffRemoveEfffect();
    }

}

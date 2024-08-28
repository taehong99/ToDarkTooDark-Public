using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelUpPotion", menuName = "Item/LevelUpPotion")]
public class LevelUpPotion : Tae.Inventory.BasePotion
{
    protected override void UsePotion(GameObject user)
    {
        if (!user.TryGetComponent(out PlayerStatsManager player))
            return;

        if (player.CurLevel < 10)
        {
            player.CurEXP = player.MaxEXP;
        }
    }

}

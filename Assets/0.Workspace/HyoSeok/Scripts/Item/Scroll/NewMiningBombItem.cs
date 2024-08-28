using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMiningBombItem", menuName = "Item/MiningBombItem")]

public class NewMiningBombItem : BaseScrollItem
{
    MiningBombItem bomb;
    protected override void UseScroll(GameObject user)
    {
        bomb = user.GetComponentInChildren<MiningBombItem>();
        if (bomb == null)
            return;
        bomb.UseItem();
    }
}

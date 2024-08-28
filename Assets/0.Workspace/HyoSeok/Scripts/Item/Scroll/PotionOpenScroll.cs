using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PotionOpenScroll", menuName = "Item/PotionOpenScroll")]
public class PotionOpenScroll : BaseScrollItem
{
    protected override void UseScroll(GameObject user)
    {
        PotionManager.Instance.AllSetOpen();
    }
}

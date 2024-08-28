using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMagicCompass", menuName = "Item/MagicCompass")]
public class NewMagicCompass : BaseScrollItem
{
    MagicCompass compass;
    protected override void UseScroll(GameObject user)
    {
        compass = user.GetComponentInChildren<MagicCompass>();
        if (compass == null)
            return;
        compass.UseItem();
    }

}

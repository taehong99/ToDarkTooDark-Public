using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tae.Inventory
{
    [CreateAssetMenu(menuName = "Item/ETCItem")]
    public class ETCItem : ConsumableItem
    {
        protected override void UseItem(GameObject user)
        {
            // 필요없음
        }
    }
}
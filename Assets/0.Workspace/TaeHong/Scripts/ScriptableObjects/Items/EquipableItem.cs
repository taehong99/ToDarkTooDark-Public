using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemLootSystem;

namespace Tae.Inventory
{
    [CreateAssetMenu(fileName = "NewEquipableItem", menuName = "Item/EquipableItem")]
    public class EquipableItem : Item
    {
        public EquipItemData equipItemData => itemData as EquipItemData;

        public override void Use(GameObject user)
        {
            Debug.Log($"Used {name}");
        }
    }
}

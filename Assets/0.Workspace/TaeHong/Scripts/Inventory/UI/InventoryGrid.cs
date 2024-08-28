using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ItemLootSystem;

namespace Tae.Inventory
{
    public class InventoryGrid : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] Transform equipmentSlotsParent;
        [SerializeField] Transform itemSlotsParent;
    
        [Header("Data Structures")]
        [SerializeField] private InventorySlot[] equipmentSlots;
        [SerializeField] private InventorySlot[] itemSlots;
        // Call at start to initialize empty inventory slots
        public void InitSlots(int ITEMSLOTS, int EQUIPMENTSLOTS)
        {
            itemSlots = itemSlotsParent.GetComponentsInChildren<InventorySlot>(true);
            equipmentSlots = equipmentSlotsParent.GetComponentsInChildren<InventorySlot>(true);
        }

        public void AddItem(int slotIdx, BaseItemData item, int count = 1)
        { 
            // 장비 아이템이면
            if (item is EquipItemData)
            {
                equipmentSlots[slotIdx].InsertItem(item);
            }
            // 사용형 아이템이면
            else
            {
                itemSlots[slotIdx].InsertItem(item, count);
            }
        }

        public void RemoveItem(int slotIdx)
        {
            itemSlots[slotIdx].ClearSlot();
        }

        public void RemoveEquipment(int slotIdx)
        {
            equipmentSlots[slotIdx].ClearSlot();
        }

        public void UpdateItemCount(int slotIdx, int count)
        {
            itemSlots[slotIdx].Item.UpdateCount(count);
        }
    }
}
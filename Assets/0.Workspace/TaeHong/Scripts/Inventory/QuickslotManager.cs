using ItemLootSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tae.Inventory
{
    public class QuickslotManager : MonoBehaviour
    {
        [SerializeField] QuickslotUI quickslotUI;

        private List<BaseItemData> slots = new();
        private Dictionary<BaseItemData, float> itemCooldowns = new();

        //private GameObject player => Manager.Game.MyPlayer;
        // 디버그용:
        [SerializeField] GameObject player;

        private void Awake()
        {
            //Manager.Game.MyPlayer = player; // TODO: REMOVE LATER

            for (int i = 0; i < 5; i++)
            {
                slots.Add(null);
            }
        }

        #region Input Keys
        private void OnUseSlot1()
        {
            if (slots[1] == null)
                return;

            InventoryManager.Instance.UseConsumable(slots[1]);
        }

        private void OnUseSlot2()
        {
            if (slots[2] == null)
                return;

            InventoryManager.Instance.UseConsumable(slots[2]);
        }

        private void OnUseSlot3()
        {
            if (slots[3] == null)
                return;

            InventoryManager.Instance.UseConsumable(slots[3]);
        }

        private void OnUseSlot4()
        {
            if (slots[4] == null)
                return;

            InventoryManager.Instance.UseConsumable(slots[4]);
        }
        #endregion

        public void RegisterItem(int slotNumber, BaseItemData item)
        {
            // 아이템이 이미 등록되어있을 경우
            if (slots.Contains(item))
            {
                if (slots[slotNumber] == item)
                    return;

                RemoveFromSlot(slots.IndexOf(item));
            }

            // 자리가 비어있을 경우
            if (slots[slotNumber] == null)
            {
                slots[slotNumber] = item;
                quickslotUI.InitSlot(item, slotNumber);
                return;
            }

            slots[slotNumber] = item;
            quickslotUI.UpdateSlot(item, slotNumber);
        }

        public void UpdateQuickslotCount(BaseItemData item, int newCount)
        {
            int slotNumber = slots.IndexOf(item);

            // Item not found
            if (slotNumber == -1) 
                return;

            // Item used up
            if (newCount == 0)
            {
                RemoveFromSlot(slotNumber);
                return;
            }

            quickslotUI.UpdateCount(slotNumber, newCount);
        }

        public void RemoveFromSlot(int slotNumber)
        {
            slots[slotNumber] = null;
            quickslotUI.ClearSlot(slotNumber);
        }
    }
}


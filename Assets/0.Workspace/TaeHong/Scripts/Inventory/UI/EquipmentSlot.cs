using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ItemLootSystem;
using UnityEngine.EventSystems;

namespace Tae.Inventory
{
    public class EquipmentSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image itemImage;

        private EquipItemData item;
        public EquipItemData Item => item;
        private BaseItemData itemData;
        public BaseItemData ItemData { get { return itemData; } set { itemData = value; UpdateSlot(); } }
        public bool isOccupied { get => itemData != null; }
        private bool isDoubleClick;
        private Coroutine doubleClickRoutine;
        private Coroutine showTooltipRoutine;

        private void OnDisable()
        {
            OnPointerExit(null);
        }

        public void AddOrSwapItem(EquipItemData item)
        {
            if(itemData == null)
                itemImage.gameObject.SetActive(true);

            this.item = item;
            ItemData = item; // Property automatically updates sprite
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // 좌클릭 연속으로하면 장비 장착 해제
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // 첫번째 클릭은 코루틴 실행
                if (!isDoubleClick)
                {
                    doubleClickRoutine = StartCoroutine(DoubleClickChecker());
                    return;
                }

                // 아이템이 없으면
                if (!isOccupied)
                    return;

                // 장비 장착 해제
                InventoryManager.Instance.UnequipItem(item);
                StopCoroutine(doubleClickRoutine);
                OnPointerExit(null);
                isDoubleClick = false;
            }
        }

        public void RemoveItem()
        {
            itemImage.gameObject.SetActive(false);
            item = null;
            itemData = null;
        }

        // Used in property setter
        private void UpdateSlot()
        {
            itemImage.sprite = itemData.icon;
        }

        private IEnumerator DoubleClickChecker()
        {
            isDoubleClick = true;

            float t = 0;
            while (t <= InventoryManager.DOUBLE_CLICK_TIME_WINDOW)
            {
                t += Time.deltaTime;
                yield return null;
            }

            isDoubleClick = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isOccupied)
                return;

            showTooltipRoutine = StartCoroutine(ShowTooltipRoutine());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(showTooltipRoutine != null)
                StopCoroutine(showTooltipRoutine);

            InventoryManager.Instance.equipmentTooltip.gameObject.SetActive(false);
        }

        private IEnumerator ShowTooltipRoutine()
        {
            yield return new WaitForSeconds(InventoryManager.TOOLTIP_DISPLAY_DELAY);

            InventoryManager.Instance.equipmentTooltip.Init(item);
            InventoryManager.Instance.equipmentTooltip.gameObject.SetActive(true);
        }
    }
}

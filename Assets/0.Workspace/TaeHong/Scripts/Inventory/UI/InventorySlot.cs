using ItemLootSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tae.Inventory
{
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] GameObject itemTemplate;
        [SerializeField] Image outline;

        public bool isEquipmentSlot;
        public bool isOccupied;
        public bool isOutInventory = false; // Inventory 이외에 슬롯 사용하는 경우 체크 (ex. 돌돌이라던가.. 돌돌이라던가...)

        public Transform itemTransform => transform.GetChild(0);
        public int Index => transform.GetSiblingIndex();

        private InventoryItem myItem;
        public InventoryItem Item => myItem;

        public UnityAction<BaseItemData> OnChangeItem;

        private void OnDisable()
        {
            InventoryManager.Instance.hoveredSlot = null;
            outline.gameObject.SetActive(false);
        }

        public void InsertItem(BaseItemData item, int count = 1)
        {
            InventoryItem itemUI = Instantiate(itemTemplate).GetComponent<InventoryItem>();

            // Store item instance
            myItem = itemUI;
            //Debug.Log(myItem.name);
            isOccupied = true;


            // Init ItemUI
            OnChangeItem?.Invoke(item);
            itemUI.Init(item, this, count);
        }

        public void ClearSlot()
        {
            if (transform.childCount > 0)
                Destroy(myItem.gameObject);

            myItem = null;
            isOccupied = false;
            OnChangeItem?.Invoke(null);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            InventoryManager.Instance.hoveredSlot = this;
            outline.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryManager.Instance.hoveredSlot = null;
            outline.gameObject.SetActive(false);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using ItemLootSystem;

namespace Tae.Inventory
{
    public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, UnityEngine.EventSystems.IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Components")]
        private RectTransform rectTransform;
        private Image image;
        [SerializeField] GameObject countUI;
        [SerializeField] TextMeshProUGUI countText;

        [Header("Data")]
        public BaseItemData item;

        private RectTransform gridTransform;
        private bool isDragging { get => InventoryManager.Instance.isDragging; set => InventoryManager.Instance.isDragging = value; }
        private Coroutine showTooltipRoutine;

        // Double Click
        private bool isDoubleClick;
        Coroutine doubleClickRoutine = null;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
        }

        private void Start()
        {
            rectTransform.offsetMin = Vector2.one * 5;
            rectTransform.offsetMax = Vector2.one * -5;
        }

        private void OnDisable()
        {
            OnPointerExit(null);
        }

        public void Init(BaseItemData item, InventorySlot slot, int count = 1)
        {
            RectTransform slotTransform = slot.GetComponent<RectTransform>();
            rectTransform.SetParent(slotTransform);
            rectTransform.position = slotTransform.position;
            this.item = item;

            if (item.isPotion)
            {
                UnknownPotionData unknownPotionData = PotionManager.Instance.PotionBottleData(item.ID);
                image.sprite = unknownPotionData.sprite;
            }
            else
            {
                image.sprite = item.icon;
            }

            SetCount(count);
        }

        public void SetCount(int count)
        {
            countText.text = $"x{count}";
            countUI.SetActive(true);
        }

        public void UpdateCount(int count)
        {
            countText.text = $"x{count}";
        }

        public void HideCount()
        {
            countUI.SetActive(false);
        }

        private InventorySlot GetCurSlot()
        {
            return transform.parent.GetComponent<InventorySlot>();
        }

        #region Pointer Events
        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            InventoryManager.Instance.originalSlot = GetCurSlot();
            InventoryManager.Instance.draggedItem = this;
            transform.SetParent(transform.root);
            image.raycastTarget = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging) // 마우스를 따라감
            {
                rectTransform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            image.raycastTarget = true;

            InventoryManager.Instance.DropItemInSlot(eventData);
            //Destroy(gameObject); // TODO: fix drop outside bounds
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // 기타 아이템은 클릭 기능 없음
            if (item.itemType == ItemType.ETC)
                return;

            // 우클릭하면 아이템 등록 UI가 뜸
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (item.itemType == ItemType.Consumable)
                {
                    InventoryManager.Instance.itemActionPopUpUI.transform.position = transform.position;
                    InventoryManager.Instance.itemActionPopUpUI.RightClickItem(item);
                }
                else if (item.itemType == ItemType.Upgrade)
                {
                    InventoryManager.Instance.upgradeItemPopUpUI.transform.position = transform.position;
                    InventoryManager.Instance.upgradeItemPopUpUI.RightClickItem(item);

                }
            }

            // 좌클릭 연속으로하면 아이템 사용
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // 첫번째 클릭은 코루틴 실행
                if (!isDoubleClick)
                {
                    doubleClickRoutine = StartCoroutine(DoubleClickChecker());
                    return;
                }

                // 더블클릭 
                if (item is EquipItemData)
                {
                    InventoryManager.Instance.EquipItem((EquipItemData) item);
                }
                // 사용 아이템일때
                else if (item.itemType == ItemType.Consumable)
                {
                    InventoryManager.Instance.UseConsumable(item);
                }
                else
                {
                    InventoryManager.Instance.upgradeItemPopUpUI.transform.position = transform.position;
                    InventoryManager.Instance.upgradeItemPopUpUI.RightClickItem(item);
                }
                
                StopCoroutine(doubleClickRoutine);
                isDoubleClick = false;
            }
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
            showTooltipRoutine = StartCoroutine(ShowTooltipRoutine());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (showTooltipRoutine == null)
                return;

            StopCoroutine(showTooltipRoutine);

            if (item.itemType == ItemType.Equipment)
            {
                InventoryManager.Instance.equipmentTooltip.gameObject.SetActive(false);
            }
            else
            {
                InventoryManager.Instance.itemTooltip.gameObject.SetActive(false);
            }
        }

        private IEnumerator ShowTooltipRoutine()
        {
            yield return new WaitForSeconds(InventoryManager.TOOLTIP_DISPLAY_DELAY);

            if (item is EquipItemData)
            {
                InventoryManager.Instance.equipmentTooltip.Init((EquipItemData) item);
                InventoryManager.Instance.equipmentTooltip.gameObject.SetActive(true);
            }
            else
            {
                InventoryManager.Instance.itemTooltip.Init(item);
                InventoryManager.Instance.itemTooltip.gameObject.SetActive(true);
            }

        }
        #endregion
    }
}


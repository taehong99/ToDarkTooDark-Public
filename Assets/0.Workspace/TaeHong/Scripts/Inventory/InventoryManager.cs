using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Tae;
using ItemLootSystem;
using Photon.Pun;
using Photon.Realtime;

namespace Tae.Inventory
{
    public class InventoryManager : MonoBehaviourPun
    {
        public static InventoryManager Instance { get; private set; }

        const int EQUIPMENTSLOTS = 4;
        const int ITEMSLOTS = 12;
        public const float DOUBLE_CLICK_TIME_WINDOW = 0.25f;
        public const float TOOLTIP_DISPLAY_DELAY = 0.5f;

        [Header("Components")]
        [SerializeField] List<BaseItemData> itemInventory;  // 실제로 아이템을 보관하는 인벤토리
        [SerializeField] List<EquipItemData> equipmentInventory; // 실제로 장비를 보관하는 인벤토리(임시로 풀어둠)
        private Dictionary<BaseItemData, int> itemCounts = new(); // 아이템 개수 보관소

        public List<BaseItemData> ItemInventory { get => itemInventory; } // 외부에서 아이템을 봐야되면 
        public List<EquipItemData> EquipmentInventory { get => equipmentInventory; } // 외부에서 장비를 봐야되면 
        public Dictionary<BaseItemData, int> ItemCounts { get => itemCounts; } // 외부에서 아이템 개수를 봐야되면

        [SerializeField] GameObject inventoryUI;
        [SerializeField] InventoryGrid inventoryGrid; // 인벤토리 안에 있는 아이템을 보여주는 UI
        [SerializeField] EquipmentUI equipmentUI; // 플레이어가 입고있는 장비를 보여주는 UI
        public QuickslotManager quickslotManager; // 퀵슬롯
        //public PotionManager potionManager;
        private PlayerStatsManager playerStatsManager => Manager.Game.MyPlayer.GetComponent<PlayerStatsManager>();
        private RectTransform inventoryGridRect;
        private RectTransform equipmentUIRect;

        [Header("Tooltips & PopUps")]
        public CurrencyUI currencyUI;
        public ItemActionPopUpUI itemActionPopUpUI; // 아이템 행동 UI
        public ThrowItemPopUpUI throwItemPopUpUI;
        public UpgradeItemPopUpUI upgradeItemPopUpUI;
        public ItemTooltip itemTooltip;
        public EquipmentTooltip equipmentTooltip;
        public SkillTooltip skillTooltip;

        [Header("Drag and Drop")]
        public bool isDragging;
        public InventoryItem draggedItem;
        public InventorySlot originalSlot;
        public InventorySlot hoveredSlot;
        private BaseItemData itemToThrow;

        [Header("Gold")]
        private int goldCount;
        public int GoldCount { get { return goldCount; } set { goldCount = value; goldCountChangedEvent?.Invoke(goldCount); } }
        public event Action<int> goldCountChangedEvent;

        public int[] keyCounts = new int[3];
        public int BronzeKeyCount { get { return keyCounts[0]; } set { keyCounts[0] = value; keyCountChangedEvent?.Invoke(); } }
        public int SilverKeyCount { get { return keyCounts[1]; } set { keyCounts[1] = value; keyCountChangedEvent?.Invoke(); } }
        public int GoldKeyCount { get { return keyCounts[2]; } set { keyCounts[2] = value; keyCountChangedEvent?.Invoke(); } }
        public event Action keyCountChangedEvent;

        public bool IsConsumableInventoryFull => GetFirstItemOpenSlot() == -1;
        public bool IsEquipmentInventoryFull => GetFirstEquipmentOpenSlot() == -1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            inventoryGrid.InitSlots(ITEMSLOTS, EQUIPMENTSLOTS);
            inventoryGridRect = inventoryGrid.GetComponent<RectTransform>();
            equipmentUIRect = equipmentUI.GetComponent<RectTransform>();
            currencyUI.Init();
            itemActionPopUpUI.Init();
            throwItemPopUpUI.Init();
            upgradeItemPopUpUI.Init();
            InitInventory();
        }

        // Test용 키 셋팅
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                ToggleInventoryUI();
            }
        }

        #region Utils
        public void ToggleInventoryUI()
        {
            bool isActive = inventoryUI.activeSelf;
            inventoryUI.SetActive(!isActive);
        }

        public void InitInventory()
        {
            itemInventory = new(ITEMSLOTS);
            equipmentInventory = new(EQUIPMENTSLOTS);
            for(int i = 0; i < ITEMSLOTS; i++)
            {
                itemInventory.Add(null);
            }
            for (int i = 0; i < EQUIPMENTSLOTS; i++)
            {
                equipmentInventory.Add(null);
            }
        }

        private int GetFirstItemOpenSlot()
        {
            // 첫번째 빈슬롯 찾기
            for (int i = 0; i < itemInventory.Count; i++)
            {
                if (itemInventory[i] == null)
                {
                    return i;
                }
            }

            // 여기까지 왔으면 빈슬롯 없음
            return -1;
        }

        private int GetFirstEquipmentOpenSlot()
        {
            // 첫번째 빈슬롯 찾기
            for (int i = 0; i < equipmentInventory.Count; i++)
            {
                if (equipmentInventory[i] == null)
                {
                    return i;
                }
            }

            // 여기까지 왔으면 빈슬롯 없음
            return -1;
        }

        public int GetItemCount(BaseItemData item)
        {
            if (item is EquipItemData)
                return 1;

            if (itemInventory.Contains(item))
                return itemCounts[item];

            return 0;
        }

        public bool CanPickUp(BaseItemData item)
        {
            if(item is EquipItemData)
            {
                bool canEquip = false;
                bool canAddToInventory = false;
                EquipItemData equipment = (EquipItemData) item;
                if (equipmentUI.CanEquip(equipment))
                    canEquip = true;
                if (GetFirstEquipmentOpenSlot() != -1)
                    canAddToInventory = true;
                return canEquip || canAddToInventory;
            }

            return GetFirstItemOpenSlot() != -1;
        }

        // 아이템 구매
        public void BuyItem(BaseItemData item)
        {
            ObtainItem(item);
            SpendGold(item.cost);
        }

        public void BuyItem(BaseItemData item, int cost)
        {
            ObtainItem(item);
            SpendGold(cost);
        }

        public void BuyItem(BaseItemData item, int cost, int num)
        {
            for(int i = 0; i < num; i++)
                ObtainItem(item);
            SpendGold(cost*num);
        }

        // 아이템 판매
        public void SellItem(BaseItemData data, int cost)
        {
            RemoveItemOrEquipment(data);
            ObtainGold(cost);
        }

        public void SellItem(BaseItemData data, int cost, int count)
        {
            if (data is EquipItemData)
            {
                RemoveEquipment((EquipItemData) data);
                return;
            }

            ReduceConsumableCount(data, count);
            ObtainGold(cost * count);
        }
        #endregion

        #region Gold
        public void ObtainGold(int amount)
        {
            GoldCount += amount;
        }

        public void ObtainGold(int amount, Player player)
        {
            photonView.RPC("ClientObtainGold", player, amount);
        }

        [PunRPC]
        public void ClientObtainGold(int amount)
        {
            GoldCount += amount;
        }

        public void SpendGold(int amount)
        {
            GoldCount -= amount;
        }
        #endregion

        #region Inventory
        // 땅에서 줍거나 상점에서 구매한 아이템을 가져오는 함수
        public void ObtainItem(BaseItemData item)
        {
            // 열쇠면
            if (item.itemType == ItemType.Key)
                ObtainKey(item);
            // 사용형 아이템이면
            else if (item is EquipItemData)
                ObtainEquipment((EquipItemData) item, true);
            // 장비 아이템이면
            else
                ObtainConsumableOrETC(item);
        }

        // 열쇠 얻기
        private void ObtainKey(BaseItemData key)
        {
            // 시간 없어서 하드코딩
            switch (key.ID)
            {
                case 997:
                    BronzeKeyCount++;
                    break;
                case 998:
                    SilverKeyCount++;
                    break;
                case 999:
                    GoldKeyCount++;
                    break;
                default:
                    Debug.LogWarning("INVALID KEY");
                    break;
            }
        }

        // 아이템 얻기
        private void ObtainConsumableOrETC(BaseItemData item, int amount = 1)
        {
            Debug.Log("obtained conusmable");
            // 아이템을 이미 가지고 있으면, 개수 늘려줌
            int itemIdx = itemInventory.IndexOf(item);
            if (itemIdx != -1)
            {
                BaseItemData curItem = itemInventory[itemIdx];
                itemCounts[curItem] += amount;
                inventoryGrid.UpdateItemCount(itemIdx, itemCounts[curItem]);
                quickslotManager.UpdateQuickslotCount(curItem, itemCounts[curItem]);
                return;
            }

            // 빈 자리 찾고 없으면 나가기
            int slotIdx = GetFirstItemOpenSlot();
            if (slotIdx == -1)
            {
                Debug.Log("Inventory Full");
                return;
            }

            // 가방에 아이템 추가하기
            inventoryGrid.AddItem(slotIdx, item, amount);
            itemCounts.Add(item, amount);
            itemInventory[slotIdx] = item;
        }

        // 아이템 쓰기
        public void UseConsumable(BaseItemData item)
        {
            if (item.isPotion)
            {
                PotionManager.Instance.RequestUsePotion(item.ID);
            }
            else
            {
                PotionManager.Instance.UseScrollItem(item.ID);
            }
            ReduceConsumableCount(item, 1);
        }

        public void ReduceConsumableCount(BaseItemData item, int count)
        {
            // 아이템 개수 줄이기
            int itemSlotIdx = itemInventory.IndexOf(item);
            itemCounts[item] -= count;
            inventoryGrid.UpdateItemCount(itemSlotIdx, itemCounts[item]);
            quickslotManager.UpdateQuickslotCount(item, itemCounts[item]);

            // 아이템 개수가 0이면 인벤토리에서 제거
            if (itemCounts[item] == 0)
            {
                RemoveItem(item);
            }
        }

        // 장비 얻기
        private void ObtainEquipment(EquipItemData item, bool isPickup)
        {
            // 자동장착 가능하면 장착하기
            if (equipmentUI.CanEquip(item) && isPickup)
            {
                Debug.Log("Can equip");
                EquipItem(item);
                return;
            }

            // 빈 자리 찾고 없으면 나가기
            int slotIdx = GetFirstEquipmentOpenSlot();
            Debug.Log(slotIdx);
            if (slotIdx == -1)
            {
                Debug.Log("Inventory Full");
                return;
            }

            // 가방에 장비 추가하기
            inventoryGrid.AddItem(slotIdx, item);
            equipmentInventory[slotIdx] = item;
        }

        // RemoveItem() + RemoveEquipment()
        public void RemoveItemOrEquipment(BaseItemData item)
        {
            if (item is EquipItemData)
                RemoveEquipment((EquipItemData) item);
            else
                RemoveItem(item);
        }

        // 아이템을 버리거나 팔때 인벤토리에서 제거하는 함수
        public void RemoveItem(BaseItemData item)
        {
            int index = itemInventory.IndexOf(item);
            inventoryGrid.RemoveItem(index);
            itemInventory[index] = null;
            itemCounts.Remove(item);
        }

        // 장비를 장착하거나, 버리거나, 팔때 인벤토리에서 제거하는 함수
        public void RemoveEquipment(EquipItemData item)
        {
            int index = equipmentInventory.IndexOf(item);
            inventoryGrid.RemoveEquipment(index);
            equipmentInventory[index] = null;
        }
        #endregion

        #region Throw Item
        private void RequestThrowItem() // 유저가 아이템을 UI 밖으로 드래그하면 
        {
            if (itemToThrow is EquipItemData) // 장비 아이템은 바로 버려짐
            {
                RemoveEquipment((EquipItemData)itemToThrow);
                SpawnNewItem(itemToThrow);
            }
            else
            {
                // 1개밖에 없으면 UI 안뜨고 바로 버리기
                int maxCount = GetItemCount(itemToThrow);
                if (maxCount <= 1) 
                {
                    RemoveItem(itemToThrow);
                    SpawnNewItem(itemToThrow);
                    return;
                }

                // 버리기 UI
                throwItemPopUpUI.PopUp(maxCount, HandleThrowItem);
            }
        }

        private void HandleThrowItem(int throwCount) // 아이템 버리기 UI가 뜨면
        {
            if (throwCount == 0)
                return;
                
            int count = GetItemCount(itemToThrow);
            // 다 버리면 인벤토리에서 제거
            if (throwCount == count)
            {
                RemoveItem(itemToThrow);
            }
            // 아니면 개수만 빼기
            else
            {
                ReduceConsumableCount(itemToThrow, throwCount);
            }

            // 버려진 아이템 생성
            SpawnNewItem(itemToThrow, throwCount);
        }

        private void SpawnNewItem(BaseItemData item, int throwCount = 1)
        {
            ItemFactory.Instance.DropItem(item, Manager.Game.MyPlayer.transform.position, throwCount);
        }
        #endregion

        #region Pointer Events
        public void DropItemInSlot(PointerEventData eventData)
        {
            // Item dropped outside grid => return item to original position
            if (IsOutSideDrop(eventData))
            {
                // 아이템 버리기
                itemToThrow = draggedItem.item;
                ReturnItemToOriginalSlot();
                RequestThrowItem();
            }
            // Item dropped inside grid
            else
            {
                // Return to original slot if dropped in invalid slot
                if (!CanBeDropped())
                {
                    ReturnItemToOriginalSlot();
                    return;
                }

                // If slot is empty, move item to slot
                if (!hoveredSlot.isOccupied && hoveredSlot != originalSlot)
                {
                    hoveredSlot.InsertItem(draggedItem.item, GetItemCount(draggedItem.item));
                    originalSlot.ClearSlot();
                    HandleItemSlotChange(draggedItem.item, originalSlot, hoveredSlot);
                    Destroy(draggedItem.gameObject);
                }

                // If slot is taken, swap item with original slot
                else
                {
                    originalSlot.InsertItem(hoveredSlot.Item.item, GetItemCount(draggedItem.item));
                    hoveredSlot.ClearSlot();
                    hoveredSlot.InsertItem(draggedItem.item, GetItemCount(draggedItem.item));
                    HandleItemSlotChange(draggedItem.item, originalSlot, hoveredSlot);
                    Destroy(draggedItem.gameObject);
                }
            }
        }

        private void HandleItemSlotChange(BaseItemData item, InventorySlot originSlot, InventorySlot targetSlot)
        {
            if (item is EquipItemData)
            {
                // 만약 대상 Slot이 돌돌이면 sawp 대신 기존 슬롯꺼는 삭제, slotb에는 데이터만 넘겨줘야함
                //Debug.Log($"Swap {slotA.Index} <=> {slotB.Index}");
                if (originalSlot.isOutInventory)
                {
                    equipmentInventory[targetSlot.Index] = item as EquipItemData;
                }
                else if (targetSlot.isOutInventory)
                    equipmentInventory[originalSlot.Index] = null; // RemoveAt 하면 리스트 공간 자체가 삭제되기 때문에 null 대입으로 초기화해줌
                else
                    equipmentInventory.SwapElements(originSlot.Index, targetSlot.Index);
            }
            else
            {
                itemInventory.SwapElements(originSlot.Index, targetSlot.Index);
            }
        }

        bool IsOutSideDrop(PointerEventData eventData)
        {
            //!RectTransformUtility.RectangleContainsScreenPoint(inventoryGridRect, eventData.position) && !RectTransformUtility.RectangleContainsScreenPoint(equipmentUIRect, eventData.position // <- 기존 조건
            if (!RectTransformUtility.RectangleContainsScreenPoint(equipmentUIRect, eventData.position)
                && hoveredSlot == null && !RectTransformUtility.RectangleContainsScreenPoint(inventoryGridRect, eventData.position))
                return true;
            else
                return false;
        }

        public bool CanBeDropped()
        {
            // Check if valid location
            if (hoveredSlot == null || hoveredSlot == originalSlot)
                return false;

            var inventoryItem = draggedItem.item;
            bool isEquipmentItem = inventoryItem is EquipItemData;
            bool isEquipmentSlot = hoveredSlot.isEquipmentSlot;

            // Ensure that equipment items go to equipment slots and other items go to non-equipment slots
            if (isEquipmentSlot != isEquipmentItem)
                return false;

            return true;
        }

        public void ReturnItemToOriginalSlot()
        {
            RectTransform itemTransform = draggedItem.GetComponent<RectTransform>();
            itemTransform.SetParent(originalSlot.transform);
            itemTransform.localPosition = Vector2.zero;
        }

        #endregion

        #region Equipment
        // 인벤토리창에서 장비를 더블클릭하면 장착하는 함수
        public void EquipItem(EquipItemData item)
        {
            // 무기가 내 직업꺼 아니면 장착 불가능
            if(item is WeaponItemData)
            {
                WeaponItemData weapon = (WeaponItemData) item;
                if (!weapon.CanUseWeapon())
                    return;
            }

            // 인벤토리창에 장비 제거
            if (equipmentInventory.Contains(item))
                RemoveEquipment(item);

            // 장비창에 장비 생성
            equipmentUI.Equip(item);

            // 장비 스탯 적용
            foreach (var entry in item.GetStat())
            {
                Debug.Log($"{entry.Key} {entry.Value}");
                playerStatsManager.UpdateStat(entry.Key, entry.Value);
            }
        }
        // 장비창에서 장비를 더블클릭하면 장착 해제하는 함수
        public void UnequipItem(EquipItemData item)
        {
            if (GetFirstEquipmentOpenSlot() == -1) // 인벤토리 꽉 찼음
                return;

            equipmentUI.UnEquip(item); // 장비창에서 장비 제거
            ObtainEquipment(item, false); // 인벤토리창에 장비 생성
            foreach (var entry in item.GetStat())
            {
                Debug.Log($"{entry.Key} {entry.Value}");
                playerStatsManager.UpdateStat(entry.Key, -entry.Value);
            }
        }
        #endregion
    }
}
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemLootSystem
{   /// <summary>
    /// 아이템을 드랍시키고 싶은 객체에 ItemDropTable 삽입한 다음에 DropedItems() 호출해서 반환 받으면 됨.
    /// 골드 드랍 & 플레이어 사용 무기별 필터링(EquipItemPool) 은 아직 미구현.
    /// </summary>
    // 드랍하는 아이템 목록이나, 갯수, 규칙 등은 DropTable에 미리 정의해 둘 것.
    

    [CreateAssetMenu(menuName = "ItemLoot/ItemDropTable")]
    public class ItemDropTable : ScriptableObject
    {
        public int id;
        public ItemRarity rarity; // 사실 필요 없음

        // Data Field (Immutable)
        [Space(10),SerializedDictionary("Item Type", "Weight")]
        public TypeWeightDictionary typeWeightsDic = new TypeWeightDictionary(){
            {ItemType.Equipment , 0 },
            {ItemType.Consumable , 0},
            {ItemType.ETC , 0},
            {ItemType.Key , 0},
        };
        // 열쇠를 제외한 나머지는 한번씩 드랍을 보장. 그 후 타입별 확률은 균등. 각자 가질수있는 범위만 다를뿐


        [SerializedDictionary("Item Type", "Maximum Drop Count")]
        public SerializedDictionary<ItemType, int> maximumDropCountDic = new SerializedDictionary<ItemType, int>();

        [SerializeField] public List<ItemPool> pools;

        [Header("Monster Version")]
        [SerializeField] List<EquipItemPool> equipmentPools;
        [SerializeField] List<ItemPool> itemPools;

        // Temporary Data Field (Deep Copy)
        List<ItemPool> dropablePools;

        Dictionary<ItemType, int> curWeight;
        Dictionary<ItemType, int> curDropCount;

        List<BaseItemData> pickedItems;

        protected void PickedItemsRandom(WeaponType type)
        {
            // pool 별로 최대/최소 아이템 갯수 제한 만족 시킬것.
            ItemPool pickedPool = PickedItemPool();

            if (pickedPool == null)
                return;

            ItemType pickedPoolType = pickedPool.poolType;

            if (curDropCount[pickedPoolType] <= 0)
            {
                dropablePools.Remove(pickedPool);
            }
            else
            {
                // pool에 weight 인수로 넘겨줘서 나올수 있는것만 random 돌리자,            
                BaseItemData pickedItem = pickedPool.PickItem(curWeight[pickedPoolType] , type);
                if (pickedItem == null)
                {
                    dropablePools.Remove(pickedPool);
                }
                else
                {
                    curWeight[pickedPoolType] -= pickedItem.weight;
                    curDropCount[pickedPoolType]--;
                    pickedItems.Add(pickedItem);
                    Debug.Log($"{pickedItem.Name}," +
                        $"\n Type: {pickedPoolType} \n Left Weight: {curWeight[pickedPoolType]}" +
                        $"\n Left Count: {curDropCount[pickedPoolType]}");
                }
            }
            PickedItemsRandom(type);
            return;
        }

        void PickedItemsEachPool(List<BaseItemData> pickedItems)
        {
            foreach (var pool in pools)
            {
                ItemType curPoolType = pool.poolType;
                if (curPoolType != ItemType.Key) // Key를 제외한 나머지 풀에서 아이템 하나씩 선택
                {
                    //아이템 픽
                    BaseItemData pickedItem = pool.PickItem(curWeight[curPoolType]);

                    if (pickedItem == null)
                    {
                        Debug.Log("PickedItem is Null");
                        continue;
                    }
                    //Pick해서 리스트에 Add
                    pickedItems.Add(pickedItem);
                    Debug.Log($"{pickedItem.Name}");

                    //각 타입별 weight, 카운트 감소
                    curWeight[curPoolType] -= pickedItem.weight;
                    curDropCount[curPoolType]--;
                }
            }
        }

        ItemPool PickedItemPool()
        {
            int temp = dropablePools.Count;
            if(temp == 0)
                return null;

            int randomValue = Random.Range(0, temp);

            return dropablePools[randomValue];
        }

        public List<BaseItemData> DropedItems(WeaponType type)
        {
            //Initialize Temporary Data
            pickedItems = new List<BaseItemData>();
            curWeight = new Dictionary<ItemType, int>(typeWeightsDic); // Deep Copy
            curDropCount = new Dictionary<ItemType, int>(maximumDropCountDic);
            dropablePools = new List<ItemPool>(pools);

            //Pick Item from Each Pool
            PickedItemsEachPool(pickedItems);

            //Add more Item by Random
            PickedItemsRandom(type);
            return pickedItems;
        }

         //해당 테이블의 등급에 맞는 골드 드랍
        public int DropGold()
        {
            switch (rarity)
            {
                case ItemRarity.Normal:
                    return Random.Range(10,21);
                case ItemRarity.Rare:
                    return Random.Range(20, 31);
                case ItemRarity.Epic:
                    return Random.Range(30, 51);
                case ItemRarity.Unique:
                    return Random.Range(80, 121);
                case ItemRarity.Legendary:
                    return 500;
            }
            return 0;
        }

        public EquipItemPool GetEquipmentPool(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.Phase1:
                    return equipmentPools[0];
                case GamePhase.Phase2:
                    return equipmentPools[1];
                case GamePhase.Phase3:
                    return equipmentPools[2];
                case GamePhase.Phase4:
                    return equipmentPools[3];
                case GamePhase.Phase5:
                    return equipmentPools[4];
                default:
                    return null;
            }
        }

        public ItemPool GetItemPool(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.Phase1:
                    return itemPools[0];
                case GamePhase.Phase2:
                    return itemPools[1];
                case GamePhase.Phase3:
                    return itemPools[2];
                case GamePhase.Phase4:
                    return itemPools[3];
                case GamePhase.Phase5:
                    return itemPools[4];
                default:
                    return null;
            }
        }
    }


    [System.Serializable]
    public class TierRateDictionary : SerializedDictionary<ItemRarity, float> { }
    [System.Serializable]
    public class TypeWeightDictionary : SerializedDictionary<ItemType, int> { }
}
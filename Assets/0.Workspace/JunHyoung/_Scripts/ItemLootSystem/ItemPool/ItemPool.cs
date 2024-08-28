using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItemLootSystem
{
    [CreateAssetMenu(menuName = "ItemLoot/ItemPool")]
    public class ItemPool : ScriptableObject
    {
        [SerializeField] public ItemType poolType;

        [SerializedDictionary("Item Tier", "Probabilities")]
        public TierRateDictionary tierProbabilities = new TierRateDictionary() {
            { ItemRarity.Normal, 0 },
            { ItemRarity.Rare, 0},
            { ItemRarity.Epic, 0 },
            { ItemRarity.Legendary, 0 },
            { ItemRarity.Unique, 0 },
            { ItemRarity.Artifact, 0 }};

        [Space(15)]
        [SerializeField] public List<BaseItemData> items = new List<BaseItemData>();


   
        [ContextMenu("> Pick Item")]
        public virtual BaseItemData PickItem(int weight , WeaponType type = WeaponType.NULL)
        {
            List<BaseItemData> dropableList = FilteredByWeight(items, weight);

            float totalWeight = dropableList.Sum(item => tierProbabilities[item.rarity]);
            /*float totalRate = 0;
            foreach(var item in items)
            {
                totalRate += tierProbabilities[item.rarity];
            }*/

            if (totalWeight <= 0)
                return null;

            float randomValue = Random.value * totalWeight;

            float currentWeight = 0f;

            foreach (var item in dropableList)
            {
                float itemWeight = tierProbabilities[item.rarity];
                if (currentWeight + itemWeight >= randomValue)
                {
                    var instance = Instantiate(item);
                    return instance;
                }
                currentWeight += itemWeight;
            }
            return null;
        }

        public ItemRarity GetRandomRarity()
        {
            float random = Random.Range(0, 100f);
            float curTotal = 0;
            foreach(var entry in tierProbabilities)
            {
                curTotal += entry.Value;
                if (random <= curTotal)
                    return entry.Key;
            }
            return ItemRarity.Normal;
        }

        /// <summary>
        /// 인수로 받은 weight 보다 큰 아이템들을 제외한 리스트를 반환
        /// </summary>
        /// <param name="items"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        protected static List<BaseItemData> FilteredByWeight(List<BaseItemData> items , int weight)
        {
            return items.Where(item => item.weight <= weight).ToList(); //where , select , findall
        }


        protected virtual void OnValidate()
        {
            ValidateItems();
        }

        void ValidateItems()
        {
            if(items == null) 
                return;

            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i] == null)
                    return;

                if (items[i].itemType != poolType)
                {
                    Debug.LogWarning("Item Type 이 현재 Pool Type과 일치하지 않습니다!");
                    items.RemoveAt(i);
                }
            }
        }
    }

}

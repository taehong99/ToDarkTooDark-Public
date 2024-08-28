using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun.UtilityScripts;
using Tae.Inventory;

namespace ItemLootSystem
{
    [CreateAssetMenu(menuName = "ItemLoot/ObjDropTable")]
    public class ObjectDropTable : ScriptableObject
    {

        [SerializeField] Vector2Int goldDropRange;
        [SerializeField] float goldDropRate;
        [Space(10),SerializeField] List<DropItem> dropedItems = new List<DropItem>();


        [Serializable]
        public class DropItem
        {
            public BaseItemData itemData;
            public Vector2Int dropCountRange;
            public float dropRate;
        }


        public List<BaseItemData> PickedItem()
        {
            List<BaseItemData > items = new List<BaseItemData>();

            float rand = UnityEngine.Random.value * 100;
            if(rand < goldDropRate)
            {
                GoldDrop();
                return null;
            }

            float totalDropRate = 0;
            foreach (var dropItem in dropedItems)
            {
                totalDropRate += dropItem.dropRate;
            }

            float normalizedRand = UnityEngine.Random.value * totalDropRate;

            float cumulativeDropRate = 0;
            foreach (var dropItem in dropedItems)
            {
                cumulativeDropRate += dropItem.dropRate;
                if (normalizedRand <= cumulativeDropRate)
                {
                    int dropCount = UnityEngine.Random.Range(dropItem.dropCountRange.x, dropItem.dropCountRange.y);
                    for(int i = 0; i < dropCount; i++)
                    {
                        items.Add(Instantiate(dropItem.itemData));
                    }
                    return items;
                }
            }
            return null;
        }

        public void GoldDrop()
        {
            int gold = UnityEngine.Random.Range(goldDropRange.x, goldDropRange.y + 1);
            InventoryManager.Instance?.ObtainGold(gold);
        }
    }



}
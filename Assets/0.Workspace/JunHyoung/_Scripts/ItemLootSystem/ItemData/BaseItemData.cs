using System;
using System.Collections.Generic;
using UnityEngine;
using Tae.Inventory;

namespace ItemLootSystem
{
    [CreateAssetMenu(menuName = "ItemData/ItemData")]
    public class BaseItemData : ScriptableObject
    {
        public Sprite icon;
        public GameObject prefab;

        public int ID;
        public string Name;
        public ItemRarity rarity;
        public ItemType itemType;
        public bool isPotion;

 

        public int weight; //절대적인 가치
        public int probability; // 상대적인 확률

        public Item item; // 아이템 기능을 가진 ScriptableObject
		public int maxDrops;
		public string etcItemType;

        [Multiline]
        public string toolTip;

        public int cost; //가격
        /******************************************************
        *                Serialize / Deserialize
        ******************************************************/
        #region Serilaze/Deserialize
        public static byte[] Serialize(object obj)
        {
            return Serialize((BaseItemData) obj);
        }
        private static byte[] Serialize(BaseItemData data)
        {
            List<byte> result = new List<byte>();

            // id 값을 바이트 배열로 변환하여 추가
            result.AddRange(BitConverter.GetBytes(data.ID));

            return result.ToArray();
        }

        public static BaseItemData Deserialize(byte[] data)
        {
            int offset = 0;

            // id 값을 읽기
            int id = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);

            BaseItemData itemData = CreateInstance<BaseItemData>();
            itemData.ID = id;
            return itemData;
        }
        #endregion
    }



}
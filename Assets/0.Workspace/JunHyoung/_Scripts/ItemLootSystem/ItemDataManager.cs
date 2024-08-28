using System.Collections.Generic;
using UnityEngine;

namespace ItemLootSystem
{
    public class ItemDataManager : Singleton<ItemDataManager>
    {
        // 모든 아이템을 가지는 Dictionary
        [SerializeField] public ItemDictionary DB;

        [SerializeField] public EquipAffixesTable EquipAffixesTable;


        /// <summary>
        /// SerializedData로 ItemData 획득, Equip이나 Weapon인지도 자동으로 찾아서 반환. 쓸 때 캐스팅만 해주면 됨.
        /// </summary>
        /// <param name="serializedData"></param>
        /// <returns></returns>
        public BaseItemData GetItemData(byte[] serializedData)
        {
            BaseItemData data = BaseItemData.Deserialize(serializedData);

            //DB에서 일단 ID로만 조회
            BaseItemData dbData = DB.itemDB[data.ID];

            if (dbData is EquipItemData)
            {
                if ((dbData as EquipItemData).equipmentType == EquipmentType.Weapon)
                {
                    return GetWeaponData(serializedData);
                }
                else
                {
                    return GetEquipData(serializedData);
                }
            }
            //return Instantiate(dbData);
            return dbData;
        }


        /// <summary>
        /// Method for Casting Data
        /// </summary>
        /// <param name="serializedData"></param>
        /// <returns></returns>
        private EquipItemData GetEquipData(byte[] serializedData)
        {
            EquipItemData data = EquipItemData.Deserialize(serializedData);
            EquipItemData itemData = Instantiate(DB.itemDB[data.ID] as EquipItemData);

            itemData.enhanceLevel = data.enhanceLevel;
            itemData.affixes = new List<EquipAffix>();

            for (int i = 0; i < data.affixes.Count; i++)
            {
                EquipAffix affix = Instantiate(EquipAffixesTable.affixes[data.affixes[i].ID]);
                affix.isPositive = data.affixes[i].isPositive;
                itemData.affixes.Add(affix);
            }

            return itemData;
        }

        /// <summary>
        /// Method for Casting Data
        /// </summary>
        /// <param name="serializedData"></param>
        /// <returns></returns>
        private WeaponItemData GetWeaponData(byte[] serializedData)
        {
            EquipItemData equipData = EquipItemData.Deserialize(serializedData);
            WeaponItemData itemData = Instantiate(DB.itemDB[equipData.ID] as WeaponItemData);

            itemData.enhanceLevel = equipData.enhanceLevel;
            itemData.affixes = new List<EquipAffix>();

            for (int i = 0; i < equipData.affixes.Count; i++)
            {
                EquipAffix affix = Instantiate(EquipAffixesTable.affixes[equipData.affixes[i].ID]);
                affix.isPositive = equipData.affixes[i].isPositive;
                itemData.affixes.Add(affix);
            }

            return itemData;
        }


        /// <summary>
        /// Photon stream에 전송하기 위해 object[] 배열로 반환. itemType 참조해서 자동으로 직렬화 해줌
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        public object[] ConvertToObjectArray(BaseItemData itemData, int count = 1)
        {
            byte[] serializedData;

            if (itemData.itemType == ItemType.Equipment)
            {
                serializedData = EquipItemData.Serialize(itemData);
            }
            else
            {
                serializedData = BaseItemData.Serialize(itemData);
            }

            object[] objectData = new object[2];
            objectData[0] = serializedData;
            objectData[1] = count;

            return objectData;
        }

    }
}

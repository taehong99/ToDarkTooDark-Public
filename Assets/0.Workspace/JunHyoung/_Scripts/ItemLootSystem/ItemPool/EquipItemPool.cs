using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ItemLootSystem
{
    [CreateAssetMenu(menuName = "ItemLoot/EquipItemPool")]
    public class EquipItemPool : ItemPool
    {
        [Space(15), SerializedDictionary("EquipmentType", "Probabilities")]
        public SerializedDictionary<EquipmentType, float> equipTypeProbabilities = new SerializedDictionary<EquipmentType, float>();

        [Space(10)]
        [SerializeField] EquipAffixesTable affixesTable;
        [SerializeField] ItemEnhanceProbilitiy enhanceProbilitiy;

        //각 부위별 List;
        List<BaseItemData> weapons;
        List<BaseItemData> armors;
        List<BaseItemData> accessorys;

        public override BaseItemData PickItem(int weight, WeaponType type )
        {
            FilteredByType();
                                                                     // 24.08.09 17:00 PJH
            List<BaseItemData> dropableList = SelectEquipType(type); // Update : 무기 타입별 필터링 기능. 
            //dropableList = FilteredByWeight(dropableList, weight);// Legacy : 드랍 방식이 가치 기반에서 축소되어 
                                                                    // 최대 갯수만 가지고 드랍시켜주기 때문에, 굳이 해당부분은 호출하지 말자.
            float totalWeight = dropableList.Sum(item => tierProbabilities[item.rarity]);

            if (totalWeight <= 0)
                return null;

            float randomValue = Random.value * totalWeight;

            float currentWeight = 0f;

            foreach (var item in dropableList)
            {
                float itemWeight = tierProbabilities[item.rarity];
                if (currentWeight + itemWeight >= randomValue)
                {
                    EquipItemData instace = Instantiate(item) as EquipItemData;
                    // 반환전 어픽스 붙여 추가
                    affixesTable.AddAffixes(instace); // + 어픽스 붙은거에 따라 아이템 가중치 증감해줘야함

                    // 강화 수치 추가
                    enhanceProbilitiy.EnhanceItemRandom(instace);
                    return instace;
                }
                currentWeight += itemWeight;
            }
            return null;
        }

        //부위별로 List 생성
        void FilteredByType()
        {
            // Init List
            weapons = new List<BaseItemData>();
            armors = new List<BaseItemData>();
            accessorys = new List<BaseItemData>();

            foreach (var item in items)
            {
                EquipItemData equipItemData = (EquipItemData) item;
                switch (equipItemData.equipmentType)
                {
                    case EquipmentType.Weapon:
                        weapons.Add(item);
                        break;
                    case EquipmentType.Armor:
                        armors.Add(item);
                        break;
                    case EquipmentType.Accessory:
                        accessorys.Add(item);
                        break;
                }
            }
        }

        /// <summary>
        /// equipTypeProbabilities 에 맞춰 부위 지정, 해당 List 반환
        /// </summary>
        /// <returns></returns>
        List<BaseItemData> SelectEquipType(WeaponType type)
        {
            float totalProabilities = 0;
            foreach (var proabilities in equipTypeProbabilities.Values)
            {
                totalProabilities += proabilities;
            }

            float randomValue = Random.value * totalProabilities;
            float currentValue = 0;

            foreach (var kvp in equipTypeProbabilities)
            {
                currentValue += kvp.Value;
                if (randomValue <= currentValue)
                {
                    switch (kvp.Key)
                    {
                        case EquipmentType.Weapon:
                            return FilteredWeaponList(type);
                        case EquipmentType.Armor:
                            return armors;
                        case EquipmentType.Accessory:
                            return accessorys;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 사용자의 장비 Type에 따라 무기 리스트를 필터링
        /// </summary>
        List<BaseItemData> FilteredWeaponList(WeaponType targetType)
        {
            if (targetType == WeaponType.NULL)
                return weapons;

            List<BaseItemData> filteredList = new List<BaseItemData>();

            for(int i = 0; i < weapons.Count; i++)
            {
                WeaponItemData weaponItemData = weapons[i] as WeaponItemData;
                if(weaponItemData.weaponType == targetType)
                {
                    filteredList.Add(weaponItemData);
                }
            }
            return filteredList;
        }


        protected override void OnValidate()
        {
            poolType = ItemType.Equipment;
            base.OnValidate();
        }
    }
}
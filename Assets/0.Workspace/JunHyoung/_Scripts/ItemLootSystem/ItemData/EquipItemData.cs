using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ItemLootSystem
{
    [CreateAssetMenu(menuName = "ItemData/EquipData")]
    public class EquipItemData : BaseItemData
    {


        public EquipmentType equipmentType;
        public EquipmentGroup equipmentGroup;

        public StatType mainStat;
        public float mainStatAmount;

        public StatType subStat;
        public float subStatAmount;

        public int enhanceLevel; // 현재 강화 수치


        public List<EquipAffix> affixes = new List<EquipAffix>(); // 장비 수식어, 0~2개 // type, grade, value 

        /// <summary>
        /// 수식어 강화수치 등 계산해서 총합 스탯 반환
        /// </summary>
        /// <returns></returns>
        public Dictionary<StatType, float> GetStat()
        {
            Dictionary<StatType, float> itemStatDic = new Dictionary<StatType, float>();

            //+ MainStat
            float mainStatValue = EnhancedStat(enhanceLevel);
            itemStatDic.Add(mainStat, mainStatValue);

            //+ SubStat
            if(subStat != 0)
                itemStatDic.Add(subStat, subStatAmount);

            // Affix
            foreach (var affix in affixes)
            {
                if (affix.isPositive)
                {
                    if (itemStatDic.ContainsKey(affix.statType))
                    {
                        itemStatDic[affix.statType] += affix.value;
                    }
                    else
                    {
                        itemStatDic.Add(affix.statType, affix.value);
                    }
                }
                else
                {
                    if (itemStatDic.ContainsKey(affix.statType))
                    {
                        itemStatDic[affix.statType] -= affix.value;
                    }
                    else
                    {
                        itemStatDic.Add(affix.statType, -affix.value);
                    }
                }
            }
            return itemStatDic;
        }


        // 메인 스탯 강화 계산 
        public float EnhancedStat(int enhanceLevel)
        {
            if (enhanceLevel == 0)
                return mainStatAmount;

            if (EnhanceMultiPlier.enhanceMultipliers.TryGetValue((mainStat, equipmentType), out float multiplier))
            {
                return mainStatAmount + multiplier * enhanceLevel;
            }

            // Default case if no specific multiplier is found
            return mainStatAmount;
        }

        /// <summary>
        /// 수식어를 포함한 아이템 이름을 한줄로 반환하는 메서드, 수식어의 경우 한칸씩 구분함
        /// </summary>
        /// <returns></returns>
        public string GetItemNameWithSpaces()
        {
            StringBuilder sb = new StringBuilder();

            if (affixes == null || affixes.Count == 0)
                return Name;

            for (int i = 0; i < affixes.Count - 1; i++)
            {
                sb.Append(affixes[i].GetTextTag());
                sb.Append(affixes[i].GetFirstName());
                sb.Append("</color> ");
            }
            sb.Append(affixes[affixes.Count - 1].GetTextTag());
            sb.Append(affixes[affixes.Count - 1].GetSingleName());
            sb.Append("</color> ");
            sb.Append(Name);
            return sb.ToString().Trim();
        }

        /// <summary>
        /// 수식어를 포함한 아이템 이름을 반환하는 메서드, 수식어는 한줄씩 띄워서 반환함
        /// </summary>
        /// <returns></returns>
        public string GetItemNameWithNewLines()
        {
            StringBuilder sb = new StringBuilder();

            if (affixes == null || affixes.Count == 0)
                return Name;

            for (int i = 0; i < affixes.Count - 1; i++)
            {
                sb.Append("<size=70%>");
                sb.Append(affixes[i].GetTextTag());
                sb.AppendLine(affixes[i].GetFirstName());
                sb.Append("</color> ");
            }
            sb.Append(affixes[affixes.Count - 1].GetTextTag());
            sb.Append(affixes[affixes.Count - 1].GetSingleName());
            sb.AppendLine("</color></size>");
            sb.Append(Name);
            return sb.ToString();
        }




        /******************************************************
        *                Serialize / Deserialize
        ******************************************************/
        #region Serilaze/Deserialize
        public static new byte[] Serialize(object obj)
        {
            return Serialize((EquipItemData) obj);
        }


        // 직렬화 데이터 -> ID + 강화LV + 수식어갯수 + (수식어ID + 수식어 긍부정) 
        private static byte[] Serialize(EquipItemData itemData)
        {
            List<byte> result = new List<byte>();

            // id 값(int)
            result.AddRange(BitConverter.GetBytes(itemData.ID));

            // enhanceLevel(int)
            result.AddRange(BitConverter.GetBytes(itemData.enhanceLevel));

            // affixes 리스트 개수
            result.AddRange(BitConverter.GetBytes(itemData.affixes.Count));

            // 각 affix ID (int)
            foreach (var affix in itemData.affixes)
            {
                result.AddRange(BitConverter.GetBytes(affix.ID));
                result.Add(Convert.ToByte(affix.isPositive));
            }

            return result.ToArray();
        }

        public static new EquipItemData Deserialize(byte[] data)
        {
            int offset = 0;

            // id 값 읽기
            int id = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);

            // enhanceLevel 읽기
            int enhanceLevel = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);

            // affixes 개수 읽기
            int affixCount = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);
            // 각 affix ID 읽기
            List<EquipAffix> affixes = new List<EquipAffix>();
            for (int i = 0; i < affixCount; i++)
            {
                int affixID = BitConverter.ToInt32(data, offset);
                offset += sizeof(int);
                bool isPositive = BitConverter.ToBoolean(data, offset);
                offset++;

                EquipAffix affix = CreateInstance<EquipAffix>();
                affix.ID = affixID;
                affix.isPositive = isPositive;
                affixes.Add(affix);
            }

            // EquipItemData 객체 생성 및 반환
            EquipItemData itemData = CreateInstance<EquipItemData>();

            itemData.ID = id;
            itemData.enhanceLevel = enhanceLevel;
            itemData.affixes = affixes;

            return itemData;
        }
        #endregion


    }

    // 스탯&타입 별 강화 수치 설정 https://docs.google.com/spreadsheets/d/14d2qYy8WmOYkSLwdZj3gGJmSQRUYsomi/edit?gid=2119960834#gid=2119960834
    public static class EnhanceMultiPlier
    {
        public static readonly Dictionary<(StatType, EquipmentType), float> enhanceMultipliers = new Dictionary<(StatType, EquipmentType), float>
        {
        {(StatType.Power, EquipmentType.Weapon), 1.0f},
        {(StatType.Power, EquipmentType.Accessory), 0.5f},
        {(StatType.Armor, EquipmentType.Armor), 0.5f},
        {(StatType.Armor, EquipmentType.Accessory), 0.25f},
        {(StatType.MaxHp, EquipmentType.Accessory), 10.0f},
        {(StatType.HpRegen, EquipmentType.Accessory), 0.5f},
        {(StatType.Speed, EquipmentType.Accessory), 0.05f},
        {(StatType.Critical, EquipmentType.Accessory), 1.0f},
        {(StatType.CriticalMultiplier, EquipmentType.Accessory), 4.0f}
        };
    }

    // 레어도별 최대 강화 수치
    // https://docs.google.com/spreadsheets/d/14d2qYy8WmOYkSLwdZj3gGJmSQRUYsomi/edit?gid=1047061629#gid=1047061629
    public static class MaxEnahnceLevel
    {
        public static readonly Dictionary<ItemRarity, int> pairs = new Dictionary<ItemRarity, int>
        {   {ItemRarity.Normal, 5 },
            {ItemRarity.Rare, 10 },
            {ItemRarity.Epic, 10 },
            {ItemRarity.Unique, 15 },
            {ItemRarity.Legendary, 15 },
            {ItemRarity.Artifact, 20 },
        };
    }
}
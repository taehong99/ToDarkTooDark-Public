using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemLootSystem
{
    [CreateAssetMenu(menuName = "ItemLoot/Enhance/EnhanceProbilitiy")]
    public class ItemEnhanceProbilitiy : ScriptableObject
    {
        // 생성시 레어도별 최소, 최대 강화 수치 (강화 수치별 확률이 추가되면 사라져도 될 듯)
        Dictionary<ItemRarity, (int, int)> MaxMinEnhanceLevel = ItemEnhanceProbilitiyTemp.MaxMinEnhanceLevel;//= new Dictionary<ItemRarity, (int, int)>();


        // 레어도별 강화 확률
        [SerializedDictionary("Rarity", "Rate")]
        SerializedDictionary<ItemRarity, float> EnhanceProbilitiy = ItemEnhanceProbilitiyTemp.EnhanceProbilitiy;

        // 강화 수치별 확률 (+ 추후 추가될것)


        //(임시) 
        public void EnhanceItemRandom(EquipItemData equip)
        {
            ItemRarity rarity = equip.rarity;

            //레어도별 강화 여부 계산
           // float rand = Random.value;
          //  if (rand > EnhanceProbilitiy[rarity])
           //     return;

            //레어도별 강화 수치 계산
            float rand = Random.Range(MaxMinEnhanceLevel[rarity].Item1, MaxMinEnhanceLevel[rarity].Item2 + 1);
            
            equip.enhanceLevel = (int)rand;
        }
    }

    public static class ItemEnhanceProbilitiyTemp
    {
        public static readonly Dictionary<ItemRarity, (int, int)> MaxMinEnhanceLevel = new Dictionary<ItemRarity, (int, int)>
        {   {ItemRarity.Normal,(-4,4) },
            {ItemRarity.Rare,(-2,6) },
            {ItemRarity.Epic,(-3,10)},
            {ItemRarity.Unique,(1,15) },
            {ItemRarity.Legendary,(5,15) },
            {ItemRarity.Artifact,(4,20) },
        };

        public static SerializedDictionary<ItemRarity, float> EnhanceProbilitiy = new SerializedDictionary<ItemRarity, float>
        { { ItemRarity.Normal, 0.1f },
          { ItemRarity.Rare, 0.3f },
          { ItemRarity.Epic, 0.7f },
          { ItemRarity.Unique, 0.8f },
          { ItemRarity.Legendary, 1f },
          { ItemRarity.Artifact, 1f }
        };
    }
}

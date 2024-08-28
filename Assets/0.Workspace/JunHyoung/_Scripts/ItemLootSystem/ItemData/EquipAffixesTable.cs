using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper;
using AYellowpaper.SerializedCollections;

namespace ItemLootSystem
{
    [CreateAssetMenu(menuName = "ItemLoot/Affix/AffixesTable")]
    public class EquipAffixesTable : ScriptableObject
    {
        //레어도별 첫번째, 두번째 어픽스 붙는 확률 설정
        [Space(10), SerializedDictionary("Rarity", "Rate")]
        public SerializedDictionary<ItemRarity, float> firstAffixRate = new SerializedDictionary<ItemRarity, float>();

        [Space(10),SerializedDictionary("Rarity", "Rate")]
        public SerializedDictionary<ItemRarity, float> secondAffixRate = new SerializedDictionary<ItemRarity, float>();


        //레어도별 수식어 부정, 긍정 확률
        [Space(10),SerializedDictionary("Rarity", "Positve Rate")]
        public SerializedDictionary<ItemRarity, float> affixPositiveRate = new SerializedDictionary<ItemRarity, float>();

        // 모든 수식어 리스트
        [Space(15),SerializeField] public List<EquipAffix> affixes = new List<EquipAffix>();

        /// <summary>
        /// 장비 생성시
        /// </summary>
        /// <param name="equip"></param>
        public void AddAffixes(EquipItemData equip)
        {
            equip.affixes = new List<EquipAffix>();
            float randomValue = Random.value * 100;

           // for (int i = 0; i < 10; i++)
           //   AddAffix(equip);

            if(randomValue < firstAffixRate[equip.rarity])
            {
                AddAffix(equip);

                randomValue = Random.value * 100;

                if(randomValue < secondAffixRate[equip.rarity])
                {
                    AddAffix(equip);
                }

                //중복검사 다음에 Instantiate한 객체로 교체해줌... 먼저하면 Exists로 중복객체 검사가 안돔
                //  - 아니면 중복검사 방식을 ID랑 긍부정 두개 가지고 비교하는걸로 바꾸던가
                List<EquipAffix> affixInstance = new List<EquipAffix>();
                foreach(EquipAffix affix in equip.affixes)
                {
                   affixInstance.Add(Instantiate(affix));
                }
                equip.affixes = affixInstance;
            }
        }

        void AddAffix(EquipItemData equip)
        {
            EquipAffix selectedAffix = null;
            bool isDuplicate = true;

            // 중복 방지, 중복이 아닐때까지 돌림
            while (isDuplicate)
            {
                selectedAffix = SelectAffixByWeight();
                isDuplicate = equip.affixes.Exists(affix => affix == selectedAffix);
            }

            // 긍정부정 선택 , 체력리젠의 경우 부정 어픽스가 없으므로 skip
            if (selectedAffix != null && selectedAffix.statType != StatType.HpRegen)
            {
                float randomValue = Random.value * 100;
                selectedAffix.isPositive = randomValue < affixPositiveRate[equip.rarity];

                equip.affixes.Add(selectedAffix);
                //Debug.Log($"Added affix: {selectedAffix.positiveName} (Positive: {selectedAffix.isPositive})");
            }
        }

        /// <summary>
        /// 돌돌이로 수식어 붙일 때는 이거 호출
        /// </summary>
        /// <param name="equip">붙일 장비 아이템</param>
        /// <param name="mode">true면 프리미엄 돌돌이 모드</param>
        public void AddAffixesByDolDol(EquipItemData equip, bool mode = false)
        {
            equip.affixes = new List<EquipAffix>();

            //일반 모드면 0~2개

            //긍정 모드면 1~2개
            int affixCnt = mode ? Random.Range(1, 3) : Random.Range(0, 3);

            for (int i=0; i<affixCnt; i++)
            {
                AddAffixByDolDol(equip, mode);
            }
        }

        // 수식어 결졍
        private void AddAffixByDolDol(EquipItemData equip, bool mode)
        {
            EquipAffix affix = SelectAffixByWeight();
            affix = Instantiate(affix);

            if (affix.statType == StatType.HpRegen || mode)
            {
                affix.isPositive = true;
            }
            else
            {
                float randomValue = Random.value;
                if(randomValue < 0.5)
                {
                    affix.isPositive = false;
                }
                else
                {
                    affix.isPositive = true;
                }
            }
            //Debug.Log($"Affix Table Select {affix.statType},{affix.isPositive} ,{affix.GetSingleName()}");
            equip.affixes.Add(affix);
        }

        private EquipAffix SelectAffixByWeight()
        {
            float totalWeight = 0;
            foreach (var affix in affixes)
            {
                totalWeight += affix.weight;
            }

            float randomValue = Random.value * totalWeight;
            float curWeight = 0;

            foreach (var affix in affixes)
            {
                curWeight += affix.weight;
                if (randomValue <= curWeight)
                {
                    //return Instantiate(affix); //Instantiate 하면 중복검사 안됨. 객체 검사라서
                    return affix;
                }
            }
            Debug.Log("Affix Table Retrurn NULL");
            return null;
        }
    }
}
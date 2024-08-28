using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemLootSystem {
    [CreateAssetMenu(menuName = "ItemLoot/Affix/AffixData")]
    public class EquipAffix : ScriptableObject
    {
        public int ID;

        [Header("분류")]
        public StatType statType; // 올려주는 수식어 타입
        public int grade; // 1단계 ~ 4단계 //없어도 될랑가?a
        public bool isPositive = true;

        [Space(10), Header("이름")]
        public string positiveName;
        public string firstPositiveName;
        public string negativeName;
        public string firstNegativeName;

        public float value; // 수치

        public float weight; // 확률 


        public EquipAffix(int ID)
        {
            this.ID = ID;
        }

        public string GetSingleName()
        {
            return isPositive ? positiveName : negativeName;
        }

        public string GetFirstName()
        {
            return isPositive ? firstPositiveName : firstNegativeName;
        }

        // 데이터 테이블서 표시한 컬러 기준으로 반환 : https://docs.google.com/spreadsheets/d/1ySK59e681jnpoGLxo4jVPZaRP-q_P__0MjZqIJNzH1I/edit?gid=608786785#gid=608786785
        public Color GetTextColor()
        {
            Color color = Color.white;
            switch (statType)
            {
                case StatType.Power:
                     ColorUtility.TryParseHtmlString("#ea9999",out color);
                    break;
                case StatType.Armor:
                    ColorUtility.TryParseHtmlString("#f6b26b", out color);
                    break;
                case StatType.MaxHp:
                    ColorUtility.TryParseHtmlString("#b6d7a8", out color);
                    break;
                case StatType.HpRegen:
                    ColorUtility.TryParseHtmlString("#c27ba0", out color);
                    break;
                case StatType.Speed:
                    ColorUtility.TryParseHtmlString("#6fa8dc", out color);
                    break;
                case StatType.Critical:
                    ColorUtility.TryParseHtmlString("#4285f4", out color);
                    break;
                case StatType.CriticalMultiplier:
                    ColorUtility.TryParseHtmlString("#76a5af", out color);
                    break;
            }
            return color;
        }
        
        // for using at TMP Pro
        public string GetTextTag()
        {
            string tag = string.Empty;
            switch (statType)
            {
                case StatType.Power:
                    tag = "<color=#ea9999>";
                    break;
                case StatType.Armor:
                    tag = "<color=#f6b26b>";
                    break;
                case StatType.MaxHp:
                    tag = "<color=#b6d7a8>";
                    break;
                case StatType.HpRegen:
                    tag = "<color=#c27ba0>";
                    break;
                case StatType.Speed:
                    tag = "<color=#6fa8dc>";
                    break;
                case StatType.Critical:
                    tag = "<color=#4285f4>";
                    break;
                case StatType.CriticalMultiplier:
                    tag = "<color=#76a5af>";
                    break;
            }
            return tag;
        }
    }
}
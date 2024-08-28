using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace ItemLootSystem
{
    public class AffixCSVtoSO
    {
        #region ########### FILE PATH  ###########
        const string AFFIXESPATH = "Assets/0.Workspace/JunHyoung/CSV/EquipAffixes.csv";

        const string SOSAVEPATH = "Assets/0.Workspace/JunHyoung/SO/Affixes/";

        const string AFFIXTABLEPATH = "Assets/0.Workspace/JunHyoung/SO/AffixTable.asset";

        #endregion

        #region ########### INDEX ###########

        const int STATTYPEGRADE = 0; // 타입 및 단계 

        const int POSITIVENAME = 1;
        const int FIRSTPOSITIVENAME = 2;
        const int NEGATIVENAME = 3;
        const int FIRSTNEGATIVENAME = 4;

        const int STATVALUE = 5;
        const int WEIGHT = 6;
        #endregion

        [MenuItem("Util/Convert Affix CSV", priority = 101)]
        public static void ConvertAffixCSVtoSO()
        {
            string[] allLines = File.ReadAllLines(AFFIXESPATH);
            int cnt = 0;

            foreach (string allLine in allLines)
            {
                string[] splitData = allLine.Split(',');
                CreateAffixData(splitData, cnt++);
            }
            AssetDatabase.SaveAssets();
            UpdateAffixTable();
        }

        static EquipAffix CreateAffixData(string[] splitData, int cnt)
        {
            EquipAffix affix = ScriptableObject.CreateInstance<EquipAffix>();

            string statTypeGrade = splitData[STATTYPEGRADE];

            affix.ID= cnt;

            affix.statType = GetStatType(statTypeGrade);
            affix.grade = GetGrade(statTypeGrade);


            affix.positiveName = splitData[POSITIVENAME];
            affix.firstPositiveName = splitData[FIRSTPOSITIVENAME];
            affix.negativeName = splitData[NEGATIVENAME];
            affix.firstNegativeName = splitData[FIRSTNEGATIVENAME];

            float value;
            if (float.TryParse(splitData[STATVALUE], out value))
                affix.value = value;

            float weight;
            if (float.TryParse(splitData[WEIGHT].TrimEnd('%'), out weight))
                affix.weight = weight;

            AssetDatabase.CreateAsset(affix, $"{SOSAVEPATH}{statTypeGrade}.asset");
            Debug.Log($"Create '{statTypeGrade}.asset' ");
            return affix;
        }


        private static StatType GetStatType(string statTypeGrade)
        {
            if (statTypeGrade.StartsWith("공격력")) return StatType.Power;
            if (statTypeGrade.StartsWith("방어력")) return StatType.Armor;
            if (statTypeGrade.StartsWith("체력")) return StatType.MaxHp;
            if (statTypeGrade.StartsWith("초당 체력 회복")) return StatType.HpRegen;
            if (statTypeGrade.StartsWith("이동속도")) return StatType.Speed;
            if (statTypeGrade.StartsWith("크리티컬 확률")) return StatType.Critical;
            if (statTypeGrade.StartsWith("크리티컬 배율")) return StatType.CriticalMultiplier;
            return 0;
        }

        private static int GetGrade(string statTypeGrade)
        {
            if (statTypeGrade.Contains("1단계")) return 1;
            if (statTypeGrade.Contains("2단계")) return 2;
            if (statTypeGrade.Contains("3단계")) return 3;
            if (statTypeGrade.Contains("4단계")) return 4;
            return 0;
        }


        [MenuItem("Util/Update Affix Table", priority = 201)]
        public static void UpdateAffixTable()
        {
            // 지정된 폴더 경로
            string folderPath = SOSAVEPATH;
            // AffixTable이 저장될 경로
            string affixTablePath = AFFIXTABLEPATH;

            // 해당 폴더 내의 모든 EquipAffix ScriptableObject를 찾음
            string[] guids = AssetDatabase.FindAssets("t:EquipAffix", new[] { folderPath });
            List<EquipAffix> affixes = new List<EquipAffix>();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                EquipAffix affix = AssetDatabase.LoadAssetAtPath<EquipAffix>(assetPath);
                if (affix != null)
                {
                    affixes.Add(affix);
                }
            }

            // AffixTable을 로드하거나 없으면 새로 생성
            EquipAffixesTable affixTable = AssetDatabase.LoadAssetAtPath<EquipAffixesTable>(affixTablePath);
            if (affixTable == null)
            {
                affixTable = ScriptableObject.CreateInstance<EquipAffixesTable>();
                AssetDatabase.CreateAsset(affixTable, affixTablePath);
            }

            // affixes 리스트 업데이트
            affixTable.affixes = affixes;

            // 변경사항 저장
            EditorUtility.SetDirty(affixTable);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("AffixTable 업데이트 완료!");
        }

    }
}
#endif
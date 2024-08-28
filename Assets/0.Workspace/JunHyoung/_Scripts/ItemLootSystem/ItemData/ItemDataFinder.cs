using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;



#if UNITY_EDITOR
namespace ItemLootSystem
{
    /// <summary>
    /// BaseItmeData Asset 찾아서, ItemDataManager의 딕셔너리에 삽입 
    /// +
    /// 보물상자 데이터 테이블 생성
    /// </summary>
    public class ItemDataFinder
    {
        #region ########### FILE PATH  ###########

        const string ITEMPATH = "Assets/0.Workspace/JunHyoung/SO";
        const string DICPATH = "Assets/0.Workspace/JunHyoung/SO/DropTable&Pool/ItemDictionary.asset";

        const string TreasureRarityTable = "Assets/0.Workspace/JunHyoung/CSV/보물상자 레어도별 확률 데이터테이블.csv";
        const string TrasureItmeTable = "Assets/0.Workspace/JunHyoung/CSV/보물상자별 획득 가능 아이템 데이터테이블.csv";

        #endregion

        #region ########### INDEX ###########
        const int CHEST_ID = 0;

        const int ITEM_TYPE = 1;
        const int ITEM_RARITY = 2;
        const int DROP_PROBLITY = 3;
        const int FIRSTNEGATIVENAME = 4;

        const int ITEM_ID = 1;
        #endregion

        [MenuItem("Util/Update Tresure DropTable", priority = 203)]
        static void SetTresureBoxDropTable()
        {
            // 기존의 Item Dictionary , ItemDropTable Read 
            ItemDictionary itemDictionary = AssetDatabase.LoadAssetAtPath<ItemDictionary>(DICPATH);
            if (itemDictionary == null)
            {
                itemDictionary = ScriptableObject.CreateInstance<ItemDictionary>();
                AssetDatabase.CreateAsset(itemDictionary, DICPATH);
            }

            string[] guids = AssetDatabase.FindAssets("t: ItemDropTable", new[] { ITEMPATH });
            Dictionary<int, ItemDropTable> tables = new Dictionary<int, ItemDropTable>();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ItemDropTable tableData = AssetDatabase.LoadAssetAtPath<ItemDropTable>(assetPath);
                if (tableData != null)
                {
                    tables.Add(tableData.id, tableData);
                }
            }


            ReadItemTable(tables, itemDictionary);


        }

        static void ReadRarityTable(Dictionary<int, ItemDropTable> tables, ItemDictionary dictionary)
        {
            string[] allLines = File.ReadAllLines(TreasureRarityTable);

            int line = 0;
            foreach (string allLine in allLines)
            {
                string[] splitData = allLine.Split(',');

                //위에 두줄은 안쓰는 데이터임 -,- ;
                if (line < 2)
                {
                    line++;
                    continue;
                }

            


            }
        }

        static void ReadItemTable(Dictionary<int, ItemDropTable> tables ,ItemDictionary dictionary)
        {
            string[] allLines = File.ReadAllLines(TrasureItmeTable);

            int line = 0;
            foreach (string allLine in allLines)
            {
                string[] splitData = allLine.Split(',');

                //위에 두줄은 안쓰는 데이터임 -,- ;
                if (line < 2)
                {
                    line++;
                    continue;
                }

                int.TryParse(splitData[CHEST_ID], out int chestID);
                int.TryParse(splitData[ITEM_ID], out int itemID);

                ItemDropTable targetTable = tables[chestID];
                BaseItemData targetItem = dictionary.itemDB[itemID];

                foreach(ItemPool pool in targetTable.pools)
                {
                    if(pool.poolType == targetItem.itemType)
                    {
                        pool.items.Add(targetItem);
                    }
                }

                }
        }


        [MenuItem("Util/Update Item Dic", priority = 203)]
        static void SetDictionary()
        {
            // 지정된 폴더 경로
            string folderPath = ITEMPATH;
            string DICTablePath = DICPATH;

            // 해당 폴더 내의 모든 EquipAffix ScriptableObject를 찾음
            string[] guids = AssetDatabase.FindAssets("t: BaseItemData", new[] { folderPath });
            List<BaseItemData> items = new List<BaseItemData>();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                BaseItemData itemData = AssetDatabase.LoadAssetAtPath<BaseItemData>(assetPath);
                if (itemData != null)
                {
                    items.Add(itemData);
                }
            }

            // AItemDictionary 로드하거나 없으면 새로 생성
            ItemDictionary itemDictionary = AssetDatabase.LoadAssetAtPath<ItemDictionary>(DICTablePath);
            if (itemDictionary == null)
            {
                itemDictionary = ScriptableObject.CreateInstance<ItemDictionary>();
                AssetDatabase.CreateAsset(itemDictionary, DICTablePath);
            }

            itemDictionary.itemDB.Clear();

            //DIC 업데이트
            foreach (var item in items)
            {
                itemDictionary.itemDB.TryAdd(item.ID, item);
            }

            // 변경사항 저장
            EditorUtility.SetDirty(itemDictionary);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Item Dictionary 업데이트 완료!");
        }
    }

}

#endif
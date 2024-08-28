using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace ItemLootSystem
{
    /// <summary>
    /// CSV 파일을 ItemData ScriptableObject로 변환해 생성해주는 클래스
    /// </summary>
    public class ItemCSVtoSO
    {
        #region ########### FILE PATH  ###########
        const string ITEMCSVPATH = "... PATH";
        //const string EQUIPCSVPATH = "Assets/0.Workspace/JunHyoung/CSV/SAMPLE_EquipDataTable.csv"; //구버전
        const string EQUIPCSVPATH = "Assets/0.Workspace/TaeHong/Data/CSV/EquipmentTable.csv"; //신버전
        const string POTION_CSV_PATH = "Assets/0.Workspace/TaeHong/Data/CSV/PotionTable.csv";
        const string UNKNOWN_POTION_CSV_PATH = "Assets/0.Workspace/TaeHong/Data/CSV/UnknownPotionTable.csv";
        const string ETC_ITEM_CSV_PATH = "Assets/0.Workspace/TaeHong/Data/CSV/ETCItemTable.csv";
        const string DEAFAULTSOPATH = "..PATH";

        // 아이템 분류별 지정해둔 폴더에 생성?
        const string COMMSUMABLESOPATH = DEAFAULTSOPATH + "...PATH";
        const string EQUIPMENTSOPATH =  "Assets/0.Workspace/JunHyoung/SO/Equipments";
        const string WEAPONITEMPATH = EQUIPMENTSOPATH + "/Weapons/";
        const string ARMORITEMPATH = EQUIPMENTSOPATH + "/Armors/";
        const string ACCESSORYITEMPATH = EQUIPMENTSOPATH + "/Accessorys/";

        const string POTION_SO_PATH = "Assets/0.Workspace/TaeHong/Data/Items/PotionData/";
        const string UNKNOWN_POTION_SO_PATH = "Assets/0.Workspace/TaeHong/Data/Items/UnknownPotionData/";
        const string ETC_ITEM_SO_PATH = "Assets/0.Workspace/TaeHong/Data/Items/ETCItemData/";

        //임시 
        const string ICONATLASPATH = "Assets/Imports/UI/Inventory.png"; //Resources.LoadAll<Sprite> 로 접근해서 Sprite[] 사용해야하나본데
        #endregion

        // index 확인할것.
        #region ########### INDEX ###########
        const int ID = 0;
        const int NAME = 1;
        const int RARITY = 2;
        //const int ITEMTYPE = 4;
        const int EQUIPTYPE = 3;
        const int EQUIPGROUP = 4;
        const int WEAPONTYPE = 5;

        // STAT DATA INDEX
        const int MAINSTAT = 6; 
        const int MSAMOUNT = 7;
        const int SUBSTAT = 8;
        const int SSAMOUNT = 9;

        const int WEIGHT = 10; // ==Dropper in Sheet
        const int RESOURCEPATH = 11;
        const int TOOLTIP = 12;

        const int REMARKS = 13;
        const int CAUTIONS = 14;
        #endregion

        static Dictionary<string, Sprite> spritesheetDic = new Dictionary<string, Sprite>();


        public static void ParseSprites()
        {
            spritesheetDic.Clear();
            foreach(Sprite sprite in Resources.LoadAll<Sprite>("Spritesheets"))
            {
                spritesheetDic.Add(sprite.name, sprite);
            }
        }

        [MenuItem("Util/Convert EQUIP CSV")]
        public static void ConvertEQUIPCSVtoSO()
        {
            string[] allLines = File.ReadAllLines(EQUIPCSVPATH);
            //List<Dictionary<string, object>> values = CSVReader.Read(EQUIPMENTSOPATH); <- 흠 이거 써야하나

            ParseSprites();

            int line = 0;
            foreach (string allLine in allLines)
            {
                string[] splitData = allLine.Split(',');

                //위에 두줄은 안쓰는 데이터임 -,- ;
                if(line < 2)
                {
                    line++;
                    continue;
                }

                int equipType;
                EquipmentType type = EquipmentType.NULL;
                if (int.TryParse(splitData[EQUIPTYPE], out equipType))
                     type =(EquipmentType) equipType;

                switch(type)
                {
                    case EquipmentType.Weapon:
                        CreateWeaponItemData(splitData, WEAPONITEMPATH);
                        break;
                    case EquipmentType.Armor:
                        CreateEquipItemData(splitData, ARMORITEMPATH); 
                        break;
                    case EquipmentType.Accessory:
                        CreateEquipItemData(splitData, ACCESSORYITEMPATH);
                        break;
                    default: break;
                }

            }
           AssetDatabase.SaveAssets();
        }

        static EquipItemData CreateEquipItemData(string[] splitData, string PATH)
        {
            // 이름 불러오기 
            string name = splitData[NAME];

            // 이미 있으면 최신화, 없으면 새로 생성
            EquipItemData itemData = AssetDatabase.LoadAssetAtPath<EquipItemData>($"{PATH}{name}.asset");
            if (itemData == null)
            {
                itemData = ScriptableObject.CreateInstance<EquipItemData>();
                AssetDatabase.CreateAsset(itemData, $"{PATH}{name}.asset");
                Debug.Log($"Create '{name}.asset' ");
            }

            itemData.ID = int.Parse(splitData[ID]);

            itemData.Name = name;

            int rarity;
            if (int.TryParse(splitData[RARITY], out rarity))
                itemData.rarity = (ItemRarity) rarity - 1;  //실제 정의는 0부터 시작해서... 

            itemData.itemType = ItemType.Equipment;

            int equipType;
            if (int.TryParse(splitData[EQUIPTYPE], out equipType))
                itemData.equipmentType = (EquipmentType) equipType;

            int group;
            if (int.TryParse(splitData[EQUIPGROUP], out group))
                itemData.equipmentGroup = (EquipmentGroup) group;

            itemData.mainStat = (StatType) int.Parse(splitData[MAINSTAT]);
            float mainStatAmount;
            if (float.TryParse(splitData[MSAMOUNT], out mainStatAmount))
                itemData.mainStatAmount = mainStatAmount;

            int subStat;
            if (int.TryParse(splitData[SUBSTAT], out subStat))
                itemData.subStat = (StatType) subStat;

            float subStatAmount;
            if (float.TryParse(splitData[SUBSTAT], out subStatAmount))
                itemData.subStatAmount = subStatAmount;

            itemData.weight = int.Parse(splitData[WEIGHT]);

            if (splitData[RESOURCEPATH] == "추후 제작")
                itemData.icon = spritesheetDic["Yellow Torch"];
            else
                itemData.icon = spritesheetDic[$"{splitData[RESOURCEPATH].Split('/')[1]}"];

            itemData.toolTip = splitData[TOOLTIP];

            return itemData;
        }
       

        static WeaponItemData CreateWeaponItemData(string[] splitData, string PATH)
        {
            // 이름 불러오기 
            string name = splitData[NAME];

            // 이미 있으면 최신화, 없으면 새로 생성
            WeaponItemData itemData = AssetDatabase.LoadAssetAtPath<WeaponItemData>($"{PATH}{name}.asset");
            if (itemData == null)
            {
                itemData = ScriptableObject.CreateInstance<WeaponItemData>();
                AssetDatabase.CreateAsset(itemData, $"{PATH}{name}.asset");
                Debug.Log($"Create '{name}.asset' ");
            } 

            itemData.Name = name;

            if (int.TryParse(splitData[ID], out int id))
                itemData.ID = id;

            int rarity;
            if (int.TryParse(splitData[RARITY], out rarity))
                itemData.rarity = (ItemRarity) rarity - 1;  //실제 정의는 0부터 시작해서... 

            itemData.itemType = ItemType.Equipment;

            int equipType;
            if (int.TryParse(splitData[EQUIPTYPE], out equipType))
                itemData.equipmentType = (EquipmentType) equipType;

            int group;
            if (int.TryParse(splitData[EQUIPGROUP], out group))
                itemData.equipmentGroup = (EquipmentGroup) group;

            int weaponType;
            if (int.TryParse(splitData[EQUIPTYPE], out weaponType))
                itemData.weaponType = (WeaponType) weaponType;


            itemData.mainStat = (StatType) int.Parse(splitData[MAINSTAT]);
            float mainStatAmount;
            if (float.TryParse(splitData[MSAMOUNT], out mainStatAmount))
                itemData.mainStatAmount = mainStatAmount;

            int subStat;
            if (int.TryParse(splitData[SUBSTAT], out subStat))
                itemData.subStat = (StatType) subStat;

            float subStatAmount;
            if (float.TryParse(splitData[SUBSTAT], out subStatAmount))
                itemData.subStatAmount = subStatAmount;

            itemData.weight = int.Parse(splitData[WEIGHT]);

            if (splitData[RESOURCEPATH] == "추후 제작")
                itemData.icon = spritesheetDic["Yellow Torch"];
            else
                itemData.icon = spritesheetDic[$"{splitData[RESOURCEPATH].Split('/')[1]}"];

            itemData.toolTip = splitData[TOOLTIP]; 

            return itemData;
        }

        [MenuItem("Util/Convert Test")]
        public static void TESTCSVtoSO()
        {
            string[] allLines = File.ReadAllLines( ETC_ITEM_CSV_PATH);
            Debug.Log(allLines.Length);

            foreach(string allLine in allLines)
            {
                string[] splitData = allLine.Split(',');
                int index = 0;

                foreach (var data in splitData)
                {
                    Debug.Log($"{index} : {data}");
                    index++;
                }
            }
        }

        [MenuItem("Util/Convert Potion CSV")]
        public static void CSVtoPotion()
        {
            ParseSprites();
            Debug.Log("convert potion csv");
            string[] allLines = File.ReadAllLines(POTION_CSV_PATH);

            int line = 0;
            foreach (string allLine in allLines)
            {
                string[] splitData = allLine.Split(',');

                if (line < 2)
                {
                    line++;
                    continue;
                }

                CreatePotionSO(splitData, POTION_SO_PATH);
            }

            Debug.Log("convert unknown potion csv");
            allLines = File.ReadAllLines(UNKNOWN_POTION_CSV_PATH);

            line = 0;
            foreach (string allLine in allLines)
            {
                string[] splitData = allLine.Split(',');

                if (line < 1)
                {
                    line++;
                    continue;
                }

                CreateUnknownPotionSO(splitData, UNKNOWN_POTION_SO_PATH);
            }

            AssetDatabase.SaveAssets();
        }

        static BaseItemData CreatePotionSO(string[] splitData, string PATH)
        {
            const int POTION_ID = 0;
            const int POTION_NAME = 2;
            const int POTION_RARITY = 3;
            const int POTION_TOOLTIP = 4;
            const int POTION_WEIGHT = 12;

            // ID 불러오기 
            int id = int.Parse(splitData[POTION_ID]);

            // 이미 있으면 최신화, 없으면 새로 생성
            BaseItemData itemData = AssetDatabase.LoadAssetAtPath<BaseItemData>($"{PATH}Potion-{id}.asset");
            if (itemData == null)
            {
                itemData = ScriptableObject.CreateInstance<BaseItemData>();
                AssetDatabase.CreateAsset(itemData, $"{PATH}Potion-{id}.asset");
                Debug.Log($"Create '{itemData.Name}.asset' ");
            }
                
            itemData.ID = id;
            itemData.itemType = ItemType.Consumable;
            itemData.isPotion = true;

            itemData.Name = splitData[POTION_NAME];

            string rarity = splitData[POTION_RARITY];
            switch (rarity)
            {
                case "노말":
                    itemData.rarity = ItemRarity.Normal;
                    break;
                case "레어":
                    itemData.rarity = ItemRarity.Rare;
                    break;
                case "에픽":
                    itemData.rarity = ItemRarity.Epic;
                    break;
                case "유니크":
                    itemData.rarity = ItemRarity.Unique;
                    break;
                case "레전더리":
                    itemData.rarity = ItemRarity.Legendary;
                    break;
            }

            itemData.toolTip = splitData[POTION_TOOLTIP];
            itemData.weight = int.Parse(splitData[POTION_WEIGHT]);

            // 변경사항 저장
            EditorUtility.SetDirty(itemData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Update '{itemData.Name}.asset' ");
            return itemData;
        }

        static UnknownPotionData CreateUnknownPotionSO(string[] splitData, string PATH)
        {
            const int ID = 0;
            const int NAME = 1;
            const int TOOLTIP = 2;
            const int RESOURCE = 3;

            // ID 불러오기 
            int id = int.Parse(splitData[ID]);

            // 이미 있으면 최신화, 없으면 새로 생성
            UnknownPotionData potionData = AssetDatabase.LoadAssetAtPath<UnknownPotionData>($"{PATH}Potion-{id}.asset");
            if (potionData == null)
            {
                potionData = ScriptableObject.CreateInstance<UnknownPotionData>();
                AssetDatabase.CreateAsset(potionData, $"{PATH}Potion-{id}.asset");
                Debug.Log($"Create '{potionData.potionName}.asset' ");
            }

            potionData.id = id;
            potionData.potionName = splitData[NAME];
            potionData.tooltip = splitData[TOOLTIP];

            Debug.Log(splitData[ID]);
            Debug.Log(splitData[NAME]);
            Debug.Log(splitData[TOOLTIP]);
            Debug.Log(splitData.Length);
            string[] split = splitData[RESOURCE].Split('/');
            string filename = splitData[RESOURCE].Split('/')[split.Length - 1];
            potionData.sprite = spritesheetDic[filename];

            Debug.Log($"Update '{potionData.potionName}.asset' ");
            return potionData;
        }

        [MenuItem("Util/Convert ETC Items")]
        public static void ETCItemCSVtoSO()
        {
            ParseSprites();
            string[] allLines = File.ReadAllLines(ETC_ITEM_CSV_PATH);

            int line = 0;
            foreach (string allLine in allLines)
            {
                string[] splitData = allLine.Split(',');

                if (line < 1)
                {
                    line++;
                    continue;
                }

                CreateETCItemSO(splitData, ETC_ITEM_SO_PATH);
            }
        }

        static BaseItemData CreateETCItemSO(string[] splitData, string PATH)
        {
            const int ID = 0;
            const int NAME = 1;
            const int TYPE = 2;
            const int MAXDROPS = 3;
            const int PRICE = 4;
            const int TOOLTIP = 6;
            const int RESOURCE = 7;

            // ID 불러오기 
            int id = int.Parse(splitData[ID]);

            // 이미 있으면 최신화, 없으면 새로 생성
            BaseItemData itemData = AssetDatabase.LoadAssetAtPath<BaseItemData>($"{PATH}ETCItem-{id}.asset");
            if (itemData == null)
            {
                itemData = ScriptableObject.CreateInstance<BaseItemData>();
                AssetDatabase.CreateAsset(itemData, $"{PATH}ETCItem-{id}.asset");
                Debug.Log($"Create '{itemData.Name}.asset' ");
            }

            itemData.itemType = ItemType.ETC;

            itemData.Name = splitData[NAME];

            itemData.etcItemType = splitData[TYPE];

            if (int.TryParse(splitData[MAXDROPS], out int drops))
                itemData.maxDrops = drops;

            if (int.TryParse(splitData[PRICE], out int price))
                itemData.cost = price;

            itemData.toolTip = splitData[TOOLTIP];

            string[] split = splitData[RESOURCE].Split('/');
            string filename = splitData[RESOURCE].Split('/')[split.Length - 1];
            itemData.icon = spritesheetDic[filename];

            Debug.Log($"Update '{itemData.Name}.asset' ");
            return itemData;
        }
    }
}
#endif
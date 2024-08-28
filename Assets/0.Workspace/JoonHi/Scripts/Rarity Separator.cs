using ItemLootSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class RaritySeparator
{
    #region PATH
    // Rarity Data Creating Spot Path
    const string RARITYDATACREATEPATH = /*"Assets/0.Workspace/JoonHi/Resources/Test/RarityData"*/"Assets/0.Workspace/JunHyoung/SO/DropTable&Pool/RarityData";
    // Equip Rarity Data Creating Spot Path
    const string EQUIPRARITYDATACREATEPATH = /*"Assets/0.Workspace/JoonHi/Resources/Test/EquipRarityData"*/"Assets/0.Workspace/JunHyoung/SO/DropTable&Pool/EquipRarityData";

    /*
    // Resourse Files
    const string RARITYDATAPATH = "Test/RarityData";
    const string EQUIPRARITYDATAPATH = "Test/EquipRarityData";
    const string EQUIPMENTSPATH = "Test/Equip";
    const string COMMSUMABLESPATH = "Test/Commsumable";
    const string KEYPATH = "Test/KEY";
    const string ETCPATH = "Test/ETC";
    */

    // Resourse Dictionary
    const string ITEMDICTIONARYPATH = "Assets/0.Workspace/JunHyoung/SO/DropTable&Pool/ItemDictionary.asset";

    #endregion

    private static RarityData rarityData;
    private static EquipRarityData equipRarityData;
    /*
    private static EquipItemData[] allEquipmentData;
    private static BaseItemData[] allCommsumableData;
    private static BaseItemData[] allKeyData;
    private static BaseItemData[] allETCData;
    */
    private static List<EquipItemData> allEquipmentData = new List<EquipItemData>();
    private static List<BaseItemData> allCommsumableData = new List<BaseItemData>();
    private static List<BaseItemData> allKeyData = new List<BaseItemData>();
    private static List<BaseItemData> allETCData = new List<BaseItemData>();

    private static List<EquipItemData> allWeaponsData = new List<EquipItemData>();
    private static List<EquipItemData> allArmorData = new List<EquipItemData>();
    private static List<EquipItemData> allAccessoryData = new List<EquipItemData>();

    #region Full Separator

    [MenuItem("Util/Rarity Separator/Separate ALL Item Rarity Data", false, priority = 1)]
    public static void SeparateALLRarity()
    {
        // Initialise
        ClearRarityData();

        // 데이터를 로드
        LoadData();

        /*
        // 분류한 데이터를 넣을 데이터 에셋 탐색/생성
        rarityData = Resources.Load<RarityData>(RARITYDATAPATH);
        if (rarityData == null)
        {
            Debug.Log("Rarity Data Not Found: Creating RarityData.asset");
            RarityData rarityDatainst = ScriptableObject.CreateInstance<RarityData>();
            AssetDatabase.CreateAsset(rarityDatainst, $"{RARITYDATACREATEPATH}.asset");
            rarityData = Resources.Load<RarityData>(RARITYDATAPATH);
        }
        Debug.Log(rarityData.name);

        equipRarityData = Resources.Load<EquipRarityData>(EQUIPRARITYDATACREATEPATH);
        if (equipRarityData == null)
        {
            Debug.Log("Rarity Data Not Found: Creating RarityData.asset");
            EquipRarityData equipRarityDatainst = ScriptableObject.CreateInstance<EquipRarityData>();
            AssetDatabase.CreateAsset(equipRarityDatainst, $"{EQUIPRARITYDATACREATEPATH}.asset");
            equipRarityData = Resources.Load<EquipRarityData>(EQUIPRARITYDATACREATEPATH);
        }
        Debug.Log(equipRarityData.name);
        */

        // 전체 분류
        SeparateEquiptmentRarity();
        SeparateCommsumableRarity();
        SeparateKeyRarity();
        SeparateETCRarity();

        SeparateEquipmentType();

        // 변경사항 저장
        EditorUtility.SetDirty(rarityData);
        EditorUtility.SetDirty(equipRarityData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    #endregion

    #region Individual Separator

    [MenuItem("Util/Rarity Separator/Separate Equiptment Rarity Data", false, priority = 101)]
    public static void SeparateEquiptmentRarity()
    {
        for (int i = 0; i < allEquipmentData.Count; i++)
        {
            switch (allEquipmentData[i].rarity)
            {
                case ItemRarity.Normal:
                    rarityData.NormalEquiptment.Add(allEquipmentData[i]);
                    break;
                case ItemRarity.Rare:
                    rarityData.RareEquiptment.Add(allEquipmentData[i]);
                    break;
                case ItemRarity.Epic:
                    rarityData.EpicEquiptment.Add(allEquipmentData[i]);
                    break;
                case ItemRarity.Unique:
                    rarityData.UniqueEquiptment.Add(allEquipmentData[i]);
                    break;
                case ItemRarity.Legendary:
                    rarityData.LegendaryEquiptment.Add(allEquipmentData[i]);
                    break;
                case ItemRarity.Artifact:
                    rarityData.ArtifactEquiptment.Add(allEquipmentData[i]);
                    break;
            }
        }
        Debug.Log("Equipment Rarity Separate Done");
    }

    [MenuItem("Util/Rarity Separator/Separate Commsumable Item Rarity Data", false, priority = 102)]
    public static void SeparateCommsumableRarity()
    {
        for (int i = 0; i < allCommsumableData.Count; i++)
        {
            switch (allCommsumableData[i].rarity)
            {
                case ItemRarity.Normal:
                    rarityData.NormalCommsumable.Add(allCommsumableData[i]);
                    break;
                case ItemRarity.Rare:
                    rarityData.RareCommsumable.Add(allCommsumableData[i]);
                    break;
                case ItemRarity.Epic:
                    rarityData.EpicCommsumable.Add(allCommsumableData[i]);
                    break;
                case ItemRarity.Unique:
                    rarityData.UniqueCommsumable.Add(allCommsumableData[i]);
                    break;
                case ItemRarity.Legendary:
                    rarityData.LegendaryCommsumable.Add(allCommsumableData[i]);
                    break;
            }
        }
        Debug.Log("Commsumable Item Rarity Separate Done");
    }

    [MenuItem("Util/Rarity Separator/Separate Key Rarity Data", false, priority = 103)]
    public static void SeparateKeyRarity()
    {
        for (int i = 0; i < allKeyData.Count; i++)
        {
            switch (allKeyData[i].rarity)
            {
                case ItemRarity.Epic:
                    rarityData.EpicKey.Add(allKeyData[i]);
                    break;
                case ItemRarity.Unique:
                    rarityData.UniqueKey.Add(allKeyData[i]);
                    break;
                case ItemRarity.Legendary:
                    rarityData.LegendaryKey.Add(allKeyData[i]);
                    break;
                default:
                    break;
            }
        }
        Debug.Log("Key Rarity Separate Done");
    }

    [MenuItem("Util/Rarity Separator/Separate ETC Rarity Data", false, priority = 104)]
    public static void SeparateETCRarity()
    {
        for (int i = 0; i < allETCData.Count; i++)
        {
            switch (allETCData[i].rarity)
            {
                case ItemRarity.Normal:
                    rarityData.NormalETC.Add(allETCData[i]);
                    break;
                case ItemRarity.Rare:
                    rarityData.RareETC.Add(allETCData[i]);
                    break;
                case ItemRarity.Epic:
                    rarityData.EpicETC.Add(allETCData[i]);
                    break;
                case ItemRarity.Unique:
                    rarityData.UniqueETC.Add(allETCData[i]);
                    break;
                case ItemRarity.Legendary:
                    rarityData.LegendaryETC.Add(allETCData[i]);
                    break;
            }
        }
        Debug.Log("ETC Item Rarity Separate Done");
    }

    [MenuItem("Util/Rarity Separator/Separate Equipment Type Rarity Data", false, priority = 201)]
    // Equipment Rarity Separator
    public static void SeparateEquipmentType()
    {
        // 장비 종류 분류
        for (int i = 0; i < allEquipmentData.Count; i++)
        {
            switch (allEquipmentData[i].equipmentType)
            {
                case EquipmentType.Weapon:
                    allWeaponsData.Add(allEquipmentData[i]);
                    break;
                case EquipmentType.Armor:
                    allArmorData.Add(allEquipmentData[i]);
                    break;
                case EquipmentType.Accessory:
                    allAccessoryData.Add(allEquipmentData[i]);
                    break;
            }
        }

        // 무기 희귀도 분류
        for (int i = 0; i < allWeaponsData.Count; i++)
        {
            switch (allWeaponsData[i].rarity)
            {
                case ItemRarity.Normal:
                    equipRarityData.NormalWeapon.Add(allWeaponsData[i]);
                    break;
                case ItemRarity.Rare:
                    equipRarityData.RareWeapon.Add(allWeaponsData[i]);
                    break;
                case ItemRarity.Epic:
                    equipRarityData.EpicWeapon.Add(allWeaponsData[i]);
                    break;
                case ItemRarity.Unique:
                    equipRarityData.UniqueWeapon.Add(allWeaponsData[i]);
                    break;
                case ItemRarity.Legendary:
                    equipRarityData.LegendaryWeapon.Add(allWeaponsData[i]);
                    break;
                case ItemRarity.Artifact:
                    equipRarityData.ArtifactWeapon.Add(allWeaponsData[i]);
                    break;
            }
        }

        // 갑옷 희귀도 분류
        for (int i = 0; i < allArmorData.Count; i++)
        {
            switch (allArmorData[i].rarity)
            {
                case ItemRarity.Normal:
                    equipRarityData.NormalArmor.Add(allArmorData[i]);
                    break;
                case ItemRarity.Rare:
                    equipRarityData.RareArmor.Add(allArmorData[i]);
                    break;
                case ItemRarity.Epic:
                    equipRarityData.EpicArmor.Add(allArmorData[i]);
                    break;
                case ItemRarity.Unique:
                    equipRarityData.UniqueArmor.Add(allArmorData[i]);
                    break;
                case ItemRarity.Legendary:
                    equipRarityData.LegendaryArmor.Add(allArmorData[i]);
                    break;
                case ItemRarity.Artifact:
                    equipRarityData.ArtifactArmor.Add(allArmorData[i]);
                    break;
            }
        }

        // 장신구 희귀도 분류
        for (int i = 0; i < allAccessoryData.Count; i++)
        {
            switch (allAccessoryData[i].rarity)
            {
                case ItemRarity.Normal:
                    equipRarityData.NormalAccessory.Add(allAccessoryData[i]);
                    break;
                case ItemRarity.Rare:
                    equipRarityData.RareAccessory.Add(allAccessoryData[i]);
                    break;
                case ItemRarity.Epic:
                    equipRarityData.EpicAccessory.Add(allAccessoryData[i]);
                    break;
                case ItemRarity.Unique:
                    equipRarityData.UniqueAccessory.Add(allAccessoryData[i]);
                    break;
                case ItemRarity.Legendary:
                    equipRarityData.LegendaryAccessory.Add(allAccessoryData[i]);
                    break;
                case ItemRarity.Artifact:
                    equipRarityData.ArtifactAccessory.Add(allAccessoryData[i]);
                    break;
            }
        }
    }

    #endregion

    #region Clear

    [MenuItem("Util/Rarity Separator/Clear Item Rarity Data", false, priority = 301)]
    public static void ClearRarityData()
    {
        /*
        rarityData = Resources.Load<RarityData>(RARITYDATAPATH);
        equipRarityData = Resources.Load<EquipRarityData>(EQUIPRARITYDATAPATH);

        if (rarityData == null)
        {
            Debug.Log("Rarity Data Not Found: Creating RarityData.asset");
            RarityData rarityDatainst = ScriptableObject.CreateInstance<RarityData>();
            AssetDatabase.CreateAsset(rarityDatainst, $"{RARITYDATACREATEPATH}.asset");
            rarityData = Resources.Load<RarityData>(RARITYDATAPATH);
        }

        if (equipRarityData == null)
        {
            Debug.Log("Equip Rarity Data Not Found: Creating RarityData.asset");
            EquipRarityData equipRarityDatainst = ScriptableObject.CreateInstance<EquipRarityData>();
            AssetDatabase.CreateAsset(equipRarityDatainst, $"{EQUIPRARITYDATACREATEPATH}.asset");
            equipRarityData = Resources.Load<EquipRarityData>(EQUIPRARITYDATAPATH);
        }
        Debug.Log(rarityData.name);
        Debug.Log(equipRarityData.name);
        */

        rarityData = AssetDatabase.LoadAssetAtPath<RarityData>($"{RARITYDATACREATEPATH}.asset");
        equipRarityData = AssetDatabase.LoadAssetAtPath<EquipRarityData>($"{EQUIPRARITYDATACREATEPATH}.asset");
        if (rarityData == null)
        {
            Debug.Log("Rarity Data Not Found: Creating RarityData.asset");
            RarityData rarityDatainst = ScriptableObject.CreateInstance<RarityData>();
            AssetDatabase.CreateAsset(rarityDatainst, $"{RARITYDATACREATEPATH}.asset");
            rarityData = AssetDatabase.LoadAssetAtPath<RarityData>(RARITYDATACREATEPATH);
        }
        if (equipRarityData == null)
        {
            Debug.Log("Equip Rarity Data Not Found: Creating RarityData.asset");
            EquipRarityData equipRarityDatainst = ScriptableObject.CreateInstance<EquipRarityData>();
            AssetDatabase.CreateAsset(equipRarityDatainst, $"{EQUIPRARITYDATACREATEPATH}.asset");
            equipRarityData = AssetDatabase.LoadAssetAtPath<EquipRarityData>(EQUIPRARITYDATACREATEPATH);
        }
        Debug.Log(rarityData.name);
        Debug.Log(equipRarityData.name);

        Debug.Log("Rarity Data Found: Clear RarityData.asset");
        rarityData.NormalEquiptment.Clear();
        rarityData.RareEquiptment.Clear();
        rarityData.EpicEquiptment.Clear();
        rarityData.UniqueEquiptment.Clear();
        rarityData.LegendaryEquiptment.Clear();
        rarityData.ArtifactEquiptment.Clear();

        rarityData.NormalCommsumable.Clear();
        rarityData.RareCommsumable.Clear();
        rarityData.EpicCommsumable.Clear();
        rarityData.UniqueCommsumable.Clear();
        rarityData.LegendaryCommsumable.Clear();

        rarityData.NormalETC.Clear();
        rarityData.RareETC.Clear();
        rarityData.EpicETC.Clear();
        rarityData.UniqueETC.Clear();
        rarityData.LegendaryETC.Clear();

        rarityData.EpicKey.Clear();
        rarityData.UniqueKey.Clear();
        rarityData.LegendaryKey.Clear();

        equipRarityData.NormalWeapon.Clear();
        equipRarityData.RareWeapon.Clear();
        equipRarityData.EpicWeapon.Clear();
        equipRarityData.UniqueWeapon.Clear();
        equipRarityData.LegendaryWeapon.Clear();
        equipRarityData.ArtifactWeapon.Clear();

        equipRarityData.NormalArmor.Clear();
        equipRarityData.RareArmor.Clear();
        equipRarityData.EpicArmor.Clear();
        equipRarityData.UniqueArmor.Clear();
        equipRarityData.LegendaryArmor.Clear();
        equipRarityData.ArtifactArmor.Clear();

        equipRarityData.NormalAccessory.Clear();
        equipRarityData.RareAccessory.Clear();
        equipRarityData.EpicAccessory.Clear();
        equipRarityData.UniqueAccessory.Clear();
        equipRarityData.LegendaryAccessory.Clear();
        equipRarityData.ArtifactAccessory.Clear();

        Debug.Log("Rarity Data Clearing Done");
    }

    #endregion

    #region Private Methods

    private static void LoadData()
    {
        /*
        allEquipmentData = Resources.LoadAll<EquipItemData>(EQUIPMENTSPATH);
        Debug.Log($"Equipment All Load : {allEquipmentData.Length} Equipemt Found");
        allCommsumableData = Resources.LoadAll<BaseItemData>(COMMSUMABLESPATH);
        Debug.Log($"Commsumable Items All Load : {allCommsumableData.Length} Commsumable Items Found");
        allKeyData = Resources.LoadAll<BaseItemData>(KEYPATH);
        Debug.Log($"Key All Load : {allKeyData.Length} Key Found");
        allETCData = Resources.LoadAll<BaseItemData>(ETCPATH);
        Debug.Log($"ETC Item All Load : {allKeyData.Length} ETC Items Found");
        */

        ItemDictionary itemDictionary = AssetDatabase.LoadAssetAtPath<ItemDictionary>(ITEMDICTIONARYPATH);

        foreach (BaseItemData itemData in itemDictionary.itemDB.Values)
        {
            switch(itemData.itemType)
            {
                case ItemType.Equipment:
                    allEquipmentData.Add(itemData as EquipItemData);
                    break;
                case ItemType.Consumable:
                    allCommsumableData.Add(itemData);
                    break;
                case ItemType.Key:
                    allKeyData.Add(itemData);
                    break;
                case ItemType.ETC:
                    allETCData.Add(itemData);
                    break;
            }
        }

        Debug.Log($"Equipment All Load : {allEquipmentData.Count} Equipemt Found");
        Debug.Log($"Commsumable Items All Load : {allCommsumableData.Count} Commsumable Items Found");
        Debug.Log($"Key All Load : {allKeyData.Count} Key Found");
        Debug.Log($"ETC Item All Load : {allKeyData.Count} ETC Items Found");
    }
    #endregion

}
#endif
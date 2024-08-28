using AYellowpaper.SerializedCollections;
using ItemLootSystem;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPCs/MeokdoriData")]
public class MeokdoriData : ScriptableObject
{
    public MeokdoriGrade Grade;
    public List<int> Costs = new List<int>();

    [Header("RearityProbabilityWaight")]
    [Space(10), SerializedDictionary("Item Rearity", "Weight")]
    public MeokdoriRarity RearityWaight = new MeokdoriRarity() {
            {ItemRarity.Normal , null},
            {ItemRarity.Rare , null},
            {ItemRarity.Epic , null},
            {ItemRarity.Unique , null},
            {ItemRarity.Legendary, null},
            {ItemRarity.Artifact, null},
    };

    [Header("RearityProbabilityWaight")]
    [Space(10), SerializedDictionary("Item Type", "Weight")]
    public MeokdoriItemTypeWeight ItemTypeWeight = new MeokdoriItemTypeWeight() {
            {ItemType.Equipment , 0},
            {ItemType.Consumable , 0},
            {ItemType.Key , 0},
    };

    [Header("ItemTypeWeight")]
    [Space(10), SerializedDictionary("Equipment Type", "Weight")]
    public MeokdoriEquipTypeWeight EquipTypeWeight = new MeokdoriEquipTypeWeight() {
            {EquipmentType.Weapon , 0},
            {EquipmentType.Armor , 0},
            {EquipmentType.Accessory , 0},
    };
}

public enum MeokdoriGrade { Normal, Golden };

[System.Serializable]
public class MeokdoriRarity : SerializedDictionary<ItemRarity, List<int>> { }

[System.Serializable]
public class MeokdoriItemTypeWeight : SerializedDictionary<ItemType, int> { }

[System.Serializable]
public class MeokdoriEquipTypeWeight : SerializedDictionary<EquipmentType, int> { }

using ItemLootSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipRarityData : ScriptableObject
{
    [Header("Weapons RarityData")]
    public List<BaseItemData> NormalWeapon;
    public List<BaseItemData> RareWeapon;
    public List<BaseItemData> EpicWeapon;
    public List<BaseItemData> UniqueWeapon;
    public List<BaseItemData> LegendaryWeapon;
    public List<BaseItemData> ArtifactWeapon;

    [Header("Armors RarityData")]
    public List<BaseItemData> NormalArmor;
    public List<BaseItemData> RareArmor;
    public List<BaseItemData> EpicArmor;
    public List<BaseItemData> UniqueArmor;
    public List<BaseItemData> LegendaryArmor;
    public List<BaseItemData> ArtifactArmor;

    [Header("Accessories RarityData")]
    public List<BaseItemData> NormalAccessory;
    public List<BaseItemData> RareAccessory;
    public List<BaseItemData> EpicAccessory;
    public List<BaseItemData> UniqueAccessory;
    public List<BaseItemData> LegendaryAccessory;
    public List<BaseItemData> ArtifactAccessory;
}

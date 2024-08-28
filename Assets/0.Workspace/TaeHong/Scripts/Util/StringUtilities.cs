using System;
using UnityEngine;
using ItemLootSystem;

public static class StringUtilities
{
    public static string GetEquipmentTypeName(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Weapon:
                return "무기";
            case EquipmentType.Armor:
                return "갑옷";
            case EquipmentType.Accessory:
                return "장신구";
            default:
                return "";
        }
    }

    public static string GetRarityName(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Normal:
                return "노말";
            case ItemRarity.Rare:
                return "레어";
            case ItemRarity.Epic:
                return "에픽";
            case ItemRarity.Unique:
                return "유니크";
            case ItemRarity.Legendary:
                return "레전더리";
            case ItemRarity.Artifact:
                return "아티펙트";
            case ItemRarity.Excalibur:
                return "엑스칼리버";
            default:
                return "";
        }
    }
}

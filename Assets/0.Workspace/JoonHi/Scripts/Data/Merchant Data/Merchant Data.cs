using AYellowpaper.SerializedCollections;
using ItemLootSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "NPCs/MerchantData")]
public class MerchantData : ScriptableObject
{
    public MerchantGrade Grade;
    public Sprite merchantSprite;
    public Sprite shopSprite;
    public string merchantName;

    [Header("Equipment Cost")]
    [Space(10), SerializedDictionary("Equipment Rearity", "Cost")]
    public MerchantEquipCost EquipCost = new MerchantEquipCost() {
            {ItemRarity.Normal , 0},
            {ItemRarity.Rare , 0},
            {ItemRarity.Epic , 0},
            {ItemRarity.Unique , 0},
    };

    [Header("Key Cost")]
    [Space(10), SerializedDictionary("Key Rearity", "Cost")]
    public MerchantKeyCost KeyCost = new MerchantKeyCost() {
            {ItemRarity.Epic , 0},
            {ItemRarity.Unique , 0},
            {ItemRarity.Legendary , 0},
    };

    [Header("Item Cost Rate")]
    [Space(10), SerializedDictionary("Item Rearity", "Additional Cost Rate")]
    public MerchantItemCost ItemCost = new MerchantItemCost() {
            {ItemRarity.Normal , 0f},
            {ItemRarity.Rare , 0f},
    };

    [Header("Sell Cost Rate")]
    public float SellCostRate;
}

public enum MerchantGrade { Lower, Intermediate, Upper, Key };

[System.Serializable]
public class MerchantEquipCost : SerializedDictionary<ItemRarity, int> { }

[System.Serializable]
public class MerchantKeyCost : SerializedDictionary<ItemRarity, int> { }

[System.Serializable]
public class MerchantItemCost : SerializedDictionary<ItemRarity, float> { }

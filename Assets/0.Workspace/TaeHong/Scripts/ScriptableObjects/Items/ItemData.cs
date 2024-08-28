using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemLootSystem;

[CreateAssetMenu(menuName = "ItemData")]
public class ItemData : ScriptableObject
{
    public string Name;
    [Multiline]
    public string toolTip;
    public Sprite itemIcon;
    public ItemType itemType;

    public ItemRarity itemTier;
    public EquipmentType equipmentType;
}

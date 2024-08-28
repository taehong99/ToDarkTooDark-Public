using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemLootSystem;

public class ItemCollection : MonoBehaviour
{
    [SerializeField] EquipItemData[] allEquipItems;
    [SerializeField] BaseItemData[] allItems;

    private Dictionary<ItemRarity, List<EquipItemData>> equipItemsByRarity = new();
    private Dictionary<ItemRarity, List<BaseItemData>> itemsByRarity = new();

    private void Start()
    {
        // 해시테이블 초기화
        foreach(ItemRarity rarity in System.Enum.GetValues(typeof(ItemRarity)))
        {
            equipItemsByRarity.Add(rarity, new List<EquipItemData>());
            itemsByRarity.Add(rarity, new List<BaseItemData>());
        }
        
        // 아이템 추가
        foreach(EquipItemData equipment in allEquipItems)
        {
            equipItemsByRarity[equipment.rarity].Add(equipment);
        }
        foreach(BaseItemData item in allItems)
        {
            itemsByRarity[item.rarity].Add(item);
        }
    }

    public EquipItemData GetEquipmentWithRarity(ItemRarity rarity)
    {
        List<EquipItemData> equipItemPool = equipItemsByRarity[rarity];
        int random = Random.Range(0, equipItemPool.Count);
        EquipItemData equipment = equipItemPool[random];

        // 무기가 나왔는데 내 직업이랑 안맞으면 다시 돌리기
        while (equipment is WeaponItemData && ((WeaponItemData)equipment).weaponType != Manager.Game.MyWeaponType)
        {
            random = Random.Range(0, equipItemPool.Count);
            equipment = equipItemPool[random];
        }

        return equipment;
    }

    public BaseItemData GetItemWithRarity(ItemRarity rarity)
    {
        List<BaseItemData> itemPool = itemsByRarity[rarity];
        int random = Random.Range(0, itemPool.Count);
        BaseItemData item = itemPool[random];

        return item;
    }
}

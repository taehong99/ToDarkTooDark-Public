using UnityEngine;

namespace ItemLootSystem
{
    [CreateAssetMenu(menuName = "ItemData/WeaponData")]
    public class WeaponItemData : EquipItemData
    {
        public WeaponType weaponType;
    }
}
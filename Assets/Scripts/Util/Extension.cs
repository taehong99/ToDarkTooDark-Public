using UnityEngine;
using ItemLootSystem;

public static class Extension
{
    public static bool Contain(this LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }

    public static bool CanUseWeapon(this WeaponItemData weapon)
    {
        return weapon.weaponType == Manager.Game.MyWeaponType;
    }
}

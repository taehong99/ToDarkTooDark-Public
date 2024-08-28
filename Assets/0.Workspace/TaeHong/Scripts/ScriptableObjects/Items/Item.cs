using ItemLootSystem;
using UnityEngine;

namespace Tae.Inventory
{
    public abstract class Item : ScriptableObject
    {
        public BaseItemData itemData;

        public abstract void Use(GameObject user);
    }
}
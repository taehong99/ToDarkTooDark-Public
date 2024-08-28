using UnityEngine;
using ItemLootSystem;

namespace Tae.Inventory
{
    public abstract class ConsumableItem : Item
    {
        public override void Use(GameObject user)
        {
            UseItem(user);
            ReduceCount();
        }

        protected abstract void UseItem(GameObject user);

        protected void ReduceCount()
        {
            //InventoryManager.Instance.ReduceConsumableCount(this, 1);
        }
    }
}
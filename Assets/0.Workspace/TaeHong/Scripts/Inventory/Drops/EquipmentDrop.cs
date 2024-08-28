using ItemLootSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tae.Inventory
{
    public class EquipmentDrop : ItemDrop
    {
        [SerializeField] EquipItemData item;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            if(item != null)
            {
                spriteRenderer.sprite = item.icon;
            }
        }

        public override void OnCollect(Collector collector)
        {
            if (InventoryManager.Instance.IsEquipmentInventoryFull)
                return;

            collector.CollectItem(item);
            Destroy(gameObject);
        }

        public void InitDrop(EquipItemData item)
        {
            this.item = item;
            spriteRenderer.sprite = item.icon;
        }
    }
}
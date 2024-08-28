using System.Collections;
using System.Collections.Generic;
using Tae;
using Tae.Inventory;
using UnityEngine;

namespace ItemLootSystem
{
    public class TreasureBox : BaseBox
    {
        public TreasureRarity rarity;

        public override void OnInteract(Interactor interactor)
        {
            base.OnInteract(interactor);
            /*
            switch (rarity)
            {
                case TreasureRarity.D:
                case TreasureRarity.C:
                    base.OnInteract(interactor);
                    break;
                case TreasureRarity.B:
                    if (InventoryManager.Instance.BronzeKeyCount > 0)
                    {
                        InventoryManager.Instance.BronzeKeyCount--;
                        base.OnInteract(interactor);
                    }
                    break;
                case TreasureRarity.A:
                    if (InventoryManager.Instance.SilverKeyCount > 0)
                    {
                        InventoryManager.Instance.SilverKeyCount--;
                        base.OnInteract(interactor);
                    }
                    break;
                case TreasureRarity.S:
                    if(InventoryManager.Instance.GoldKeyCount > 0)
                    {
                        InventoryManager.Instance.GoldKeyCount--;
                        base.OnInteract(interactor);
                    }
                    break;
            }*/
        }
    }
}
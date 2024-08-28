using UnityEngine;

namespace Tae.Inventory
{
    public abstract class BasePotion : ConsumableItem
    {
        public int ID;
        public int dynamicID;
        public string Name => PotionManager.Instance.unknownPotionDic[dynamicID].potionName;
        public Sprite sprite => PotionManager.Instance.unknownPotionDic[dynamicID].sprite;
        public string Tooltip => PotionManager.Instance.unknownPotionDic[dynamicID].tooltip;
        public int weight;

        protected override void UseItem(GameObject user)
        {
            UsePotion(user);
            SetOpen();
        }

        protected abstract void UsePotion(GameObject user);

        protected void SetOpen()
        {
            PotionManager.Instance.SetOpen(ID);
        }
    }
}

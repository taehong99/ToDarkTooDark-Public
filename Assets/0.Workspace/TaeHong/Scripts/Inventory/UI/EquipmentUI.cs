using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ItemLootSystem;
using Photon.Pun;

namespace Tae.Inventory
{
    public class EquipmentUI : BaseUI
    {
        [Header("Player Portrait")]
        [SerializeField] Image portraitImage;
        [SerializeField] Sprite swordsmanSprite;
        [SerializeField] Sprite archerSprite;
        [SerializeField] Sprite priestSprite;
        [SerializeField] Sprite wizardSprite;

        [Header("Equipment Slots")]
        [SerializeField] EquipmentSlot weaponSlot;
        [SerializeField] EquipmentSlot armorSlot;
        [SerializeField] EquipmentSlot accessorySlot1;
        [SerializeField] EquipmentSlot accessorySlot2;

        [Space(10)]
        [Header("Stat Values")]
        [SerializeField] TextMeshProUGUI powerText;
        [SerializeField] TextMeshProUGUI armorText;
        [SerializeField] TextMeshProUGUI speedText;
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] TextMeshProUGUI regenText;
        [SerializeField] TextMeshProUGUI critRateText;
        [SerializeField] TextMeshProUGUI critDmgText;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            Manager.Game.MyPlayer.GetComponent<PlayerHealth>().maxhealthChangedEvent += UpdateHealthValue;
            Manager.Game.MyPlayer.GetComponent<PlayerStatsManager>().StatChangedEvent += UpdateStatValue;
            Manager.Game.MyPlayer.GetComponent<PlayerStatsManager>().CurrentStats();
            switch (PhotonNetwork.LocalPlayer.GetJob())
            {
                case PlayerJob.Swordsman:
                    portraitImage.sprite = swordsmanSprite;
                    break;
                case PlayerJob.Archer:
                    portraitImage.sprite = archerSprite;
                    break;
                case PlayerJob.Priest:
                    portraitImage.sprite = priestSprite;
                    break;
                case PlayerJob.Wizard:
                    portraitImage.sprite = wizardSprite;
                    break;
            }
        }

        #region Equipment Panel
        public void Equip(EquipItemData item)
        {
            EquipmentSlot slot = GetCorrectSlot(item.equipmentType);
            if (slot.isOccupied)
            {
                InventoryManager.Instance.UnequipItem(slot.Item);
            }
            slot.AddOrSwapItem(item);
        }

        public void UnEquip(EquipItemData item)
        {
            GetCorrectSlot(item).RemoveItem();
        }

        private EquipmentSlot GetCorrectSlot(EquipItemData item)
        {
            switch (item.equipmentType)
            {
                case EquipmentType.Weapon:
                    return weaponSlot;
                case EquipmentType.Armor:
                    return armorSlot;
                case EquipmentType.Accessory:
                    if (item == accessorySlot1.Item)
                        return accessorySlot1;
                    return accessorySlot2;
                default:
                    return null;
            }
        }

        private EquipmentSlot GetCorrectSlot(EquipmentType type)
        {
            switch (type)
            {
                case EquipmentType.Weapon:
                    return weaponSlot;
                case EquipmentType.Armor:
                    return armorSlot;
                case EquipmentType.Accessory:
                    if (!accessorySlot1.isOccupied)
                        return accessorySlot1;
                    else
                        return accessorySlot2;
                default:
                    return null;
            }
        }

        public bool CanEquip(EquipItemData item)
        {
            // Check if weapon type is mine
            bool isMyWeaponType = true;
            WeaponItemData weapon;
            if (item is WeaponItemData)
            {
                weapon = (WeaponItemData) item;
                isMyWeaponType = weapon.CanUseWeapon();
            }
                
            // Check if slot is empty
            bool isSlotEmpty = !GetCorrectSlot(item).isOccupied;

            return isMyWeaponType && isSlotEmpty;
        }
        #endregion

        #region Stats Panel
        private void UpdateStatValue(StatType statType, float value)
        {
            switch (statType)
            {
                case StatType.Power:
                    powerText.text = $"{value:0.00}";
                    break;
                case StatType.Armor:
                    armorText.text = $"{value:0.00}";
                    break;
                case StatType.Speed:
                    speedText.text = $"{value:0.00}";
                    break;
                case StatType.HpRegen:
                    regenText.text = $"{value : 0}/sec";
                    break;
                case StatType.Critical:
                    critRateText.text = $"{value * 100:0.00}%";
                    break;
                case StatType.CriticalMultiplier:
                    critDmgText.text = $"{value * 100:0.00}%";
                    break;
            }
        }

        private void UpdateHealthValue(int value)
        {
            healthText.text = value.ToString();
        }
        #endregion
    }
}
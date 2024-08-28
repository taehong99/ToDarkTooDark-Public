using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ItemLootSystem;
using static UnityEngine.Rendering.DebugUI;
using System.Linq;

namespace Tae.Inventory
{
    public class EquipmentTooltip : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI affixText;
        [SerializeField] TextMeshProUGUI typeText;

        [Space(10),SerializeField] Image iconImage;
        [SerializeField] TextMeshProUGUI rarityText;
        [SerializeField] TextMeshProUGUI upgradesText;

        [Space(10),SerializeField] TextMeshProUGUI powerText;
        [SerializeField] TextMeshProUGUI armorText;
        [SerializeField] TextMeshProUGUI speedText;
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] TextMeshProUGUI regenText;
        [SerializeField] TextMeshProUGUI critRateText;
        [SerializeField] TextMeshProUGUI critDmgText;

        [SerializeField] TextMeshProUGUI tooltipText;

        private RectTransform rectTransform;
        bool cached;

        // Content Rect 크기가 이상하게 받아와져서;; 하드코딩함...
        private float xOffset = 200;
        private float yOffset = 250;

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            if (!cached)
            {
                rectTransform = GetComponent<RectTransform>();
                cached = true;
            }
        }

        private void OnDisable()
        {
            powerText.text = "0";
            armorText.text = "0";
            speedText.text = "0";
            healthText.text = "0";
            regenText.text = "0";
            critRateText.text = "0";
            critDmgText.text = "0";
        }

        private void Update()
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 anchoredPosition;
            // Convert the screen point to local point in the RectTransform
            RectTransform parentRectTransform = rectTransform.parent as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, mousePosition, null, out anchoredPosition);

            // Deafault offset
            Vector2 offset = new Vector2(xOffset, -yOffset);

            // Get the parent canvas size
            Vector2 parentSize = parentRectTransform.rect.size;


            // Dynamic offset pos via mousePos 
            if(mousePosition.x > parentSize.x - xOffset * 2)
            {
                offset.x = -xOffset;
            }
            
            if(mousePosition.y < parentSize.y * 0.5)
            {
                offset.y = yOffset;
            }

            // Update the RectTransform's anchored position
            rectTransform.anchoredPosition = anchoredPosition+ offset;
        }

        public void Init(EquipItemData item)
        {
            nameText.text = item.GetItemNameWithSpaces();
            typeText.text = StringUtilities.GetEquipmentTypeName(item.equipmentType);
            iconImage.sprite = item.icon;
            rarityText.text = StringUtilities.GetRarityName(item.rarity);
            tooltipText.text = item.toolTip;

            UpdateEnhanceLevel(item.enhanceLevel);
            UpdateStatNumber(item.mainStat, item.EnhancedStat(item.enhanceLevel));
            UpdateStatNumber(item.subStat, item.subStatAmount);

            foreach(EquipAffix affix in item.affixes)
            {
                Debug.Log(affix.name);
                Debug.Log(affix.value);
                AddAffixNumber(affix);
            }
        }

        private void UpdateStatNumber(StatType type, float value)
        {
            switch (type)
            {
                case StatType.Power:
                    powerText.text = $"{value:0.00}";
                    break;
                case StatType.Armor:
                    armorText.text = $"{value:0.00}";
                    break;
                case StatType.MaxHp:
                    healthText.text = value.ToString();
                    break;
                case StatType.HpRegen:
                    regenText.text = value.ToString();
                    break;
                case StatType.Speed:
                    speedText.text = $"{value:0.00}";
                    break;
                case StatType.Critical:
                    critRateText.text = $"{value:0.00}";
                    break;
                case StatType.CriticalMultiplier:
                    critDmgText.text = $"{value:0.00}";
                    break;
            }
        }

        private void AddAffixNumber(EquipAffix affix)
        {
            Debug.Log("add affix");
            char sign = affix.isPositive ? '+' : '-';

            switch (affix.statType)
            {
                case StatType.Power:
                    powerText.text = $"{powerText.text} {sign} {affix.value}";
                    break;
                case StatType.Armor:
                    armorText.text = $"{armorText.text} {sign} {affix.value}";
                    break;
                case StatType.MaxHp:
                    healthText.text = $"{healthText.text} {sign} {affix.value}";
                    break;
                case StatType.HpRegen:
                    regenText.text = $"{regenText.text} {sign} {affix.value}";
                    break;
                case StatType.Speed:
                    speedText.text = $"{speedText.text} {sign} {affix.value}";
                    break;
                case StatType.Critical:
                    critRateText.text = $"{critRateText.text} {sign} {affix.value}";
                    break;
                case StatType.CriticalMultiplier:
                    critDmgText.text = $"{critDmgText.text} {sign} {affix.value}";
                    break;
            }
        }

        private void UpdateEnhanceLevel(int level) 
        {
            upgradesText.text = $"+{level} 강화";
        }
    }
}


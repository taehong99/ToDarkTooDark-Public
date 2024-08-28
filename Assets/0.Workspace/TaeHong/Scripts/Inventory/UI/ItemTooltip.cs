using ItemLootSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tae.Inventory
{
    public class ItemTooltip : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] Image iconImage;
        [SerializeField] TextMeshProUGUI rarityText;
        [SerializeField] Image effectDivider;
        [SerializeField] TextMeshProUGUI effectText;
        [SerializeField] TextMeshProUGUI tooltipText;

        private RectTransform rectTransform;
        bool cached;

        private float xOffset = 150;
        private float yOffset = 200;

        private void OnEnable()
        {
            if (!cached)
            {
                rectTransform = GetComponent<RectTransform>();
                cached = true;
            }
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
            if (mousePosition.x > parentSize.x - xOffset * 2)
            {
                offset.x = -xOffset;
            }

            if (mousePosition.y < parentSize.y * 0.5)
            {
                offset.y = yOffset;
            }

            // Update the RectTransform's anchored position
            rectTransform.anchoredPosition = anchoredPosition + offset;
        }

        public void Init(BaseItemData item)
        {
            // 포션이면
            if (item.isPotion)
            {
                UnknownPotionData data = PotionManager.Instance.PotionBottleData(item.ID);
                nameText.text = data.potionName;
                iconImage.sprite = data.sprite;
                tooltipText.text = data.tooltip;

                // 밀봉된 포션은 내용 숨기기 
                if (!PotionManager.Instance.GetOpen(item.ID))
                {
                    rarityText.text = "???";
                    effectDivider.gameObject.SetActive(false);
                    effectText.gameObject.SetActive(false);
                }
                else
                {
                    rarityText.text = item.rarity.ToString();
                    effectText.text = item.toolTip;
                    effectDivider.gameObject.SetActive(true);
                    effectText.gameObject.SetActive(true);
                }
            }
            // 기타 아이템이면 
            else if (item.itemType == ItemType.ETC) 
            {
                nameText.text = item.Name;
                iconImage.sprite = item.icon;
                rarityText.text = item.etcItemType;
                tooltipText.text = item.toolTip;

                effectDivider.gameObject.SetActive(false);
                effectText.gameObject.SetActive(false);
            }
            // 스크롤이면
            else
            {
                nameText.text = item.Name;
                iconImage.sprite = item.icon;
                tooltipText.text = item.toolTip;
                rarityText.text = item.rarity.ToString();
                effectDivider.gameObject.SetActive(false);
                effectText.gameObject.SetActive(false);
            }
        }
    }
}


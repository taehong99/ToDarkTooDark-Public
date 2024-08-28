using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ItemLootSystem;
using Tae.Inventory;

public class QuickslotUI : BaseUI
{
    private Image[] slotImages = new Image[5];
    private TextMeshProUGUI[] slotCounts = new TextMeshProUGUI[5];

    // Cache UI Elements
    private void Start()
    {
        for(int i = 1; i <= 4; i++)
        {
            slotImages[i] = GetUI<Image>($"Slot{i}Icon");
            slotCounts[i] = GetUI<TextMeshProUGUI>($"Slot{i}Count");
        }
    }

    public void InitSlot(BaseItemData itemData, int slotNumber)
    {
        if (itemData.isPotion)
        {
            UnknownPotionData unknownPotionData = PotionManager.Instance.PotionBottleData(itemData.ID);
            slotImages[slotNumber].sprite = unknownPotionData.sprite;
        }
        else
        {
            slotImages[slotNumber].sprite = itemData.icon;
        }

        slotCounts[slotNumber].text = InventoryManager.Instance.GetItemCount(itemData).ToString();

        slotImages[slotNumber].gameObject.SetActive(true);
        slotCounts[slotNumber].gameObject.SetActive(true);
    }

    public void UpdateSlot(BaseItemData itemData, int slotNumber)
    {
        if (itemData.isPotion)
        {
            UnknownPotionData unknownPotionData = PotionManager.Instance.PotionBottleData(itemData.ID);
            slotImages[slotNumber].sprite = unknownPotionData.sprite;
        }
        else
        {
            slotImages[slotNumber].sprite = itemData.icon;
        }

        slotCounts[slotNumber].text = InventoryManager.Instance.GetItemCount(itemData).ToString();
    }

    public void UpdateCount(int slotNumber, int count)
    {
        slotCounts[slotNumber].text = count.ToString();
    }

    public void ClearSlot(int slotNumber)
    {
        slotImages[slotNumber].gameObject.SetActive(false);
        slotCounts[slotNumber].gameObject.SetActive(false);
    }
}

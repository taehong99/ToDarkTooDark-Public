using ItemLootSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tae.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class InGameItem : MonoBehaviour
{
    [Header("Merchant Canvas")]
    public MerchantCanvas merchantCanvas;

    [Header("Select Button")]
    [SerializeField] Button selectButton;
    [SerializeField] GameObject selectBoundary;

    [Header("Info UI")]
    [SerializeField] Image itemImage;
    [SerializeField] Text itemName;
    public Text itemRemain;
    [SerializeField] Text itemCost;

    public BaseItemData itemData;
    public bool isSellItem;         // true 플레이어 판매 아이템 / false 상인 판매 아이템 (플레이어 구매 아이템)
    public int index;

    public int cost;

    private void Awake()
    {
        Deselected();
        selectButton.onClick.AddListener(Selected);
    }

    private void Start()
    {
        SetCost();
        if (itemData.isPotion)
        {
            itemImage.sprite = PotionManager.Instance.PotionBottleData(itemData.ID).sprite;
        }
        else
        {
            itemImage.sprite = itemData.icon;
        }
        
        itemName.text = itemData.Name;
        itemCost.text = cost.ToString();
    }

    private void SetCost()
    {
        MerchantData merData = merchantCanvas.curMurchant.merchantData;
        switch (itemData.itemType)
        {
            case ItemType.Equipment:
                if (merData.EquipCost.Keys.Contains(itemData.rarity))
                    cost = merData.EquipCost[itemData.rarity];
                else
                    cost = itemData.cost;
                break;
            case ItemType.Consumable:
                if (merData.ItemCost.Keys.Contains(itemData.rarity))
                    cost = itemData.cost + (int) (itemData.cost * merData.ItemCost[itemData.rarity]);
                else
                    cost = itemData.cost;
                break;
            case ItemType.Key:
                switch (itemData.rarity)
                {
                    case ItemRarity.Epic:
                        cost = merData.KeyCost[ItemRarity.Unique];
                        break;
                    case ItemRarity.Unique:
                        cost = merData.KeyCost[ItemRarity.Unique];
                        break;
                    case ItemRarity.Legendary:
                        cost = merData.KeyCost[ItemRarity.Legendary];
                        break;
                }
                break;
            case ItemType.ETC:
                cost = itemData.cost;
                break;
        }
    }

    private void Selected()
    {
        if(isSellItem)
            merchantCanvas.curSellItem = this;
        else
            merchantCanvas.curItem = this;
        merchantCanvas.ChangeSelect(isSellItem);
        selectBoundary.SetActive(true);
    }

    public void Deselected()
    {
        selectBoundary.SetActive(false);
    }
}

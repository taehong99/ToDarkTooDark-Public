using Firebase.Extensions;
using ItemLootSystem;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tae.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class HowManyChoosePanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] MerchantCanvas merchantPanel;

    [Header("UI")]
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [SerializeField] Slider numSlider;
    [SerializeField] Text sliderNumText;
    [SerializeField] Text costText;
    [SerializeField] ConfirmPanel2 confirmPanel;
    public InGameItem curItem;
    public int itemNum;

    private void Start()
    {
        noButton.onClick.AddListener(Close);
        numSlider.onValueChanged.AddListener(OnValueChanged);
    }
    private void OnEnable()
    {
        yesButton.onClick.AddListener(Check);
        initValue();
    }

    private void OnDisable()
    {
        curItem = null;
        yesButton.onClick.RemoveAllListeners();
    }

    private void initValue()
    {
        numSlider.maxValue = itemNum;
        numSlider.value = 1;
        OnValueChanged(1);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnValueChanged(float num)
    {
        costText.text = (curItem.cost * (int)num).ToString();
        itemNum = (int) numSlider.value;
        sliderNumText.text = itemNum.ToString();
        if (InventoryManager.Instance.GoldCount != 0 && InventoryManager.Instance.GoldCount >= (int) num * curItem.cost)
        {
            yesButton.interactable = true;
            costText.color = Color.white;
        }
        else
        {
            yesButton.interactable = false;
            costText.color = Color.red;
        }
    }

    private void Check()
    {
        // 만약 돈이 모자르다면 실행 안도도록 조건문 붙이기
        if (curItem.isSellItem)
            confirmPanel.confirmButton.onClick.AddListener(Sell);
        else
            confirmPanel.confirmButton.onClick.AddListener(Buy);
        confirmPanel.confirmButton.onClick.AddListener(Close);
        confirmPanel.gameObject.SetActive(true);
    }

    public void Buy()
    {
        if (curItem == null)
        {
            Debug.Log("No Item Selected");
            return;
        }
        Debug.Log($"{curItem.itemData.Name} / COST {curItem.itemData.cost * itemNum}");
        // merchantPanel.curMurchant.BuyItem(curItem.index, (int) numSlider.value);
        MurchantBuy();
        InventoryManager.Instance.BuyItem(curItem.itemData, curItem.cost, itemNum);
        /*
        MerchantData merData = merchantPanel.curMurchant.merchantData;
        switch (curItem.itemData.itemType)
        {
            case ItemType.Equipment:
                if (merData.EquipCost.Keys.Contains(curItem.itemData.rarity))
                    InventoryManager.Instance.BuyItem(curItem.itemData, merData.EquipCost[curItem.itemData.rarity], itemNum);
                else
                    InventoryManager.Instance.BuyItem(curItem.itemData, curItem.itemData.cost, itemNum);
                break;
            case ItemType.Consumable:
                if(merData.ItemCost.Keys.Contains(curItem.itemData.rarity))
                    InventoryManager.Instance.BuyItem(curItem.itemData, curItem.itemData.cost + (int)(curItem.itemData.cost * merData.ItemCost[curItem.itemData.rarity]), itemNum);
                else
                    InventoryManager.Instance.BuyItem(curItem.itemData, curItem.itemData.cost, itemNum);
                break;
            case ItemType.Key:
                switch(curItem.itemData.rarity)
                {
                    case ItemRarity.Epic:
                        InventoryManager.Instance.BuyItem(curItem.itemData, merData.KeyCost[ItemRarity.Epic], itemNum);
                        break;
                    case ItemRarity.Unique:
                        InventoryManager.Instance.BuyItem(curItem.itemData, merData.KeyCost[ItemRarity.Unique], itemNum);
                        break;
                    case ItemRarity.Legendary:
                        InventoryManager.Instance.BuyItem(curItem.itemData, merData.KeyCost[ItemRarity.Legendary], itemNum);
                        break;
                }
                break;
            case ItemType.ETC:
                InventoryManager.Instance.BuyItem(curItem.itemData, curItem.itemData.cost, itemNum);
                break;
        }
        */
        merchantPanel.UpdateList();
    }

    public void Sell()
    {
        if (curItem == null)
        {
            Debug.Log("No Item Selected");
            return;
        }
        Debug.Log($"{curItem.itemData.Name} / COST {curItem.itemData.cost * itemNum}");
        InventoryManager.Instance.SellItem(curItem.itemData, curItem.cost, itemNum);
        /*
        // merchantPanel.curMurchant.SellItem(curItem.index, (int) numSlider.value);
        MerchantData merData = merchantPanel.curMurchant.merchantData;
        switch (curItem.itemData.itemType)
        {
            case ItemType.Equipment:
                if (merData.EquipCost.Keys.Contains(curItem.itemData.rarity))
                    InventoryManager.Instance.SellItem(curItem.itemData, merData.EquipCost[curItem.itemData.rarity], itemNum);
                else
                    InventoryManager.Instance.SellItem(curItem.itemData, curItem.itemData.cost, itemNum);
                break;
            case ItemType.Consumable:
                if (merData.ItemCost.Keys.Contains(curItem.itemData.rarity))
                    InventoryManager.Instance.SellItem(curItem.itemData, curItem.itemData.cost + (int) (curItem.itemData.cost * merData.ItemCost[curItem.itemData.rarity]), itemNum);
                else
                    InventoryManager.Instance.SellItem(curItem.itemData, curItem.itemData.cost, itemNum);
                break;
            case ItemType.Key:
                switch (curItem.itemData.rarity)
                {
                    case ItemRarity.Epic:
                        InventoryManager.Instance.SellItem(curItem.itemData, merData.KeyCost[ItemRarity.Epic], itemNum);
                        break;
                    case ItemRarity.Unique:
                        InventoryManager.Instance.SellItem(curItem.itemData, merData.KeyCost[ItemRarity.Unique], itemNum);
                        break;
                    case ItemRarity.Legendary:
                        InventoryManager.Instance.SellItem(curItem.itemData, merData.KeyCost[ItemRarity.Legendary], itemNum);
                        break;
                }
                break;
            case ItemType.ETC:
                InventoryManager.Instance.SellItem(curItem.itemData, curItem.itemData.cost, itemNum);
                break;
        }
        */
        merchantPanel.UpdateList();
    }

    private void MurchantBuy()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                merchantPanel.curMurchant.photonView.RPC("BuyMerchantItem", RpcTarget.MasterClient, curItem.index, (int) numSlider.value);
                return;
            }
        }
        else
        {
            Debug.Log("etnered');");
            merchantPanel.curMurchant.BuyMerchantItem(curItem.index, (int) numSlider.value);
        }
    }
}

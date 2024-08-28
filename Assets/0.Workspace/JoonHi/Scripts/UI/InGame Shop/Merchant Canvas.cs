using ItemLootSystem;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Tae.Inventory;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MerchantCanvas : MonoBehaviour
{
    public static MerchantCanvas Instance { get; private set; }

    [Header("Image")]
    [SerializeField] UnityEngine.UI.Image merchantImage;
    public UnityEngine.UI.Image playerImage;

    [Header("Buttons")]
    [SerializeField] UnityEngine.UI.Button buyButton;
    [SerializeField] UnityEngine.UI.Button sellButton;
    [SerializeField] UnityEngine.UI.Button exitButton;
    [SerializeField] UnityEngine.UI.Button merchantExitButton;
    [SerializeField] UnityEngine.UI.Button playerExitButton;

    [Header("Player Wallet")]
    [SerializeField] Text coinText;
    [SerializeField] Text goldKeyText;
    [SerializeField] Text silverKeyText;
    [SerializeField] Text brownzeKeyText;

    [Header("Texts")]
    [SerializeField] Text merchantName;
    [SerializeField] Text playerName;

    [Header("ScrollView")]
    [SerializeField] GameObject merchantScroll;
    [SerializeField] GameObject PlayerScroll;

    [Header("Prefab")]
    [SerializeField] GameObject inGameItemPrefab;

    [Header("Panels")]
    [SerializeField] HowManyChoosePanel howManyBuy;
    [SerializeField] HowManyChoosePanel howManySell;

    public Merchant curMurchant;
    public InGameItem curItem;
    public InGameItem curSellItem;

    List<InGameItem> merchantItemList = new List<InGameItem>();
    List<InGameItem> playerItemList = new List<InGameItem>();
    public List<MerchantData> merchants = new List<MerchantData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        if (curMurchant == null)
        {
            gameObject.SetActive(false);
            return;
        }

        merchantImage.sprite = curMurchant.merchantData.merchantSprite;
        merchantImage.SetNativeSize();

        merchantName.text = curMurchant.merchantData.merchantName;
        playerName.text = $"{PhotonNetwork.LocalPlayer.NickName} 인벤토리";

        exitButton.onClick.AddListener(Close);
        merchantExitButton.onClick.AddListener(Close);
        playerExitButton.onClick.AddListener(Close);

        buyButton.onClick.AddListener(HowMuchBuy);
        sellButton.onClick.AddListener(HowMuchSell);

        UpdateList();
    }

    private void OnDisable()
    {
        curMurchant = null;
        ClearList();
    }

    public void UpdateList()
    {
        WalletCheck();
        ClearList();
        int index = 0;
        foreach (BaseItemData item in curMurchant.merchantItem)
        {
            if (curMurchant.isSold[index] == false)
            {
                InGameItem ItemPrefab = Instantiate(inGameItemPrefab, merchantScroll.transform).GetComponent<InGameItem>();
                ItemPrefab.merchantCanvas = this;
                ItemPrefab.itemData = item;
                ItemPrefab.itemRemain.text = curMurchant.itemNum[index].ToString();
                ItemPrefab.isSellItem = false;
                ItemPrefab.index = index;
                merchantItemList.Add(ItemPrefab);
            }
            index++;
        }

        foreach (BaseItemData item in InventoryManager.Instance.ItemInventory)
        {
            if (item != null)
            {
                InGameItem ItemPrefab = Instantiate(inGameItemPrefab, PlayerScroll.transform).GetComponent<InGameItem>();
                ItemPrefab.merchantCanvas = this;
                ItemPrefab.itemData = item;
                ItemPrefab.itemRemain.text = InventoryManager.Instance.ItemCounts[item].ToString();
                ItemPrefab.isSellItem = true;
                playerItemList.Add(ItemPrefab);
            }
        }
    }

    private void ClearList()
    {
        foreach (InGameItem item in merchantItemList)
        {
            Destroy(item.gameObject);
        }
        merchantItemList.Clear();

        foreach (InGameItem item in playerItemList)
        {
            Destroy(item.gameObject);
        }
        playerItemList.Clear();
    }

    private void HowMuchBuy()
    {
        if (curItem != null)
        {
            howManyBuy.curItem = this.curItem;
            int num;
            int.TryParse(curItem.itemRemain.text, out num);
            howManyBuy.itemNum = num;
            howManyBuy.gameObject.SetActive(true);
        }
    }

    private void HowMuchSell()
    {
        if (curSellItem != null)
        {
            howManySell.curItem = this.curSellItem;
            int num;
            int.TryParse(curSellItem.itemRemain.text, out num);
            howManySell.itemNum = num;
            howManySell.gameObject.SetActive(true);
        }
    }

    public void ChangeSelect(bool isSellItem)
    {
        if(isSellItem)
            foreach (InGameItem item in playerItemList)
                item.Deselected();
        else
            foreach (InGameItem item in merchantItemList)
                item.Deselected();
    }

    public void WalletCheck()
    {
        brownzeKeyText.text = InventoryManager.Instance.BronzeKeyCount.ToString();
        silverKeyText.text = InventoryManager.Instance.SilverKeyCount.ToString();
        goldKeyText.text = InventoryManager.Instance.GoldKeyCount.ToString();
        coinText.text = InventoryManager.Instance.GoldCount.ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}

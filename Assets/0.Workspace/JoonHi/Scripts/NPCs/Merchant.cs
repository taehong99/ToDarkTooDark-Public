using ItemLootSystem;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using Tae;
using UnityEditor;
using UnityEngine;

public class Merchant : MonoBehaviourPun, IInteractable, IPunInstantiateMagicCallback
{
    [Header("Merchant Data")]
    public MerchantData merchantData;

    [Header("Image")]
    [SerializeField] SpriteRenderer merchantSprite;
    [SerializeField] SpriteRenderer shopSprite;
    [SerializeField] SpriteRenderer minimapIcon;

    [Header("Canvas")]
    [SerializeField] MerchantCanvas merchantCanvas;

    [Header("Animator")]
    [SerializeField] Animator animator;

    // Path
    // const string RARITYDATAPATH = "Test/RarityData";
    const string RARITYDATAPATH = "Assets/0.Workspace/JunHyoung/SO/DropTable&Pool/RarityData.asset";

    // 레덤값 시드
    private int seed;
    private System.Random randomInstance;

    [SerializeField] RarityData rarityData;

    // 상인 판매 아이템 저장 리스트
    public List<BaseItemData> merchantItem = new List<BaseItemData>();
    public List<int> itemNum = new List<int>();
    public List<bool> isSold = new List<bool>();

    public void Awake()
    {
        InitSeed();
        randomInstance = new System.Random(seed);
        // rarityData = Resources.Load<RarityData>(RARITYDATAPATH);
        // rarityData = AssetDatabase.LoadAssetAtPath<RarityData>(RARITYDATAPATH);
    }

    public void Start()
    {
       if(!PhotonNetwork.IsConnected)
        {
            SettingFromData();
        }
    }

    void SettingFromData()
    {
        merchantCanvas = MerchantCanvas.Instance;
        SetSprite();
        SetAnimation();

        if (merchantData.Grade != MerchantGrade.Key)
        {
            int randfactor;
            List<ItemRarity> rarity = merchantData.EquipCost.Keys.ToList();
            foreach (ItemRarity rar in rarity)
            {
                switch (rar)
                {
                    case ItemRarity.Normal:
                        randfactor = randomInstance.Next(0, rarityData.NormalEquiptment.Count);
                        merchantItem.Add(rarityData.NormalEquiptment[randfactor]);
                        itemNum.Add(1);
                        isSold.Add(false);
                        break;
                    case ItemRarity.Rare:
                        randfactor = randomInstance.Next(0, rarityData.RareEquiptment.Count);
                        merchantItem.Add(rarityData.RareEquiptment[randfactor]);
                        itemNum.Add(1);
                        isSold.Add(false);
                        break;
                    case ItemRarity.Epic:
                        randfactor = randomInstance.Next(0, rarityData.EpicEquiptment.Count);
                        merchantItem.Add(rarityData.EpicEquiptment[randfactor]);
                        itemNum.Add(1);
                        isSold.Add(false);
                        break;
                    case ItemRarity.Unique:
                        randfactor = randomInstance.Next(0, rarityData.UniqueEquiptment.Count);
                        merchantItem.Add(rarityData.UniqueEquiptment[randfactor]);
                        itemNum.Add(1);
                        isSold.Add(false);
                        break;
                    case ItemRarity.Legendary:
                        randfactor = randomInstance.Next(0, rarityData.LegendaryEquiptment.Count);
                        merchantItem.Add(rarityData.LegendaryEquiptment[randfactor]);
                        itemNum.Add(1);
                        isSold.Add(false);
                        break;
                    case ItemRarity.Artifact:
                        randfactor = randomInstance.Next(0, rarityData.ArtifactEquiptment.Count);
                        merchantItem.Add(rarityData.ArtifactEquiptment[randfactor]);
                        itemNum.Add(1);
                        isSold.Add(false);
                        break;
                    default:
                        merchantItem.Add(null);
                        itemNum.Add(0);
                        isSold.Add(true);
                        break;
                }
            }

            #region 표션형, 스크롤 아이템 추가
            for (int i = 0; i < 5; i++)
            {
                randfactor = randomInstance.Next(0, rarityData.NormalCommsumable.Count);
                if (!isSame(rarityData.NormalCommsumable[randfactor]))
                {
                    merchantItem.Add(rarityData.NormalCommsumable[randfactor]);
                    itemNum.Add(1);
                    isSold.Add(false);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                randfactor = randomInstance.Next(0, rarityData.RareCommsumable.Count);
                if (!isSame(rarityData.RareCommsumable[randfactor]))
                {
                    merchantItem.Add(rarityData.RareCommsumable[randfactor]);
                    itemNum.Add(1);
                    isSold.Add(false);
                }
            }
            for (int i = 0; i < 2; i++)
            {
                randfactor = randomInstance.Next(0, rarityData.EpicCommsumable.Count);
                if (!isSame(rarityData.EpicCommsumable[randfactor]))
                {
                    merchantItem.Add(rarityData.EpicCommsumable[randfactor]);
                    itemNum.Add(1);
                    isSold.Add(false);
                }
            }
            randfactor = randomInstance.Next(0, rarityData.UniqueCommsumable.Count);
            if (!isSame(rarityData.UniqueCommsumable[randfactor]))
            {
                merchantItem.Add(rarityData.UniqueCommsumable[randfactor]);
                itemNum.Add(1);
                isSold.Add(false);
            }
            #endregion
        }
        else
        {
            // Key Merchant
            int randfactor;
            List<ItemRarity> rarity = merchantData.KeyCost.Keys.ToList();
            foreach (ItemRarity rar in rarity)
            {
                switch (rar)
                {
                    case ItemRarity.Epic:
                        randfactor = randomInstance.Next(0, rarityData.EpicKey.Count);
                        merchantItem.Add(rarityData.EpicKey[randfactor]);
                        itemNum.Add(1);
                        isSold.Add(false);
                        break;
                    case ItemRarity.Unique:
                        randfactor = randomInstance.Next(0, rarityData.UniqueKey.Count);
                        merchantItem.Add(rarityData.UniqueKey[randfactor]);
                        itemNum.Add(1);
                        isSold.Add(false);
                        break;
                    case ItemRarity.Legendary:
                        randfactor = randomInstance.Next(0, rarityData.LegendaryKey.Count);
                        merchantItem.Add(rarityData.LegendaryKey[randfactor]);
                        itemNum.Add(1);
                        isSold.Add(false);
                        break;
                    default:
                        merchantItem.Add(null);
                        itemNum.Add(0);
                        isSold.Add(true);
                        break;
                }
            }
        }
    }

    private bool isSame(BaseItemData item)
    {
        int index = 0;
        foreach(BaseItemData Item in merchantItem)
        {
            if(Item == item)
            {
                itemNum[index]++;
                return true;
            }
            index++;
        }
        return false;
    }

    [PunRPC]
    public void BuyMerchantItem(int index, int num)
    {
        Debug.Log("BuyMerchantItem called");
        BuyItem(index, num);
    }

    public void BuyItem(int index, int num)
    {
        if (isSold[index] != true)
        {
            itemNum[index] -= num;
            if (itemNum[index] == 0)
                isSold[index] = true;
            Debug.Log($"Buy {merchantItem[index]}x{num}");
        }
        else
        {
            Debug.Log("This item is Sold");
        }
    }

    public void SellItem()
    {

    }

    private void InitSeed()
    {
        // 시드 값 랜덤 생성 및 Photon 커스텀 프로퍼티로 설정은 로비에서 방 만들어졌을 때 미리 정해주자.
        // 굳이 인-게임에서는 할 필요 없을것같음. 대기시간 길어지기만 할듯
        // 여기서 Photon 커스텀 프로퍼티에서 받아오기만
        // Map Generator 안태홍 추가 참조
        if (PhotonNetwork.IsConnected)
            seed = PhotonNetwork.CurrentRoom.GetSeed();
        else
            seed = Random.Range(0, 10000);

        Debug.Log($"Init Seed : {seed}");
    }

    private void SetSprite()
    {
        merchantSprite.sprite = merchantData.merchantSprite;
        shopSprite.sprite = merchantData.shopSprite;
        minimapIcon.sprite = merchantData.shopSprite;
    }

    private void SetAnimation()
    {
        switch(merchantData.Grade)
        {
            case MerchantGrade.Lower:
                animator.SetLayerWeight(1, 1);
                break;
            case MerchantGrade.Intermediate:
                animator.SetLayerWeight(2, 1);
                break;
            case MerchantGrade.Upper:
                animator.SetLayerWeight(3, 1);
                break;
            case MerchantGrade.Key:
                animator.SetLayerWeight(4, 1);
                break;
        }
        
    }

    public void OnInteract(Interactor interactor)
    {
        merchantCanvas.curMurchant = this;
        merchantCanvas.playerImage.sprite = interactor.gameObject.transform.root.GetComponent<SpriteRenderer>().sprite;
        merchantCanvas.playerImage.SetNativeSize();
        merchantCanvas.gameObject.SetActive(true);
    }

    public void ShowUI()
    {
    }

    public void HideUI()
    {
    }

    [Space(2)]
    [Header("Debug")]
    [SerializeField] int buyIndex;

    [ContextMenu("Debug BuyItem")]
    public void DebugBuyItem()
    {
        List<ItemRarity> rarity = merchantData.EquipCost.Keys.ToList();
        BuyItem(buyIndex, 1);
    }

    /**********************************************
     *             Photon Interface
     ***********************************************/

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {

        // 싱글턴 객체인 MerchantCanvas에 상점 데이터들 참조시켜 놓고, ->
        // Instantiate할때 상점 등급만 받아서, -> 해당 등급 상점 데이터 set
        object[] data = info.photonView.InstantiationData;
        merchantData = MerchantCanvas.Instance.merchants[(int)data[0]];
        SettingFromData();
    }
}

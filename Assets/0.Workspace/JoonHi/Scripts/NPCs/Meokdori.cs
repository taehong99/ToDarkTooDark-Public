using ItemLootSystem;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using Tae;
using Tae.Inventory;
using UnityEngine;

public class Meokdori : MonoBehaviourPun, IInteractable
{
    [Header("Meokdori Data")]
    public MeokdoriData meokdoriData;

    [Header("Speech Bubble")]
    [SerializeField] TextMesh costText;

    [Header("Animator")]
    [SerializeField] Animator animator;

    [Header("Sound")]
    [SerializeField] SpriteRenderer soundSprite;

    [Header("Spec")]
    [SerializeField] float dropRange;
    [Range(0, 5)]
    [SerializeField] float dropSpeed;

    // Path
    /*
    const string RARITYDATAPATH = "Test/RarityData";
    const string EQUIPRARITYDATAPATH = "Test/EquipRarityData";

    const string RARITYDATAPATH = "Assets/0.Workspace/JunHyoung/SO/DropTable&Pool/RarityData.asset";
    // Equip Rarity Data Creating Spot Path
    const string EQUIPRARITYDATAPATH = "Assets/0.Workspace/JunHyoung/SO/DropTable&Pool/EquipRarityData.asset";
    */

    // 레덤값 시드
    private int seed;
    private System.Random randomInstance;

    [SerializeField] RarityData rarityData;
    [SerializeField] EquipRarityData equipRarityData;

    private List<BaseItemData> ItemList = new List<BaseItemData>(); // 뽑기 리스트 순서
    private int curIndex;                                           // 현제 위치
    private int prizeNum;                                           // 현제 남은 당첨 갯수

    // WeightedRandom<ItemRarity> randomRarity;
    private WeightedRandom<ItemType> randomItemType;
    private WeightedRandom<EquipmentType> randomEquipmentType;

    public bool isEating;

    public void Awake()
    {
        isEating = false;
        SetPrice();
        InitSeed();
        SetAnimation();
        randomInstance = new System.Random(seed);
        // rarityData = Resources.Load<RarityData>(RARITYDATAPATH);
        // equipRarityData = Resources.Load<EquipRarityData>(EQUIPRARITYDATAPATH);
        // rarityData = AssetDatabase.LoadAssetAtPath<RarityData>(RARITYDATAPATH);
        // equipRarityData = AssetDatabase.LoadAssetAtPath<EquipRarityData>(EQUIPRARITYDATAPATH);

        curIndex = 0;
        prizeNum = 3;

        int[] prizePlace = new int[3];
        int randfactor;

        // 렌덤값 입력
        // randomRarity = new WeightedRandom<ItemRarity>(seed);
        randomItemType = new WeightedRandom<ItemType>(seed);
        randomEquipmentType = new WeightedRandom<EquipmentType>(seed);

        randomItemType.Add(ItemType.Equipment, meokdoriData.ItemTypeWeight[ItemType.Equipment]);
        if (meokdoriData.ItemTypeWeight[ItemType.Consumable] != 0)
            randomItemType.Add(ItemType.Consumable, meokdoriData.ItemTypeWeight[ItemType.Consumable]);
        if (meokdoriData.ItemTypeWeight[ItemType.Key] != 0)
            randomItemType.Add(ItemType.Key, meokdoriData.ItemTypeWeight[ItemType.Key]);

        randomEquipmentType.Add(EquipmentType.Weapon, meokdoriData.EquipTypeWeight[EquipmentType.Weapon]);
        randomEquipmentType.Add(EquipmentType.Armor, meokdoriData.EquipTypeWeight[EquipmentType.Armor]);
        randomEquipmentType.Add(EquipmentType.Accessory, meokdoriData.EquipTypeWeight[EquipmentType.Accessory]);

        for (int i = 0; i < 3; i++)
        {
            randfactor = randomInstance.Next(0, 9);
            if (prizePlace.Contains(randfactor))
            {
                i--;
                continue;
            }
            prizePlace[i] = randfactor;
            Debug.Log(randfactor);
        }

        Debug.Log($"먹돌이 상품 위치: {string.Join(",", prizePlace)}");

        for (int i = 0; i < 10; i++)
        {
            if (prizePlace.Contains(i))
                ItemList.Add(randomItem(i));
            else
                ItemList.Add(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PhotonView photonView))
        {
            if (photonView.IsMine)
            {
                animator.SetBool("PlayerNear", true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PhotonView photonView))
        {
            if (photonView.IsMine)
            {
                if (InventoryManager.Instance.GoldCount > meokdoriData.Costs[curIndex])
                    animator.SetBool("HaveMoney", true);
                else
                    animator.SetBool("HaveMoney", false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PhotonView photonView))
        {
            if (photonView.IsMine)
            {
                animator.SetBool("PlayerNear", false);
            }
        }
    }

    private BaseItemData randomItem(int index)
    {
        BaseItemData result = null;

        ItemRarity randRarity;
        ItemType randType;
        EquipmentType randEquip;


        WeightedRandom<ItemRarity> randomRarity = new WeightedRandom<ItemRarity>(seed);
        if (meokdoriData.RearityWaight[ItemRarity.Normal][index] != 0)
            randomRarity.Add(ItemRarity.Normal, meokdoriData.RearityWaight[ItemRarity.Normal][index]);
        if (meokdoriData.RearityWaight[ItemRarity.Rare][index] != 0)
            randomRarity.Add(ItemRarity.Rare, meokdoriData.RearityWaight[ItemRarity.Rare][index]);
        if (meokdoriData.RearityWaight[ItemRarity.Epic][index] != 0)
            randomRarity.Add(ItemRarity.Epic, meokdoriData.RearityWaight[ItemRarity.Epic][index]);
        if (meokdoriData.RearityWaight[ItemRarity.Unique][index] != 0)
            randomRarity.Add(ItemRarity.Unique, meokdoriData.RearityWaight[ItemRarity.Unique][index]);
        if (meokdoriData.RearityWaight[ItemRarity.Legendary][index] != 0)
            randomRarity.Add(ItemRarity.Legendary, meokdoriData.RearityWaight[ItemRarity.Legendary][index]);
        if (meokdoriData.RearityWaight[ItemRarity.Artifact][index] != 0)
            randomRarity.Add(ItemRarity.Artifact, meokdoriData.RearityWaight[ItemRarity.Artifact][index]);

        for (int i = 0; i < index; i++)
            randomRarity.GetRandomPick();
        randRarity = randomRarity.GetRandomPick();
        Debug.Log($"먹돌이 {index}번째 아이템 희귀도: {randRarity}");
        randType = randomItemType.GetRandomPick();
        Debug.Log($"먹돌이 {index}번째 아이템 종류: {randType}");
        randEquip = randomEquipmentType.GetRandomPick();
        Debug.Log($"먹돌이 {index}번째 장비 종류: {randEquip}");

        int randfactor;
        switch (randType)
        {
            case ItemType.Equipment:
                switch (randRarity)
                {
                    case ItemRarity.Normal:
                        switch (randEquip)
                        {
                            case EquipmentType.Weapon:
                                randfactor = randomInstance.Next(0, equipRarityData.NormalWeapon.Count);
                                result = equipRarityData.NormalWeapon[randfactor];
                                break;
                            case EquipmentType.Armor:
                                randfactor = randomInstance.Next(0, equipRarityData.NormalArmor.Count);
                                result = equipRarityData.NormalArmor[randfactor];
                                break;
                            case EquipmentType.Accessory:
                                randfactor = randomInstance.Next(0, equipRarityData.NormalAccessory.Count);
                                result = equipRarityData.NormalAccessory[randfactor];
                                break;
                        }
                        break;
                    case ItemRarity.Rare:
                        switch (randEquip)
                        {
                            case EquipmentType.Weapon:
                                randfactor = randomInstance.Next(0, equipRarityData.RareWeapon.Count);
                                result = equipRarityData.RareWeapon[randfactor];
                                break;
                            case EquipmentType.Armor:
                                randfactor = randomInstance.Next(0, equipRarityData.RareArmor.Count);
                                result = equipRarityData.RareArmor[randfactor];
                                break;
                            case EquipmentType.Accessory:
                                randfactor = randomInstance.Next(0, equipRarityData.RareAccessory.Count);
                                result = equipRarityData.RareAccessory[randfactor];
                                break;
                        }
                        break;
                    case ItemRarity.Epic:
                        switch (randEquip)
                        {
                            case EquipmentType.Weapon:
                                randfactor = randomInstance.Next(0, equipRarityData.EpicWeapon.Count);
                                result = equipRarityData.EpicWeapon[randfactor];
                                break;
                            case EquipmentType.Armor:
                                randfactor = randomInstance.Next(0, equipRarityData.EpicArmor.Count);
                                result = equipRarityData.EpicArmor[randfactor];
                                break;
                            case EquipmentType.Accessory:
                                randfactor = randomInstance.Next(0, equipRarityData.EpicAccessory.Count);
                                result = equipRarityData.EpicAccessory[randfactor];
                                break;
                        }
                        break;
                    case ItemRarity.Unique:
                        switch (randEquip)
                        {
                            case EquipmentType.Weapon:
                                randfactor = randomInstance.Next(0, equipRarityData.UniqueWeapon.Count);
                                result = equipRarityData.UniqueWeapon[randfactor];
                                break;
                            case EquipmentType.Armor:
                                randfactor = randomInstance.Next(0, equipRarityData.UniqueArmor.Count);
                                result = equipRarityData.UniqueArmor[randfactor];
                                break;
                            case EquipmentType.Accessory:
                                randfactor = randomInstance.Next(0, equipRarityData.UniqueAccessory.Count);
                                result = equipRarityData.UniqueAccessory[randfactor];
                                break;
                        }
                        break;
                    case ItemRarity.Legendary:
                        switch (randEquip)
                        {
                            case EquipmentType.Weapon:
                                randfactor = randomInstance.Next(0, equipRarityData.LegendaryWeapon.Count);
                                result = equipRarityData.LegendaryWeapon[randfactor];
                                break;
                            case EquipmentType.Armor:
                                randfactor = randomInstance.Next(0, equipRarityData.LegendaryArmor.Count);
                                result = equipRarityData.LegendaryArmor[randfactor];
                                break;
                            case EquipmentType.Accessory:
                                randfactor = randomInstance.Next(0, equipRarityData.LegendaryAccessory.Count);
                                result = equipRarityData.LegendaryAccessory[randfactor];
                                break;
                        }
                        break;
                    case ItemRarity.Artifact:
                        switch (randEquip)
                        {
                            case EquipmentType.Weapon:
                                randfactor = randomInstance.Next(0, equipRarityData.ArtifactWeapon.Count);
                                result = equipRarityData.ArtifactWeapon[randfactor];
                                break;
                            //case EquipmentType.Armor:
                            //    randfactor = randomInstance.Next(0, equipRarityData.ArtifactArmor.Count);
                            //    result = equipRarityData.ArtifactArmor[randfactor];
                            //    break;
                            //case EquipmentType.Accessory:
                            //    randfactor = randomInstance.Next(0, equipRarityData.ArtifactAccessory.Count);
                            //    result = equipRarityData.ArtifactAccessory[randfactor];
                            //    break;
                        }
                        break;
                }
                randfactor = randomInstance.Next(0, rarityData.NormalEquiptment.Count);
                result = rarityData.NormalEquiptment[randfactor];
                break;
            case ItemType.Consumable:
                switch (randRarity)
                {
                    case ItemRarity.Normal:
                        randfactor = randomInstance.Next(0, rarityData.NormalCommsumable.Count);
                        result = rarityData.NormalCommsumable[randfactor];
                        break;
                    case ItemRarity.Rare:
                        randfactor = randomInstance.Next(0, rarityData.RareCommsumable.Count);
                        result = rarityData.RareCommsumable[randfactor];
                        break;
                    case ItemRarity.Epic:
                        randfactor = randomInstance.Next(0, rarityData.EpicCommsumable.Count);
                        result = rarityData.EpicCommsumable[randfactor];
                        break;
                    case ItemRarity.Unique:
                        randfactor = randomInstance.Next(0, rarityData.UniqueCommsumable.Count);
                        result = rarityData.UniqueCommsumable[randfactor];
                        break;
                    case ItemRarity.Legendary:
                        randfactor = randomInstance.Next(0, rarityData.LegendaryCommsumable.Count);
                        result = rarityData.LegendaryCommsumable[randfactor];
                        break;
                }
                break;
            case ItemType.Key:
                switch (randRarity)
                {
                    case ItemRarity.Normal:
                    case ItemRarity.Rare:
                    case ItemRarity.Epic:
                        result = rarityData.EpicKey[0];
                        break;
                    case ItemRarity.Unique:
                        result = rarityData.UniqueKey[0];
                        break;
                    case ItemRarity.Legendary:
                        result = rarityData.LegendaryKey[0];
                        break;
                }
                break;
        }

        return result;
    }

    public void SetPrice()
    {
        costText.text = meokdoriData.Costs[curIndex].ToString();
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
            seed = UnityEngine.Random.Range(0, 10000);

        Debug.Log($"Init Seed : {seed}");
    }

    public virtual void OnInteract(Interactor interactor)
    {
        int cost = meokdoriData.Costs[curIndex];
        if (PhotonNetwork.IsConnected)
        {
            if (!PhotonNetwork.IsMasterClient && prizeNum > 0 && InventoryManager.Instance.GoldCount > cost && !isEating)
            {
                isEating = true;
                InventoryManager.Instance.SpendGold(cost);
                photonView.RPC("PickMeokdoriItem", RpcTarget.MasterClient);
                return;
            }
        }
        else
        {
            Debug.Log("etnered');");
            if (prizeNum > 0 && InventoryManager.Instance.GoldCount > cost && !isEating)
            {
                isEating = true;
                InventoryManager.Instance.SpendGold(cost);
                PickMeokdoriItem();
            }
        }
    }

    [PunRPC]
    public void PickMeokdoriItem()
    {
        Debug.Log("PickMeokdoriItem called");
        GetItem();
    }

    // 1회 뽑기
    public void GetItem()
    {
        isEating = true;
        animator.SetTrigger("Paied");
        if (ItemList[curIndex] != null)
        {
            Debug.Log($"Prize Result: {ItemList[curIndex]}");
            //NetworkItemDrop(ItemList[curIndex]);
            ItemFactory.Instance.SpawnItem(ItemList[curIndex], transform.position);
            prizeNum--;
            animator.SetTrigger("Prize");
        }
        else
        {
            Debug.Log($"Prize Result: 꽝");
            animator.SetTrigger("NoPrize");
        }
        curIndex += 1;
        SetPrice();
        if (prizeNum <= 0)
        {
            animator.SetBool("Sleep", true);
        }
        isEating = false;
    }

    private void SetAnimation()
    {
        switch (meokdoriData.Grade)
        {
            case MeokdoriGrade.Normal:
                animator.SetLayerWeight(1, 1);
                break;
            case MeokdoriGrade.Golden:
                animator.SetLayerWeight(2, 2);
                break;
        }

    }
    public void NetworkItemDrop(BaseItemData itemData)
    {
        object[] data = ItemDataManager.Instance.ConvertToObjectArray(itemData);

        DropedItem dropedItem = PhotonNetwork.InstantiateRoomObject
            ("DroptedItem", transform.position, Quaternion.identity, 0, data).GetComponent<DropedItem>();
        dropedItem.ItemData = itemData;
        dropedItem.Drop(dropRange, dropSpeed);
    }

    public virtual void ShowUI()
    {
        Debug.Log($"{gameObject.name} Show UI");
    }

    public virtual void HideUI()
    {
        Debug.Log($"{gameObject.name} Hide UI");
    }

#if UNITY_EDITOR

    [ContextMenu("Debug BuyItem")]
    public void DebugBuyItem()
    {
        GetItem();
    }
#endif
}
using System.Collections.Generic;
using UnityEngine;
using Tae.Inventory;
// - 20 different potions (stored as assets)
// - 20 different potion visuals (stored in a visual container)
// 1. Assign each potion's SO a random dynamicID
// 3. Each potion uses its ID to get its correct visual through animator
// 4. Store used potions' ids in hashset
// 5. When trying to check potion's tooltip, check if it is in hashet

public class PotionManager : MonoBehaviour
{
    public static PotionManager Instance { get; private set; }

    [SerializeField] UnknownPotionData[] unknownPotionPool;
    [SerializeField] Tae.Inventory.BasePotion[] potionSOs;
    public Dictionary<int, UnknownPotionData> unknownPotionDic = new();
    public Dictionary<int, Tae.Inventory.BasePotion> potionSODic = new();
    private List<int> idPool = new List<int>();
    private HashSet<int> usedPotionIDs = new HashSet<int>();

    // (효석)이거 따로 매니저 만들어줘야될까?
    [SerializeField] BaseScrollItem[] scorllItemsSO;
    public Dictionary<int, BaseScrollItem> scrollSODir = new();

    // Getters
    public UnknownPotionData PotionBottleData(int id) => unknownPotionDic[PotionBottleID(id)];
    public int PotionBottleID(int id) => potionSODic[id].dynamicID;


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

        AssignPotionIDs();
        AssingnScrollIDS();
    }

    // 모든 포션에게 랜덤 ID를 지정한다
    private void AssignPotionIDs()
    {
        unknownPotionDic.Clear();
        potionSODic.Clear();

        // Create Unknown Potion Dictionary for easy access
        foreach (UnknownPotionData data in unknownPotionPool)
        {
            unknownPotionDic.Add(data.id, data);
            idPool.Add(data.id);
        }

        // Shuffle ID list
        Tae.MathUtilities.ShuffleList(idPool, 1);

        // Assign ID to every potion
        for (int i = 0; i < potionSOs.Length; i++)
        {
            potionSOs[i].dynamicID = idPool[i];
            potionSODic.Add(potionSOs[i].ID, potionSOs[i]);
        }
    }

    public void SetOpen(int id)
    {
        usedPotionIDs.Add(id);
    }
    public void AllSetOpen()
    {
        foreach (int id in potionSODic.Keys)
        {
            if (usedPotionIDs.Contains(id))
                continue;
            usedPotionIDs.Add(id);
        }
    }
    public bool GetOpen(int id)
    {
        return usedPotionIDs.Contains(id);
    }

    public void RequestUsePotion(int id)
    {
        potionSODic[id].Use(Manager.Game.MyPlayer);
    }

    // 효석 작업 (여기 들어가면 안될거같은데 어케해야될지 모르겠어서 일단 넣어봄)
    private void AssingnScrollIDS()
    {
        scrollSODir.Clear();
        foreach (BaseScrollItem data in scorllItemsSO)
        {
            scrollSODir.Add(data.ID, data);
        }
    }
    public void UseScrollItem(int id)
    {
        scrollSODir[id].Use(Manager.Game.MyPlayer);
    }
}

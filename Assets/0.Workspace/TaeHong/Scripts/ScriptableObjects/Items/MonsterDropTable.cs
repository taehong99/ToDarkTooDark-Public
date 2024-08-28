using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tae.Inventory;
using ItemLootSystem;

[CreateAssetMenu(menuName = "ItemLoot/MonsterDropTable")]
public class MonsterDropTable : ScriptableObject
{
    public List<MonsterDrop> dropTable;
    private (int, int)[] NormalGoldDropTable = { (2, 5), (6, 11), (9, 15), (18, 32), (18, 32) };
    private (int, int)[] EliteGoldDropTable = { (7, 15), (20, 35), (30, 55), (60, 110), (60, 110) };


    public (int, int) NormalGoldToDrop => NormalGoldDropTable[Manager.Game.GetPhaseNumber() - 1];
    public (int, int) EliteGoldToDrop => EliteGoldDropTable[Manager.Game.GetPhaseNumber() - 1];

    public int GetGoldToDrop(MonsterType type)
    {
        (int, int) range;
        if (type == MonsterType.Normal)
            range = NormalGoldToDrop;
        else
            range = EliteGoldToDrop;

        return Random.Range(range.Item1, range.Item2 + 1);
    }

    public List<StackableDrop> GetRandomDrops()
    {
        List<StackableDrop> randomDrops = new();
        float random;
        int count;
        foreach(MonsterDrop drop in dropTable)
        {
            random = Random.Range(0, 1f);
            if(random <= (drop.dropRate * 0.01f))
            {
                count = Random.Range(drop.minDrops, drop.maxDrops + 1);
                randomDrops.Add(new StackableDrop(drop.item, count));
            }
        }
        return randomDrops;
    }
}

[System.Serializable]
public struct MonsterDrop
{
    public BaseItemData item;
    public int minDrops;
    public int maxDrops;
    public float dropRate;
}

public struct StackableDrop
{
    public BaseItemData item;
    public int count;

    public StackableDrop(BaseItemData item, int count)
    {
        this.item = item;
        this.count = count;
    }
}

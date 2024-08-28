using ItemLootSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RarityData : ScriptableObject
{
    [Header("Equiptment RarityData")]
    public List<BaseItemData> NormalEquiptment;
    public List<BaseItemData> RareEquiptment;
    public List<BaseItemData> EpicEquiptment;
    public List<BaseItemData> UniqueEquiptment;
    public List<BaseItemData> LegendaryEquiptment;
    public List<BaseItemData> ArtifactEquiptment;

    [Space]

    [Header("Commsumable Item RarityData")]
    public List<BaseItemData> NormalCommsumable;
    public List<BaseItemData> RareCommsumable;
    public List<BaseItemData> EpicCommsumable;
    public List<BaseItemData> UniqueCommsumable;
    public List<BaseItemData> LegendaryCommsumable;

    [Space]

    [Header("ETC Item RarityData")]
    public List<BaseItemData> NormalETC;
    public List<BaseItemData> RareETC;
    public List<BaseItemData> EpicETC;
    public List<BaseItemData> UniqueETC;
    public List<BaseItemData> LegendaryETC;

    [Space]

    [Header("Key Item RarityData")]
    public List<BaseItemData> EpicKey;
    public List<BaseItemData> UniqueKey;
    public List<BaseItemData> LegendaryKey;
}

using UnityEngine;
using AYellowpaper;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEditor;

namespace ItemLootSystem
{

    [CreateAssetMenu(menuName = "ItemData/ItemDictionary")]
    public class ItemDictionary : ScriptableObject
    {
       [SerializedDictionary("ID", "ItemData")]
       public SerializedDictionary<int, BaseItemData> itemDB  = new SerializedDictionary<int, BaseItemData>();


    }
}

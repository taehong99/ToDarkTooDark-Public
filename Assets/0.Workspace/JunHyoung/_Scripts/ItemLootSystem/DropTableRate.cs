using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemLootSystem
{
    [CreateAssetMenu(menuName = "ItemLoot/DropTableRate")]
    public class DropTableRate : ScriptableObject
    {
        [Header("NormalRoom TreasureBox Ratio")]
        [SerializeField] public float DtreasureBoxRate = 39f;
        [SerializeField] public float CtreasureBoxRate = 30f;
        [SerializeField] public float BtreasureBoxRate = 20f;
        [SerializeField] public float AtreasureBoxRate = 8f;
        [SerializeField] public float StreasureBoxRate = 3f;


        [Space(10), Header("SecretRoom TresureBox Raito")]
        [SerializeField] public float Phase2AtreasureBoxRate = 70f;
        [SerializeField] public float Phase3AtreasureBoxRate = 40f;
    }
}
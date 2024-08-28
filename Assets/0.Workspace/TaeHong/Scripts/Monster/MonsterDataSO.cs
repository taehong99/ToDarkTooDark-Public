using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Monster", fileName = "MonsterData")]
public class MonsterDataSO : ScriptableObject
{
    [Header("Properties")]
    public string _name;

    [Header("Stats")]
    public float movespeedMultiplier;
    public float attackRange;
    public float attackCooldown;
    public float skill1Cooldown;

    public MonsterPhaseStats phaseStats;
}

[System.Serializable]
public struct MonsterPhaseStats
{
    public int[] power;
    public int[] armor;
    public int[] hp;
    public int[] exp;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Player/PlayerStat", fileName = "PlayerStat")]
public class PlayerStat : ScriptableObject
{
    public string CharacterName;
    public int level;
    public float power;
    public float armor;
    public float hp;
    public float hpRegen;
    public float speed;
    public float critical;
    public float critMultiplier;
    public float damageReduction;
}

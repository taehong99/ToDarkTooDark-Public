using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PlayerStatusEffect
{
    None,       // 상태 이상 없음
    Stunned,    // 기절
    Bleeding,   // 출혈
    Poisoned,   // 중독
    ForcedMove, // 강제이동
    Silenced,   // 침묵
    Blinded,    // 암흑
    Slowed      // 둔화
}

[CreateAssetMenu(menuName = "Data/Player/SkillSO", fileName = "SkillSO")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    public int skillLevel;
    public float[] damageMultiplier;
    public float range;
    public PlayerStatusEffect[] effect;
    public float[] coolTime;
    public float startupTime;
    public float cooldownTime;
}
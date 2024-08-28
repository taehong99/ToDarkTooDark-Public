using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class PlayerData : MonoBehaviour
{
    [Header("base")]
    [SerializeField] PlayerStat data;
    public int level;
    public float power;
    public float armor;
    public float hp;
    public float hpRegen;
    public float critical;
    public float critMultiplier;
    public float speed;
    public float damageReduction;

    [Header("Attack")]
    [SerializeField] SkillSO attack;
    public float[] attDamageMultiplier;
    public float attRange;
    public PlayerStatusEffect[] attEffect;
    public float [] attCoolTime;
    public float attStartupTime;
    public float attCooldownTime;

    [Header("Dash")]
    [SerializeField] SkillSO dash;
    public int dashLevel;
    public float [] dashDamageMultiplier;
    public float dashRange;
    public PlayerStatusEffect[] dashEffect;
    public float [] dashCoolTime;
    public float dashStartupTime;
    public float dashCooldownTime;

    [Header("Skill")]
    [SerializeField] SkillSO skill;
    public int skillLevel;
    public float [] skillDamageMultiplier;
    public float skillRange;
    public PlayerStatusEffect[] skillEffect;
    public float [] skillCoolTime;
    public float skillStartupTime;
    public float skillCooldownTime;

    [Header("Utimate")]
    [SerializeField] SkillSO utimate;
    public int utiLevel;
    public float [] utiDmageMultiplier;
    public float utiRange;
    public PlayerStatusEffect[] utiEffect;
    public float [] utiCoolTime;
    public float utiStartupTime;
    public float utiCooldownTime;

    [Header("Passive")]
    [SerializeField] Passive passive;
    public int passiveLevel;
    public float[] passiveMultiplier;
    private void Awake()
    {
        // 기본스탯
        power = data.power;
        level = data.level;
        armor = data.armor;
        hp = data.hp;
        hpRegen = data.hpRegen;
        critical = data.critical;
        critMultiplier = data.critMultiplier;
        speed = data.speed;
        damageReduction = data.damageReduction;

        // 기본공격
        attDamageMultiplier = attack.damageMultiplier;
        attRange = attack.range;
        attCoolTime = attack.coolTime;
        attStartupTime = attack.startupTime;
        attCooldownTime = attack.cooldownTime;

        // 대쉬
        if (dash != null)
        {
            dashLevel = dash.skillLevel;
            dashDamageMultiplier = dash.damageMultiplier;
            dashRange = dash.range;
            dashCoolTime = dash.coolTime;
            dashStartupTime = dash.startupTime;
            dashCooldownTime = dash.cooldownTime;
        }
        // 스킬
        if (skill != null)
        {
            skillLevel = skill.skillLevel;
            skillDamageMultiplier = skill.damageMultiplier;
            skillRange = skill.range;
            skillCoolTime = skill.coolTime;
            skillStartupTime = skill.startupTime;
            skillCooldownTime = skill.cooldownTime;
        }
        // 궁극기
        if (utimate != null)
        {
            utiLevel = utimate.skillLevel;
            utiDmageMultiplier = utimate.damageMultiplier;
            utiRange = utimate.range;
            utiCoolTime = utimate.coolTime;
            utiStartupTime = utimate.startupTime;
            utiCooldownTime = utimate.cooldownTime;
        }
        // 패시브
        if (passive != null)
        {
            passiveLevel = passive.passiveLevel;
            passiveMultiplier = passive.passiveMultiplier;
        }
    }
}


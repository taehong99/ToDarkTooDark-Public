using System;
using System.Collections.Generic;
using Tae;
using UnityEngine;
using System;
using Photon.Pun;

public enum SkillType
{
    Skill,
    Dash,
    Utimate,
    Passive
}
public enum StatType
{
    Power = 1,
    Armor,
    MaxHp,
    HpRegen,
    Speed,
    Critical,
    CriticalMultiplier,
    DamageReduction
}


// 아이템이나 버프, 디버프, 패시브등이 있을때 스탯 변경
// 최종 스탯을 여기서 결정하니 모든 스탯들이 관리 되어야함
public class PlayerStatsManager : MonoBehaviourPun
{
    [SerializeField] PlayerData data;
    PlayerData Data { get { return data; } }
    [SerializeField] StatIncreaseSO statUp;
    [SerializeField] PlayerHealth playerHealth;

    Dictionary<StatType, float> BaseStat = new Dictionary<StatType, float>();
    Dictionary<StatType, float> PercentStat = new Dictionary<StatType, float>();
    public Dictionary<SkillType, int> SkillLevelUpDatas = new Dictionary<SkillType, int>();

    [SerializeField] StatType[] ClassType;
    [Header("레벨 업시 상승량 ")]
    float powerUp;
    float armorUp;
    float hpUp;
    float hpRegenUp;
    float SpeedUp;
    float criticalUp;
    float critMultiplierUp;

    [Header("현재 스탯")]
    private int curLevel;
    public int CurLevel { get => curLevel; set { curLevel = value; levelChangedEvent?.Invoke(curLevel); } }
    private int curEXP;
    public int CurEXP { get => curEXP; set { curEXP = value; CheckLeveUp(value); EXPChangedEvent?.Invoke(curEXP); } }
    private int maxExp = 200;
    public int MaxEXP { get => maxExp; set { maxExp = value; maxEXPChangedEvent?.Invoke(maxExp); } }
    public float curPower;
    public float curArmor;
    public float curHpRegen;
    public float curSpeed;
    public float curCritical;
    public float curCriticalMultiplier;
    public float curDamageReduction;

    public event Action<int> levelChangedEvent;
    public event Action<int> EXPChangedEvent;
    public event Action<int> maxEXPChangedEvent;

    int skillPoint;

    // 스탯 이벤트
    public event Action<StatType, float> StatChangedEvent;
    public float MaxHp { get => playerHealth.MaxHealth; set { playerHealth.MaxHealth = (int) value; } }
    public float Power { get => curPower; set { curPower = value; StatChangedEvent?.Invoke(StatType.Power, curPower); } }
    public float Armor { get => curArmor; set { curArmor = value; StatChangedEvent?.Invoke(StatType.Armor, curArmor); } }
    public float Speed { get => curSpeed; set { curSpeed = value; StatChangedEvent?.Invoke(StatType.Speed, curSpeed); } }
    public float CritRate { get => curCritical; set { curCritical = value; StatChangedEvent?.Invoke(StatType.Critical, curCritical); } }
    public float CritDmg { get => curCriticalMultiplier; set { curCriticalMultiplier = value; StatChangedEvent?.Invoke(StatType.CriticalMultiplier, curCriticalMultiplier); } }
    public float Regen { get => curHpRegen; set { curHpRegen = value; StatChangedEvent?.Invoke(StatType.HpRegen, curHpRegen); } }
    public float Reduction { get => curDamageReduction; set { curDamageReduction = value; StatChangedEvent?.Invoke(StatType.DamageReduction, curDamageReduction); } }


    /**********************************************
    *                 Unity Event
    ***********************************************/

    private void Awake()
    {
        powerUp = statUp.powerUp;
        armorUp = statUp.armorUp;
        hpUp = statUp.hpUp;
        hpRegenUp = statUp.hpRegenUp;
        SpeedUp = statUp.speedUp;
        criticalUp = statUp.criticalUp;
        critMultiplierUp = statUp.critMultiplierUp;

        // 최종 스탯이 될 친구
        BaseStat.Add(StatType.MaxHp, data.hp);
        BaseStat.Add(StatType.Power, data.power);
        BaseStat.Add(StatType.Armor, data.armor);
        BaseStat.Add(StatType.HpRegen, data.hpRegen);
        BaseStat.Add(StatType.Speed, data.speed);
        BaseStat.Add(StatType.Critical, data.critical);
        BaseStat.Add(StatType.CriticalMultiplier, data.critMultiplier);
        BaseStat.Add(StatType.DamageReduction, data.damageReduction);
        CurLevel = data.level;

        PercentStat.Add(StatType.Power, 0);
        PercentStat.Add(StatType.Armor, 0);
        PercentStat.Add(StatType.MaxHp, 0);
        PercentStat.Add(StatType.HpRegen, 0);
        PercentStat.Add(StatType.Speed, 0);
        PercentStat.Add(StatType.Critical, 0);
        PercentStat.Add(StatType.CriticalMultiplier, 0);
        PercentStat.Add(StatType.DamageReduction, 0);

        SkillLevelUpDatas.Add(SkillType.Utimate, data.utiLevel);
        SkillLevelUpDatas.Add(SkillType.Skill, data.skillLevel);
        SkillLevelUpDatas.Add(SkillType.Dash, data.dashLevel);
        SkillLevelUpDatas.Add(SkillType.Passive, data.passiveLevel);
        CurrentStats();
    }

    private void Start()
    {
        CurrentStats();
    }


    /**********************************************
    *                 EXP/Level Up
    ***********************************************/
    private void CheckLeveUp(int newcurExp)
    {
        if (newcurExp >= maxExp)
        {
            PlayerLevelUp();
        }
    }

    public void GainExp(int amount)
    {
        if(PhotonNetwork.IsConnected)
            photonView.RPC("ClientGainExp", photonView.Owner, amount);
        else
            ClientGainExp(amount);
    }

    [PunRPC]
    public void ClientGainExp(int amount)
    {
        Debug.Log($"{name} gained {amount} exp");
        CurEXP += amount;
    }

    private void PlayerLevelUp()
    {
        
        if (curLevel < 10)
        {
            CurLevel++;
            Debug.Log($"{gameObject.name} leveled up to level {curLevel}");
            UpdateStat(StatType.Power, powerUp, true);
            UpdateStat(StatType.Armor, armorUp, true);
            UpdateStat(StatType.MaxHp, hpUp, true);
            UpdateStat(StatType.HpRegen, hpRegenUp, true);
            UpdateStat(StatType.Speed, SpeedUp, true);
            UpdateStat(StatType.Critical, criticalUp, true);
            UpdateStat(StatType.CriticalMultiplier, critMultiplierUp, true);
            CurEXP -= maxExp;
            if (curLevel < 7)
            {
                MaxEXP += 20;
            }
            else
            {
                MaxEXP += 50;
            }
        }

        CurrentStats();
    }

    [ContextMenu("TextExpUp")]
    public void DebugExpUp()
    {
        CurEXP += 50;
    }
    [ContextMenu("SkillUp")]
    public void DebugSkiilUp()
    {
        SkillLevelUpDatas[SkillType.Skill] += 1;
        SkillLevelUpDatas[SkillType.Utimate] += 1;
        SkillLevelUpDatas[SkillType.Dash] += 1;
        SkillLevelUpDatas[SkillType.Passive] += 1;
        PassiveStat(ClassType, data.passiveMultiplier[SkillLevelUpDatas[SkillType.Passive]]);
    }

    public void SkillLevelUp(SkillType type)
    {
        // 3렙 이상은 예외
        if (SkillLevelUpDatas[type] > 3)
            return;

        SkillLevelUpDatas[type] += 1;
        PassiveStat(ClassType, data.passiveMultiplier[SkillLevelUpDatas[SkillType.Passive]]);
    }

    /**********************************************
    *                 Stats
    ***********************************************/
    public void CurrentStats()
    {
        Power = FinalStat(StatType.Power);
        Armor = FinalStat(StatType.Armor);
        MaxHp = FinalStat(StatType.MaxHp);
        Regen = FinalStat(StatType.HpRegen);
        Speed = FinalStat(StatType.Speed);
        CritRate = FinalStat(StatType.Critical);
        CritDmg = FinalStat(StatType.CriticalMultiplier);
    }

    private void PassiveStat(StatType[] types, float value)
    {
        foreach (StatType type in types)
        {
            PercentStat[type] += value;
        }
    }

    public void UpdateStat(StatType type, float value, bool isPercent = false)
    {
        if (type == StatType.Critical || type == StatType.CriticalMultiplier)
            value *= 0.01f;

        if (isPercent)
        {
            PercentStat[type] += value;
        }
        else
        {
            BaseStat[type] += value;
        }
        CurrentStats();
    }
    public float FinalStat(StatType type)
    {
        return BaseStat[type] * (1 + PercentStat[type]);
    }
}
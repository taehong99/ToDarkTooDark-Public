using Tae;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum TransformTarget { Tree, Stone }

public class Buff : StatusEffect
{
    StatType statType;
    float boost;
    bool isPrecent;
    PlayerStatsManager stat;
    public Buff(string name, float duration, StatType statType, float boost, bool isPrecent) : base(name, duration)
    {
        this.statType = statType;
        this.boost = boost;
        this.isPrecent = isPrecent;
    }

    public override void ApplyEffect(GameObject target)
    {
        stat = target.GetComponentInParent<PlayerStatsManager>();
        stat.UpdateStat(statType, boost, isPrecent);
        stat.CurrentStats();
    }

    public override void RemoveEffect(GameObject target)
    {
        stat = target.GetComponentInParent<PlayerStatsManager>();
        stat.UpdateStat(statType, -boost, isPrecent);
        stat.CurrentStats();
    }
}
public class DeBuff : StatusEffect
{
    StatType statType;
    float boost;
    bool isPrecent;
    PlayerStatsManager stat;
    
    public DeBuff(string name, float duration, StatType statType, float boost, bool isPrecent) : base(name, duration)
    {
        this.statType = statType;
        this.boost = boost;
        this.isPrecent = isPrecent;
    }

    public override void ApplyEffect(GameObject target)
    {
        stat = target.GetComponentInParent<PlayerStatsManager>();
        stat.UpdateStat(statType, boost, isPrecent);
        stat.CurrentStats();
    }

    public override void RemoveEffect(GameObject target)
    {
        stat = target.GetComponentInParent<PlayerStatsManager>();
        stat.UpdateStat(statType, -boost, isPrecent);
        stat.CurrentStats();
    }
}

public class Stun : StatusEffect
{
    GameObject particle;
    // 엑칼 떨구는건 없음...
    public Stun(string name, float duration, GameObject particle) : base(name, duration)
    {
        this.particle = particle;
    }
    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<PlayerStates>().Stop();
    }

    public override void RemoveEffect(GameObject target)
    {
        target.GetComponent<PlayerStates>().Normal();
        particle.SetActive(false);
    }
}
public class Shield : StatusEffect
{
    float Precent;
    float duration;
    int maxHp;
    public Shield(string name, float duration, float Precent) : base(name, duration)
    {
        this.Precent = Precent;
        this.duration = duration;
    }

    public override void ApplyEffect(GameObject target)
    {
        PlayerHealth health = target.GetComponent<PlayerHealth>();
        maxHp = health.MaxHealth;
        health.CreateShield((int) (maxHp * Precent), duration);
    }

    public override void RemoveEffect(GameObject target) { }
}
public class SilenceDebuff : StatusEffect
{
    public SilenceDebuff(string name, float duration) : base(name, duration) { }
    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<PlayerStates>().BlackOutState();
    }

    public override void RemoveEffect(GameObject target)
    {
        target.GetComponent<PlayerStates>().Normal();
    }
}
public class DotDamage : StatusEffect
{
    float precent;
    float duration;
    bool isPrecent;
    public DotDamage(string name, float duration, float precent, bool isPrecent) : base(name, duration)
    {
        this.precent = precent;
        this.duration = duration;
        this.isPrecent = isPrecent;
    }

    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<PlayerHealth>().TakeDotDamage(precent, (int) duration, isPrecent);
    }

    public override void RemoveEffect(GameObject target)
    {
    }
}
public class BlindnessEffect : StatusEffect
{

    public BlindnessEffect(string name, float duration) : base(name, duration)
    {
    }

    public override void ApplyEffect(GameObject target)
    {
        Debug.Log("암흑 발동");
        if (target.GetComponentInChildren<Light2D>() == null)
            return;
        target.GetComponentInChildren<Light2D>().falloffIntensity = 1f;
    }

    public override void RemoveEffect(GameObject target)
    {
        Debug.Log("암흑 끝");
        if (target.GetComponentInChildren<Light2D>() == null)
            return;
        target.GetComponentInChildren<Light2D>().falloffIntensity = 0.419f;
    }
}

public class TreeForm : StatusEffect
{
    PlayerStatsManager stats;
    PlayerStates playerStates;
    Animator animator;
    public TreeForm(string name, float duration) : base(name, duration)
    {
    }

    public override void ApplyEffect(GameObject target)
    {
        stats = target.GetComponent<PlayerStatsManager>();
        playerStates = target.GetComponent<PlayerStates>();
        animator = target.GetComponent<Animator>();
        stats.UpdateStat(StatType.HpRegen, 0.02f, true);
        stats.UpdateStat(StatType.Armor, 0.1f, true);
        stats.FinalStat(StatType.Armor);
        stats.FinalStat(StatType.HpRegen);
        // 아이템 사용 금지....
        playerStates.Stop();
        // 스킬도 멈춰줘야되서
        playerStates.BlackOutState();
        playerStates.IsUsedItem = false;
        animator.SetBool("Tree", true);
    }

    public override void RemoveEffect(GameObject target)
    {
        stats.UpdateStat(StatType.HpRegen, -0.02f, true);
        stats.UpdateStat(StatType.Armor, -0.5f, true);
        stats.FinalStat(StatType.Armor);
        stats.FinalStat(StatType.HpRegen);
        playerStates.Normal();
        playerStates.IsUsedItem = true;
        animator.SetBool("Tree", false);

    }
}
public class StoneForm : StatusEffect
{
    PlayerStatsManager stats;
    PlayerStates playerStates;
    PlayerHealth health;
    Animator animator;
    float duration;
    public StoneForm(string name, float duration) : base(name, duration)
    {
        this.duration = duration;
    }

    public override void ApplyEffect(GameObject target)
    {
        playerStates = target.GetComponent<PlayerStates>();
        stats = target.GetComponent<PlayerStatsManager>();
        health = target.GetComponent<PlayerHealth>();
        animator = target.GetComponent<Animator>();
        animator.SetBool("Stone", true);
        playerStates.Stop();
        playerStates.BlackOutState();
        playerStates.IsUsedItem = false;
        stats.UpdateStat(StatType.Armor, 0.3f, true);
        stats.FinalStat(StatType.Armor);
        health.CreateShield((int) (stats.MaxHp * 0.5f), duration);
    }
    public override void RemoveEffect(GameObject target)
    {
        animator.SetBool("Stone", false);
        playerStates.Normal();
        playerStates.IsUsedItem = true;
        stats.UpdateStat(StatType.Armor, -0.3f, true);
        stats.FinalStat(StatType.Armor);
    }
}
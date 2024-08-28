using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Tae;

public class UsableItemMethods : MonoBehaviour
{
    Dictionary<int, Action> itemMethodsByIndex = new();
    GameObject user => Manager.Game.MyPlayer;

    public void UseItem(int id)
    {
        itemMethodsByIndex[id].Invoke();
    }

    // 아이템의 기능을 ID로 찾기위해 딕셔너리에 보관
    private void InitMethods()
    {
        //itemMethodsByIndex.Add(121, UsePotion121);
    }

    #region Util
    private void UseHealingPotion(float amountToHeal, float duration, bool isPercent)
    {
        if (!user.TryGetComponent(out PlayerHealth player))
            return;

        if (duration == 0)
        {
            player.Heal(amountToHeal, isPercent);
        }
        else
        {
            player.PotionHealOverTime(amountToHeal, duration);
        }
    }

    private void UseStatIncreasePotion(StatType statType, float duration, float amount, bool isPercent)
    {
        if (!user.TryGetComponent(out StatusEffectManager player))
            return;

        Buff buff = new Buff("포션", duration, statType, amount, isPercent);
        player.AddEffect(buff);
    }

    private void UseStatusEffectPotion(StatusEffectType type)
    {
        if (!user.TryGetComponent(out StatusEffectManager player))
            return;

        StatusEffect statusEffect;
        switch (type)
        {
            case StatusEffectType.Blind:
                statusEffect = new BlindnessEffect("", 10);
                break;
            case StatusEffectType.Silence:
                statusEffect = new SilenceDebuff("", 10);
                break;
            case StatusEffectType.Burn:
                statusEffect = new DotDamage("", 5, 1, true);
                break;
            case StatusEffectType.Poison:
                statusEffect = new DotDamage("", 5, 2, true);
                break;
            default:
                statusEffect = new BlindnessEffect("", 0);
                break;
        }

        player.AddEffect(statusEffect);
    }

    private void UseTransformPotion(TransformTarget transformTarget, float duration)
    {
        if (!user.TryGetComponent(out StatusEffectManager player))
            return;

        StatusEffect statusEffect;
        switch (transformTarget)
        {
            case TransformTarget.Tree:
                statusEffect = new TreeForm("Tree", duration);
                break;
            case TransformTarget.Stone:
                statusEffect = new StoneForm("Stone", duration);
                break;
            default:
                statusEffect = new TreeForm("", 0);
                break;
        }

        player.AddEffect(statusEffect);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;

public class HealPotion : BasePotion
{
    float healingAmount;
    float healingPrecent;
    [SerializeField] GameObject particle;
    PlayerHealth health;
    private void Start()
    {
        health = GetComponentInParent<PlayerHealth>();
        if (potionSO.grade == PotionGrade.Epic)
        {
            healingPrecent = 0.1f;
        }
        else
        {
            healingPrecent = 1;
        }
    }
    [ContextMenu("Use Potion")]
    public override void UseItem()
    {
        if (isCool)
        {
            base.UseItem();
            healingAmount = health.MaxHealth * healingPrecent;
            // 힐량보다 체력이 크면 최대체력으로
            if (healingAmount + health.Health > health.MaxHealth && !health.IsUndead)
            {
                healingAmount = health.MaxHealth - health.Health;
            }
            health.Heal(healingAmount, false);
            StartCoroutine(ParticleStart());
        }
    }
    IEnumerator ParticleStart()
    {
        particle.SetActive(true);
        yield return new WaitForSeconds(1f);
        particle.SetActive(false);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tae;
using System;

public class Slime : BaseMonster
{
    const float SLIME_HEAL_THRESHOLD = 0.5f; // <50%
    const float SLIME_HEAL_AMT = 20; // 20%

    [Header("Attack Colliders")]
    [SerializeField] BoxCollider2D attack01Collider;
    private Coroutine attackRoutine;

    private void Start()
    {
        base.Start();
        monsterHealth.healthChangedEvent += OnHealthChanged;
    }

    private void OnHealthChanged(int newHealth)
    {
        // HP가 50% 이하가 되면, 전체 HP의 20%를 즉시 회복한다. (1초간 실행)
        if (newHealth <= (monsterHealth.MaxHealth * SLIME_HEAL_THRESHOLD))
        {
            monsterHealth.Heal(SLIME_HEAL_AMT, true);
            monsterHealth.healthChangedEvent -= OnHealthChanged;
            StartCoroutine(HealRoutine());
        }
    }

    private IEnumerator HealRoutine()
    {
        attackFinished = false;
        PlayAnim(Convert.ToByte(Animation.Attack02));
        yield return new WaitForSeconds(1f);
        PlayAnim(Convert.ToByte(Animation.Idle));
        attackFinished = false;
    }

    protected override void StartAttack()
    {
        base.StartAttack();
        attackRoutine = StartCoroutine(AttackRoutine());
    }

    protected override void StopAttack()
    {
        base.StopAttack();
        StopCoroutine(attackRoutine);
    }

    private IEnumerator AttackRoutine()
    {
        // 공격 준비
        attackFinished = false;
        PlayAnim(Convert.ToByte(Animation.Idle));
        yield return new WaitForSeconds(0.5f);

        // 공격 
        PlayAnim(Convert.ToByte(Animation.Attack01));
        yield return new WaitForSeconds(0.5f);

        // 공격후 잠깐 대기
        PlayAnim(Convert.ToByte(Animation.Idle));
        yield return new WaitForSeconds(1.5f);
        attackFinished = true;
    }

    public override void Attack01Frame()
    {
        SlimeSpit();
    }

    private void SlimeSpit()
    {
        Vector2 offset = attack01Collider.offset;
        if (!isFacingRight)
            offset.x *= -1;
        Vector2 center = (Vector2) transform.position + offset;
        int count = Physics2D.OverlapBoxNonAlloc(center, attack01Collider.size, 0, colliders, playerMask);

        for (int i = 0; i < count; i++)
        {
            if (colliders[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(Power);

                // 10%확률로 플레이어에게 암흑 상태를 적용한다.
                float random = UnityEngine.Random.Range(0, 1f);
                if(random <= 1f)
                {
                    if (colliders[i].TryGetComponent(out StatusEffectManager player))
                    {
                        player.AddEffect(new BlindnessEffect("", 2f));
                    }
                }
            }
        }
    }
}

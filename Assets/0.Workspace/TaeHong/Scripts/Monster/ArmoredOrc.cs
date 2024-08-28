using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tae;
using System;

public class ArmoredOrc : BaseMonster
{
    [Header("Attack Colliders")]
    [SerializeField] BoxCollider2D attack01Collider;
    [SerializeField] BoxCollider2D attack02Collider;
    [SerializeField] BoxCollider2D attack03Collider;

    private Coroutine attackRoutine;

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

        // 공격/스킬 사용
        float random = UnityEngine.Random.Range(0f, 1f);
        if (random <= 0.7f) // 기본 공격 70%
        {
            PlayAnim(Convert.ToByte(Animation.Attack01));
            yield return new WaitForSeconds(0.5f);
        }
        else // 스킬 30% 확률
        {
            yield return StartCoroutine(ComboAttack());
        }

        // 공격후 잠깐 대기
        PlayAnim(Convert.ToByte(Animation.Idle));
        yield return new WaitForSeconds(1.5f);
        attackFinished = true;
    }

    private IEnumerator ComboAttack()
    {
        PlayAnim(Convert.ToByte(Animation.Attack01));
        yield return new WaitForSeconds(0.6f);
        PlayAnim(Convert.ToByte(Animation.Attack02));
        yield return new WaitForSeconds(0.7f);
        PlayAnim(Convert.ToByte(Animation.Attack03));
        yield return new WaitForSeconds(0.8f);
    }

    // Slash
    public override void Attack01Frame()
    {
        BoxColliderAttackFrame(attack01Collider, 1);
    }

    // Stab
    public override void Attack02Frame()
    {
        BoxColliderAttackFrame(attack02Collider, 0.9f);
    }

    // Slam
    public override void Attack03Frame()
    {
        BoxColliderAttackFrame(attack03Collider, 1);
    }
}

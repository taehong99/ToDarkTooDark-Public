using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;

public class Skeleton : BaseMonster
{
    [Header("Attack Colliders")]
    [SerializeField] BoxCollider2D attack01Collider;
    [SerializeField] BoxCollider2D attack02Collider;

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
        if (random <= 0.9f) // 기본 공격 90% 확률
        {
            PlayAnim(Convert.ToByte(Animation.Attack01));
            yield return new WaitForSeconds(0.5f);
        }
        else // 스킬 10% 확률
        {
            PlayAnim(Convert.ToByte(Animation.Attack02));
            yield return new WaitForSeconds(1f);
        }

        // 공격후 잠깐 대기
        PlayAnim(Convert.ToByte(Animation.Idle));
        yield return new WaitForSeconds(1.5f);
        attackFinished = true;
    }

    public override void Attack01Frame()
    {
        UpAttackFrame();
    }

    public override void Attack02Frame()
    {
        DownAttackFrame();
    }

    private void UpAttackFrame()
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
            }
        }
    }

    private void DownAttackFrame()
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
            }
        }
    }
}

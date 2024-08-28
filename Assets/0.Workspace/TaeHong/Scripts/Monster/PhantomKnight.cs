using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tae;
using System;

public class PhantomKnight : BaseMonster
{
    [Header("Attack Values")]
    [SerializeField] float pierceDistance;
    [SerializeField] float pierceDuration;
    [SerializeField] float pierceSpeed;

    [SerializeField] float chargeDistance;
    [SerializeField] float chargeDuration;
    [SerializeField] float chargeSpeed;

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
        else // 스킬 30%
        {
            random = UnityEngine.Random.Range(0f, 1f);
            if (random <= 0.67f) // 스킬1 67%
            {
                yield return StartCoroutine(Pierce());
            }
            else // 스킬2 33%
            {
                yield return StartCoroutine(Charge());
            }
        }

        // 공격후 잠깐 대기
        PlayAnim(Convert.ToByte(Animation.Idle));
        yield return new WaitForSeconds(1.5f);
        attackFinished = true;
    }

    // Poke
    public override void Attack01Frame()
    {
        BoxColliderAttackFrame(attack01Collider, 1);
    }

    // Pierce
    public override void Attack02Frame()
    {
        BoxColliderAttackFrame(attack02Collider, 1.5f);
    }

    // Charge
    public override void Attack03Frame()
    {
        BoxColliderAttackFrame(attack03Collider, 2);
    }

    private IEnumerator Pierce()
    {
        Vector2 startPos = transform.position;
        Vector3 direction = (playerToChase.position - transform.position).normalized;
        Vector2 targetPos;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, pierceDistance, LayerMask.GetMask("Wall"));
        if(hit.collider == null)
        {
            targetPos = direction * pierceDistance;
        }
        else
        {
            targetPos = hit.point - (Vector2)direction * 0.5f;
        }

        yield return new WaitForSeconds(0.333f);
        Flip(direction);
        PlayAnim(Convert.ToByte(Animation.Attack02));
        float t = 0;
        while(t < pierceDuration)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, t / pierceDuration);
            t += Time.deltaTime * pierceSpeed;
            yield return null;
        }
    }

    private IEnumerator Charge()
    {
        Vector2 startPos = transform.position;
        Vector3 direction = (playerToChase.position - transform.position).normalized;
        Vector2 targetPos;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, chargeDistance, LayerMask.GetMask("Wall"));
        if (hit.collider == null)
        {
            targetPos = direction * chargeDistance;
        }
        else
        {
            targetPos = hit.point - (Vector2) direction * 0.5f;
        }

        yield return new WaitForSeconds(0.333f);
        Flip(direction);
        PlayAnim(Convert.ToByte(Animation.Attack03));
        float t = 0;
        while (t < chargeDuration)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, t / chargeDuration);
            t += Time.deltaTime * chargeSpeed;
            yield return null;
        }
    }
}

using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;

// 사용시 6개의 효과중 하나
// 1. 이펙트만나오는 효과 4초
// 2. 변신효과(HP : 5, 디버프 모두 제거, 스킬, 공격 사용 금지)
// 3. 10초간 공격력 70% 증가
// 4. 캐릭터의 위치에서 범위 4정도 3초뒤에 공격력 200 짜리 데미지 줌 번개쏨
// 5. 5초간 좌우만 움직임
// 6. 넉백 2칸, 데미지 40 , 범위 4 정도
public class RandomScroll : BaseUseableItem
{
    [SerializeField] GameObject shinyparticle;
    [SerializeField] PlayerHealth health;
    [SerializeField] Animator snailanimator;
    [SerializeField] LineRenderer radius;
    [SerializeField] PhotonPlayerController player;
    [SerializeField] GameObject effectAniPrefab;
    GameObject curEffectAniObject;
    Animator effectAni;
    EffectTimer effectTimer;
    int MaxHealth;
    Collider2D[] collider = new Collider2D[10];
    Vector3 curPos;
    private void Start()
    {
        player = GetComponentInParent<PhotonPlayerController>();
        health = GetComponentInParent<PlayerHealth>();
        snailanimator = GetComponentInParent<Animator>();
        curEffectAniObject = Instantiate(effectAniPrefab, transform);
        effectAni = curEffectAniObject.GetComponent<Animator>();
        effectTimer = curEffectAniObject.GetComponent<EffectTimer>();
    }
    [ContextMenu("TestMod")]
    public override void UseItem()
    {
        if (IsCool)
        {
            base.UseItem();
            int random = Random.Range(0, 6);
            RandomEffect(2);
        }
    }

    private void RandomEffect(int Effect)
    {
        effectAni.enabled = true;
        switch (Effect)
        {
            case 0:
                Debug.Log("반짝반짝");
                StartCoroutine(ShinyScrollEffect());
                break;
            case 1:
                Debug.Log("달팽이");
                StartCoroutine(ApplySnailScroll());
                break;
            case 2:
                Debug.Log("공격력");
                StartCoroutine(ApplyAttackBoost());
                break;
            case 3:
                Debug.Log("전격");
                StartCoroutine(ActivateThunderScroll());
                break;
            case 4:
                Debug.Log("횡");
                StartCoroutine(LimitVerticalMovement());
                break;
            case 5:
                Debug.Log("엘더");
                StartCoroutine(ApplyKnockbackScroll());
                break;
        }
    }
    IEnumerator ShinyScrollEffect()
    {
        curEffectAniObject.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, 0);
        effectAni.SetBool("Shiny", true);
        yield return new WaitForSeconds(4f);
        effectAni.SetBool("Shiny", false);
        effectAni.enabled = false;
    }
    IEnumerator ApplySnailScroll()
    {
        // 디버프 제거도 나중에 해줘야됨
        // 공격 못하게 해야됨
        MaxHealth = health.MaxHealth;
        health.Health = 5;
        health.MaxHealth = 5;
        snailanimator.SetBool("Snail", true);
        yield return new WaitForSeconds(10f);
        health.Health = MaxHealth;
        health.MaxHealth = MaxHealth;
        snailanimator.SetBool("Snail", false);
    }
    IEnumerator ApplyAttackBoost()
    {
        Buff buff = new Buff("공격력 업", 10, StatType.Power, 0.7f, true);
        GetComponentInParent<StatusEffectManager>().AddEffect(buff);
        curEffectAniObject.transform.position = new Vector3(transform.position.x, transform.position.y + 2f, 0);
        effectAni.Play("AttackBoost");
        yield return new WaitForSeconds(10f);
        effectAni.enabled = false;
    }
    IEnumerator ActivateThunderScroll()
    {
        curPos = transform.position;
        // 이펙트 애니메이션 상속해제
        curEffectAniObject.transform.SetParent(null);
        // 범위 표시
        radius.enabled = true;
        radius.gameObject.SetActive(true);
        radius.SetPosition(0, new Vector3(transform.position.x + 2, transform.position.y, 0));
        radius.SetPosition(1, new Vector3(transform.position.x - 2, transform.position.y, 0));
        // 이벤트에 넣어줌
        effectTimer.effectDamageEvent += ThunderDamage;
        yield return new WaitForSeconds(3f);
        // 범위 해제
        radius.enabled = false;
        // 이펙트 표시
        effectAni.SetTrigger("Thunder");
        yield return new WaitForSeconds(1f);
        // 다시 상속후 위치 0,0,0으로 바꿔주기
        curEffectAniObject.transform.SetParent(transform);
        curEffectAniObject.transform.position = transform.position;
        effectAni.enabled = false;
    }
    private void ThunderDamage()
    {
        // 데미지
        int count = Physics2D.OverlapBoxNonAlloc(curPos, new Vector2(3, 3), 0, collider);
        for (int i = 0; i < count; i++)
        {
            if (collider[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(200);
            }
        }

    }
    IEnumerator LimitVerticalMovement()
    {
        // 이펙트 생성
        player.IsUpDown = false;
        yield return new WaitForSeconds(5f);
        player.IsUpDown = true;
        // 이펙트 제거
    }

    IEnumerator ApplyKnockbackScroll()
    {
        curPos = transform.position;
        // 이펙트 애니메이션 상속해제
        curEffectAniObject.transform.SetParent(null);
        // 범위 표시
        radius.enabled = true;
        radius.gameObject.SetActive(true);
        radius.SetPosition(0, new Vector3(transform.position.x + 2, transform.position.y, 0));
        radius.SetPosition(1, new Vector3(transform.position.x - 2, transform.position.y, 0));
        // 이벤트에 넣어줌
        effectTimer.effectDamageEvent += KnockeBackDamage;
        yield return new WaitForSeconds(1f);
        // 범위 제거
        radius.enabled = false;
        //이펙트 생성
        effectAni.SetTrigger("Knockback");
        yield return new WaitForSeconds(1.5f);
        // 다시 상속후 위치 0,0,0으로 바꿔주기
        curEffectAniObject.transform.SetParent(transform);
        curEffectAniObject.transform.position = transform.position;
        effectAni.enabled = false;
    }
    private void KnockeBackDamage()
    {
        Vector2 directionToEnemy;
        // 공격 데미지및 넉백
        int count = Physics2D.OverlapBoxNonAlloc(curPos, new Vector2(3, 3), 0, collider);
        for (int i = 0; i < count; i++)
        {
            if (collider[i].gameObject == health.gameObject)
                continue;

            if (collider[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(40);
            }
            if (collider[i].TryGetComponent(out IKnockbackable knockbackable))
            {
                directionToEnemy = (collider[i].transform.position - curPos).normalized;
                knockbackable.GetKnockedBack(directionToEnemy, 2f, .5f);
            }
        }

    }
}

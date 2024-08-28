using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SwordsmanAttack : MonoBehaviourPun, IAttackDisabler
{
    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] PlayerData data;
    [SerializeField] PlayerStatsManager stat;
    [SerializeField] PlayerStates state;
    [SerializeField] LineRenderer dashRangeLine;
    [SerializeField] LineRenderer dashAttackRangeLine;
    [SerializeField] StatusEffectManager statusEffect;
    [SerializeField] List<AudioClip> attackSounds;
    [SerializeField] List<AudioClip> SkillSounds;
    [SerializeField] List<AudioClip> dashSounds;
    [SerializeField] List<AudioClip> utiSounds;
    [SerializeField] AudioSource AudioSource;

    Animator curHitEffectAni;
    Animator curAttackEffectAni;
    [Header("Properties")]
    // 삼연창 취소 시간
    [SerializeField] float cancel;
    // 대쉬시 랜더러 만들어주기
    [SerializeField] LayerMask rayMask;
    [SerializeField] PooledObject hitEffectAnimator;
    [SerializeField] GameObject attacEffectAni;


    float endTime = 0.5f;

    int skillCount = 0;
    int curSkillCount;

    [Header("States")]
    bool isSkill = true;
    bool isUtimate = true;
    bool isDash = true;

    [Header("Cached Values")]
    Vector3 mousePos;
    Vector2 distanceToMouse;
    Vector2 directionToMouse;
    Collider2D[] colliders = new Collider2D[10];
    public void PlaySFX(AudioClip clip)
    {
        AudioSource.PlayOneShot(clip);
    }
    public void StopSFX()
    {
        if (AudioSource.isPlaying == false)
            return;

        AudioSource.Stop();
    }

    private void Awake()
    {
        curAttackEffectAni = Instantiate(attacEffectAni).GetComponent<Animator>();
    }
    public void HitAniPlay(string name, Vector2 endPos)
    {
        PooledObject effect = Manager.Pool.GetPool(hitEffectAnimator, endPos, Quaternion.identity);
        curHitEffectAni = effect.gameObject.GetComponent<Animator>();

        curHitEffectAni.transform.position = endPos;
        curHitEffectAni.Play(name);
    }
    private void MouseCheck()
    {
        // mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distanceToMouse = ((Vector2) mousePos - (Vector2) transform.position);
        directionToMouse = ((Vector2) mousePos - (Vector2) transform.position).normalized;
    }
    [PunRPC]
    private void SwordsmanAttackRPC(Vector2 mousePos)
    {
        this.mousePos = mousePos;
        MouseCheck();
        StartCoroutine(AttackCoolTime());
    }
    [PunRPC]
    private void SwordsmanSkillRPC(Vector2 mousePos)
    {
        this.mousePos = mousePos;
        MouseCheck();
        if (skillCount == 0)
        {
            StartCoroutine(TripleSpear("Skill", stat.curPower * data.skillDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Skill]],
               data.skillRange, data.skillStartupTime, data.skillCooldownTime));
        }
        else if (skillCount == 1)
        {
            StartCoroutine(TripleSpear("Skill1", stat.curPower * (data.skillDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Skill]] + 0.1f),
               data.skillRange, data.skillStartupTime, data.skillCooldownTime));
        }
        else if (skillCount == 2)
        {
            StartCoroutine(TripleSpear("Skill", stat.curPower * (data.skillDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Skill]] + 0.3f),
                data.skillRange, data.skillStartupTime * 2, data.skillCooldownTime * 4));
        }
    }
    [PunRPC]
    private void SwordsmanUtiRPC()
    {
        Utimate();
        random = Random.Range(0, 2);
    }
    [PunRPC]
    private void SwordsmanDashAniRPC()
    {
        StartCoroutine(Charging());
    }

    int random;
    private void OnAttack()
    {
        if (!state.IsAttack)
            return;

        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (PhotonNetwork.IsConnected)
                photonView.RPC("SwordsmanAttackRPC", RpcTarget.AllViaServer, input);
            else
                SwordsmanAttackRPC(input);
        }

    }

    IEnumerator AttackCoolTime()
    {
        state.Stop();
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        curAttackEffectAni.transform.rotation = rotation;
        random = Random.Range(0, 5);
        PlaySFX(attackSounds[random]);
        animator.SetTrigger("Attack");
        curAttackEffectAni.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
        curAttackEffectAni.Play("Attack");
        yield return new WaitForSeconds(data.attStartupTime);
        Collider2D[] enemys = new Collider2D[10];
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            enemys = PlayerUtils.AttackCheck(180, stat.curPower * data.attDamageMultiplier[0], data.attRange, stat.curCritical, stat.curCriticalMultiplier, 0, gameObject, colliders);
        }
        if (enemys != null)
        {
            for (int i = 0; i < enemys.Length; i++)
            {
                if (enemys[i] == null)
                    continue;
                if (enemys[i].CompareTag("Player") && enemys[i].TryGetComponent(out PhotonView photonView))
                {
                    // 팀전
                    if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom.GetGameMode() == false)
                    {
                        Team myTeam = gameObject.GetComponent<PhotonView>().Owner.GetTeam();
                        if (photonView.Owner.GetTeam() == myTeam)
                            continue;
                    }
                    else // 1:1
                    {
                        if (colliders[i].gameObject == gameObject)
                            continue;
                    }
                }

                if (enemys[i].TryGetComponent(out IDamageable damageable))
                {
                    PlaySFX(attackSounds[random + 5]);
                    HitAniPlay("Attack", enemys[i].transform.position);
                }
            }
        }
        yield return new WaitForSeconds(data.attCooldownTime);
        //state.ChangeState(PlayerState.Idle);
        state.AttackNomal();
        yield return new WaitForSeconds(data.attCoolTime[0]);

    }

    private void OnSkill()
    {
        if (!state.IsSkill)
            return;
        if (!state.IsAttack)
            return;
        if (stat.SkillLevelUpDatas[SkillType.Skill] == 0)
            return;
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (PhotonNetwork.IsConnected)
                photonView.RPC("SwordsmanSkillRPC", RpcTarget.AllViaServer, input);
            else
                SwordsmanSkillRPC(input);
        }
    }
    IEnumerator TripleSpear(string aniName, float Damage, float Range, float skillStartupTime, float skillCooldownTime)
    {
        if (isSkill)
        {
            PlaySFX(SkillSounds[random]);
            float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            curAttackEffectAni.transform.rotation = rotation;
            random = Random.Range(0, 6);
            curAttackEffectAni.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
            curAttackEffectAni.Play("Skill");
            state.Stop();
            skillCount++;
            curSkillCount = skillCount;
            yield return new WaitForSeconds(skillStartupTime);
            animator.SetBool(aniName, true);
            Collider2D[] enemys = PlayerUtils.AttackCheck(180, Damage, Range, stat.curCritical, stat.curCriticalMultiplier, 1, gameObject, colliders); // 데미지랑 거리 바꿔줘야됨
            if (enemys != null)
            {
                for (int i = 0; i < enemys.Length; i++)
                {
                    if (enemys[i] == null)
                        continue;

                    if (enemys[i].CompareTag("Player") && enemys[i].TryGetComponent(out PhotonView photonView))
                    {
                        // 팀전
                        if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom.GetGameMode() == false)
                        {
                            Team myTeam = gameObject.GetComponent<PhotonView>().Owner.GetTeam();
                            if (photonView.Owner.GetTeam() == myTeam)
                                continue;
                        }
                        else // 1:1
                        {
                            if (colliders[i].gameObject == gameObject)
                                continue;
                        }
                    }

                    if (enemys[i].TryGetComponent(out IDamageable damageable))
                    {
                        HitAniPlay("Skill", enemys[i].transform.position);
                        PlaySFX(SkillSounds[random + 6]);
                    }
                }
            }

            Forward(Range);
            yield return new WaitForSeconds(skillCooldownTime);
            animator.SetBool(aniName, false);
            state.Normal();
            if (skillCount == 3)
            {
                isSkill = false;
                yield return new WaitForSeconds(data.skillCoolTime[stat.SkillLevelUpDatas[SkillType.Skill]]);
                isSkill = true;
                skillCount = 0;
                yield return null;
            }

            yield return new WaitForSeconds(cancel);
            if (curSkillCount == skillCount)
            {
                isSkill = false;
                yield return new WaitForSeconds(data.skillCoolTime[stat.SkillLevelUpDatas[SkillType.Skill]]);
                isSkill = true;
                skillCount = 0;
            }
            yield return null;

        }
    }
    Vector2 newPos;
    private void Forward(float range)
    {
        if (PlayerUtils.countEnemy == 0)
        {
            MouseCheck();
            newPos = PlayerUtils.CheckForCollision(transform.position, directionToMouse, data.skillRange, rayMask); ;
            transform.position = Vector2.MoveTowards(transform.position, newPos, 3);
        }
    }

    private void OnUtimate()
    {
        if (stat.SkillLevelUpDatas[SkillType.Utimate] == 0)
            return;

        if ( isUtimate)
        {
            if (PhotonNetwork.IsConnected && photonView.IsMine)
                photonView.RPC("SwordsmanUtiRPC", RpcTarget.AllViaServer);
            else if(!PhotonNetwork.IsConnected)
                SwordsmanUtiRPC();
        }
    }
    private void Utimate()
    {
        if (state.IsAttack && state.IsSkill)
        {
            MouseCheck();
            StartCoroutine(UtiDash());
        }
    }

    IEnumerator UtiDash()
    {
        if (isUtimate)
        {
            state.Stop();
            isUtimate = false;
            Vector2 dashEndPos = PlayerUtils.CheckForCollision(transform.position, directionToMouse, data.utiRange, rayMask);
            Vector2 dashMousePos = PlayerUtils.CheckForCollision(transform.position, directionToMouse, distanceToMouse.magnitude, rayMask);
            float stratTime = 0f;
            yield return new WaitForSeconds(data.utiStartupTime);
            while (stratTime < endTime)
            {
                stratTime += Time.deltaTime;
                float t = stratTime / endTime;
                if (distanceToMouse.sqrMagnitude < data.utiRange * data.utiRange)
                {
                    Vector2 newPosition = Vector2.Lerp(transform.position, dashMousePos, t);
                    transform.position = newPosition;
                }
                else
                {
                    Vector2 newPosition = Vector2.Lerp(transform.position, dashEndPos, t);
                    transform.position = newPosition;
                }
                yield return null;
            }
            animator.SetTrigger("Utimate");
            PlaySFX(utiSounds[random]);
            Collider2D[] enemys = PlayerUtils.AttackCheck(360, stat.curPower * data.utiDmageMultiplier[stat.SkillLevelUpDatas[SkillType.Utimate]], data.utiRange, stat.curCritical, 1f, stat.curCriticalMultiplier, gameObject, colliders);
            if (enemys != null)
            {
                for (int i = 0; i < enemys.Length; i++)
                {
                    if (enemys[i] == null)
                        continue;
                    if (enemys[i].CompareTag("Player") && enemys[i].TryGetComponent(out PhotonView photonView))
                    {
                        // 팀전
                        if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom.GetGameMode() == false)
                        {
                            Team myTeam = gameObject.GetComponent<PhotonView>().Owner.GetTeam();
                            if (photonView.Owner.GetTeam() == myTeam)
                                continue;
                        }
                        else // 1:1
                        {
                            if (colliders[i].gameObject == gameObject)
                                continue;
                        }
                    }

                    if (enemys[i].TryGetComponent(out IDamageable damageable))
                    {
                        HitAniPlay("Uti", enemys[i].transform.position);
                        PlaySFX(utiSounds[2]);
                    }
                }
            }

            KnockBack();
            yield return new WaitForSeconds(data.utiCooldownTime);
            state.Normal();
            yield return new WaitForSeconds(data.utiCoolTime[stat.SkillLevelUpDatas[SkillType.Utimate]]);
            isUtimate = true;
        }
    }
    private void KnockBack()
    {
        Vector2 directionToEnemy;
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, data.utiRange, colliders);
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].TryGetComponent(out IKnockbackable knockbackable))
            {
                if (colliders[i].gameObject == gameObject)
                    continue;
                directionToEnemy = (colliders[i].transform.position - transform.position).normalized;
                knockbackable.GetKnockedBack(directionToEnemy, 2f, .5f);
            }
        }
    }
    IEnumerator KnockBackRoutine(Collider2D Enemy, Vector2 EndPos)
    {
        float startTime = 0f;

        while (startTime < endTime)
        {
            startTime += Time.deltaTime;
            float t = startTime / endTime;
            Enemy.transform.position = Vector2.Lerp(Enemy.transform.position, EndPos, t);
            yield return null;
        }
    }

    private void OnDash()
    {
        if (stat.SkillLevelUpDatas[SkillType.Dash] == 0)
            return;

        if (photonView.IsMine && isDash && state.IsAttack && state.IsSkill ||
            !PhotonNetwork.IsConnected && isDash && state.IsAttack && state.IsSkill) // 포톤 연결 안되어 있을 때
        {
            isDash = false;
            StartCoroutine(Charging());
        }
    }
    float chargingTime;
    float maxCharging = 1;
    [SerializeField] float dashRange; // 대쉬 거리
    Vector2 dashEndPos; // 대쉬 종료 위치
    Coroutine dashRoutine;
    IEnumerator Charging()
    {
        state.Stop();
        animator.SetBool("Charging", true);
        PlaySFX(dashSounds[0]);
        while (true)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MouseCheck();
            chargingTime += Time.deltaTime;
            chargingTime = Mathf.Clamp(chargingTime, 0f, maxCharging);
            RangeRendererControl(directionToMouse);
            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (photonView.IsMine || !PhotonNetwork.IsConnected)
                {
                    dashAttackRangeLine.enabled = false;
                    dashRangeLine.enabled = false;
                    dashEndPos = PlayerUtils.CheckForCollision(transform.position, directionToMouse, chargingTime * dashRange, rayMask);
                    yield return new WaitForSeconds(data.dashStartupTime);
                    dashRoutine = StartCoroutine(Dash());
                    chargingTime = 0;
                    yield return new WaitForSeconds(data.dashCoolTime[stat.SkillLevelUpDatas[SkillType.Dash]]);
                    isDash = true;
                    yield break;
                }
            }
            float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            curAttackEffectAni.transform.rotation = rotation;
            yield return null;

        }
    }
    private IEnumerator Dash()
    {
        animator.SetBool("Charging", false);
        float startTime = 0f;
        curAttackEffectAni.Play("Dash");
        PlaySFX(SkillSounds[2]);
        while (startTime < endTime)
        {
            Dashing();
            startTime += Time.deltaTime;
            float t = startTime / endTime;
            Vector2 newPosition = Vector2.Lerp(transform.position, dashEndPos, t);
            curAttackEffectAni.transform.position = newPosition;
            transform.position = newPosition;
            animator.SetTrigger("Dash");
            yield return null;
        }
        StopSFX();
        PlaySFX(SkillSounds[1]);
        Damageables.Clear();
        // 대쉬가 끝났을때 대미지 줘야됨
        Collider2D[] enemys = PlayerUtils.AttackCheck(360, stat.curPower * data.dashDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Dash]], data.dashRange, data.critical, data.critMultiplier, 99f, gameObject, colliders);
        if (enemys != null)
        {
            for (int i = 0; i < enemys.Length; i++)
            {
                if (enemys[i] == null)
                    continue;
                if (enemys[i].CompareTag("Player") && enemys[i].TryGetComponent(out PhotonView photonView))
                {
                    // 팀전
                    if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom.GetGameMode() == false)
                    {
                        Team myTeam = gameObject.GetComponent<PhotonView>().Owner.GetTeam();
                        if (photonView.Owner.GetTeam() == myTeam)
                            continue;
                    }
                    else // 1:1
                    {
                        if (colliders[i].gameObject == gameObject)
                            continue;
                    }
                }

                if (enemys[i].TryGetComponent(out IDamageable damageable))
                    HitAniPlay("dash2", enemys[i].transform.position);
            }
        }
        curAttackEffectAni.Play("dash2");
        state.Normal();
        // 여기서 멀리 보내야됨
        StopCoroutine(dashRoutine);
        dashRoutine = null;

    }
    // 키값으로 저장되어 있어 빠르게 찾을수있다.
    HashSet<IDamageable> Damageables = new HashSet<IDamageable>();
    // 대쉬범위에 있으면 데려가기  
    private void Dashing()
    {
        int random = Random.Range(-1, 1);
        Vector2 size = new Vector2(1f, 1f);
        int count = Physics2D.OverlapBoxNonAlloc(transform.position, size, 360, colliders);
        float damage = PlayerUtils.Critical(stat.curPower * 0.2f, data.critMultiplier, data.critMultiplier);
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].gameObject == gameObject)
                continue;

            if (colliders[i].TryGetComponent(out IDamageable damageable))
            {
                if (!Damageables.Contains(damageable))
                {
                    // 여기서 방어력 계산해줘야됨
                    //colliders[i].transform.position = new Vector2(dashEndPos.x + random, dashEndPos.y + random);
                    damageable.TakeDamage((int) damage);
                    colliders[i].transform.position = dashEndPos + new Vector2(random, random);
                    Damageables.Add(damageable);
                    HitAniPlay("Dash", colliders[i].transform.position);
                }
            }
        }
    }
    private void RangeRendererControl(Vector2 direction)
    {
        dashAttackRangeLine.enabled = true;
        dashRangeLine.enabled = true;

        dashRangeLine.positionCount = 2;
        dashRangeLine.SetPosition(0, transform.position);
        dashRangeLine.SetPosition(1, PlayerUtils.CheckForCollision(transform.position, direction, chargingTime * dashRange, rayMask));
        dashAttackRangeLine.positionCount = 2;
        dashAttackRangeLine.widthMultiplier = 2 * data.dashRange;
        dashAttackRangeLine.SetPosition(0, PlayerUtils.CheckForCollision(transform.position, direction, chargingTime * dashRange, rayMask));
        dashAttackRangeLine.SetPosition(1, PlayerUtils.CheckForCollision(transform.position, direction, chargingTime * dashRange, rayMask));
    }

}

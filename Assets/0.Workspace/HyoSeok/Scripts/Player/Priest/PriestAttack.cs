using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;

public class PriestAttack : MonoBehaviourPun, IAttackDisabler
{
    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] PlayerData data;
    [SerializeField] PlayerStatsManager stat;
    [SerializeField] PlayerStates state;
    [SerializeField] StatusEffectManager statusEffect;
    [SerializeField] LayerMask layerMask;
    [SerializeField] GameObject UtimateRange;
    [SerializeField] List<AudioClip> attackSounds;
    [SerializeField] List<AudioClip> SkillSounds;
    [SerializeField] List<AudioClip> dashSounds;
    [SerializeField] List<AudioClip> utiSounds;
    [SerializeField] AudioSource AudioSource;

    PriestAttackEffect curPrefab;
    PlayerHealth health;
    float maxChargeTime = 1f;
    float maxDamageMultiplier = 0.8f;
    float minDamageMultiplier;

    [Header("Cached Values")]
    Vector3 mousePos;
    Vector2 distanceToMouse;
    Vector2 directionToMouse;
    Collider2D[] colliders = new Collider2D[10];


    [Header("Prefab")]
    [SerializeField] PooledObject skillPrefab;
    [SerializeField] PooledObject hitEffectAnimator;
    [SerializeField] GameObject attacEffectAni;

    public Animator curHitEffectAni;
    public Animator curAttackEffectAni;
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

    public void HitSoundPlay(int hitSound)
    {
        PlaySFX(SkillSounds[hitSound]);
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
    [PunRPC]
    public void PriestAttackRPC(Vector2 mousePos)
    {
        this.mousePos = mousePos;
        MouseCheck();
        StartCoroutine(AttackCoolTime());
    }

    [PunRPC]
    public void PriestSkillRPC(Vector2 mousePos, int seed, int damage)
    {
        this.mousePos = mousePos;
        this.seed = seed;
        MouseCheck();
        StartCoroutine(SkillCoolTime(damage));
        Earthquake();
    }
    [PunRPC]
    public void PriestDashRPC(Vector2 mousePos)
    {
        this.mousePos = mousePos;
        MouseCheck();
        StartCoroutine(DashCool());
    }
    [PunRPC]
    public void PriestUtiRPC()
    {
        StartCoroutine(UtimateCool());
    }


    private void Start()
    {
        minDamageMultiplier = data.attDamageMultiplier[0];
        health = GetComponent<PlayerHealth>();
    }
    private void MouseCheck()
    {
        //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distanceToMouse = ((Vector2) mousePos - (Vector2) transform.position);
        directionToMouse = ((Vector2) mousePos - (Vector2) transform.position).normalized;
    }
    int seed;
    private void OnAttack()
    {
        if (!state.IsAttack)
            return;

        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            photonView.RPC("PriestAttackRPC", RpcTarget.AllViaServer, input);
        }
    }
    IEnumerator AttackCoolTime()
    {
        float tiem = 0;
        state.MoveAttack();
        animator.SetTrigger("Chargeing");
        PlaySFX(attackSounds[1]);
        SpriteRenderer attackRenderer = curAttackEffectAni.GetComponent<SpriteRenderer>();
        while (true)
        {
            tiem += Time.deltaTime;
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                animator.SetTrigger("Attack");
                // 0.3초 전이면 기본 데미지
                if (tiem < 0.3f)
                {
                    StopSFX();
                    PlaySFX(attackSounds[0]);
                    yield return new WaitForSeconds(data.attStartupTime);
                    if (directionToMouse.x < 0)
                        attackRenderer.flipX = true;
                    else
                        attackRenderer.flipX = false;
                    curAttackEffectAni.transform.position = new Vector2(transform.position.x + directionToMouse.x, transform.position.y + directionToMouse.y);
                    Debug.Log(curAttackEffectAni.transform.position);
                    curAttackEffectAni.Play("PriestAttack");
                    Collider2D[] enemys = PlayerUtils.AttackCheck(180, stat.curPower * minDamageMultiplier, data.attRange, stat.curCritical, stat.curCriticalMultiplier, 0, gameObject, colliders);
                    if (enemys != null)
                    {
                        for (int i = 0; i < enemys.Length; i++)
                        {
                            if (enemys[i] == null)
                                continue;
                            if (enemys[i].CompareTag("Player") && enemys[i].TryGetComponent(out PhotonView photonView))
                            {
                                Team myTeam = gameObject.GetComponent<PhotonView>().Owner.GetTeam();
                                if (photonView.Owner.GetTeam() == myTeam)
                                    continue;
                            }

                            if (enemys[i].TryGetComponent(out IDamageable damageable))
                            {
                                HitAniPlay("PriestAttack", enemys[i].transform.position);
                                Manager.Sound.PlaySFX(attackSounds[3]);
                            }
                        }
                    }
                    yield return new WaitForSeconds(data.attCooldownTime);
                    yield return new WaitForSeconds(data.attCoolTime[0]);
                    state.Normal();
                    yield break;

                }
                // 1초까지 시간에 비례해 데미지 변경(스턴 넣어주셈)
                else if (tiem < 1f)
                {
                    if (directionToMouse.x < 0)
                        attackRenderer.flipX = true;
                    else
                        attackRenderer.flipX = false;
                    curAttackEffectAni.transform.position = transform.position;
                    Manager.Sound.StopSFX();
                    curAttackEffectAni.Play("PriestAttack");
                    Manager.Sound.PlaySFX(attackSounds[0]);
                    float chargeRatio = Mathf.Clamp01(tiem / maxChargeTime);
                    float damage = chargeRatio * stat.curPower * maxDamageMultiplier;
                    yield return new WaitForSeconds(data.attStartupTime);
                    Collider2D[] enemys = PlayerUtils.AttackCheck(180, damage, data.attRange, stat.curCritical, stat.curCriticalMultiplier, 0, gameObject, colliders);
                    if (enemys != null)
                    {
                        for (int i = 0; i < enemys.Length; i++)
                        {
                            if (enemys[i] == null)
                                continue;
                            if (enemys[i].CompareTag("Player") && enemys[i].TryGetComponent(out PhotonView photonView))
                            {
                                Team myTeam = gameObject.GetComponent<PhotonView>().Owner.GetTeam();
                                if (photonView.Owner.GetTeam() == myTeam)
                                    continue;
                            }

                            if (enemys[i].TryGetComponent(out IDamageable damageable))
                            {
                                HitAniPlay("PriestAttack", enemys[i].transform.position);
                                Manager.Sound.PlaySFX(attackSounds[3]);
                            }
                        }
                    }
                    yield return new WaitForSeconds(data.attCooldownTime);
                    yield return new WaitForSeconds(data.attCoolTime[0]);
                    state.Normal();
                    yield break;

                }
                // 1초 이후에는 계속 같은 데미지(스턴 넣어주셈)
                else
                {
                    if (directionToMouse.x < 0)
                    {
                        attackRenderer.flipX = true;
                        curAttackEffectAni.transform.position = new Vector2(transform.position.x - 2, transform.position.y);
                    }
                    else
                    {
                        attackRenderer.flipX = false;
                        curAttackEffectAni.transform.position = new Vector2(transform.position.x + 2, transform.position.y);

                    }
                    Manager.Sound.StopSFX();
                    Manager.Sound.PlaySFX(attackSounds[2]);
                    curAttackEffectAni.Play("PriestChargingAttack");
                    yield return new WaitForSeconds(data.attStartupTime);
                    Collider2D[] enemys = PlayerUtils.AttackCheck(180, stat.curPower * maxDamageMultiplier, data.attRange, stat.curCritical, stat.curCriticalMultiplier, 0.3f, gameObject, colliders);
                    if (enemys != null)
                    {
                        for (int i = 0; i < enemys.Length; i++)
                        {
                            if (enemys[i] == null)
                                continue;
                            if (enemys[i].CompareTag("Player") && enemys[i].TryGetComponent(out PhotonView photonView))
                            {
                                Team myTeam = gameObject.GetComponent<PhotonView>().Owner.GetTeam();
                                if (photonView.Owner.GetTeam() == myTeam)
                                    continue;
                            }

                            if (enemys[i].TryGetComponent(out IDamageable damageable))
                            {
                                HitAniPlay("PriestChargingAttack", enemys[i].transform.position);
                                Manager.Sound.PlaySFX(attackSounds[4]);
                            }
                        }
                    }
                    yield return new WaitForSeconds(data.attCooldownTime);
                    yield return new WaitForSeconds(data.attCoolTime[0]);
                    state.Normal();
                    yield break;

                }
            }
            attackRenderer.flipX = false;
            yield return null;
        }
    }
    private void OnSkill()
    {
        if (!state.IsSkill)
            return;
        if (!state.IsAttack)
            return;
        if (stat.SkillLevelUpDatas[SkillType.Skill] == 0)
            return;

        if (photonView.IsMine && isSkill || !PhotonNetwork.IsConnected)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            photonView.RPC("PriestSkillRPC", RpcTarget.AllViaServer, input, seed, data.skillDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Skill]]);

        }
    }
    bool isSkill = true;
    IEnumerator SkillCoolTime(int damage)
    {
        isSkill = false;
        animator.SetTrigger("Skill");
        yield return new WaitForSeconds(data.skillStartupTime);
        CreatePrefab(skillPrefab);
        FirePrefab(curPrefab, damage, data.skillRange);
        yield return new WaitForSeconds(data.skillCoolTime[stat.SkillLevelUpDatas[SkillType.Skill]]);
        isSkill = true;
    }
    private void CreatePrefab(PooledObject Prefab)
    {
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        Vector3 PrefabPos = new Vector3(transform.position.x, transform.position.y + 0.5f, 0);

        PooledObject effect = Manager.Pool.GetPool(Prefab, PrefabPos, rotation);
        curPrefab = effect.gameObject.GetComponent<PriestAttackEffect>();
    }

    private void FirePrefab(PriestAttackEffect arrow, float SkillMultiplier, float range)
    {
        arrow.damage = PlayerUtils.Critical(stat.curPower * SkillMultiplier, stat.curCritical, stat.curCriticalMultiplier);
        arrow.StartFire(transform.position, (Vector2) transform.position + directionToMouse * range, photonView.Owner, gameObject, "PriestSkill");
    }
    private void Earthquake()
    {
        curAttackEffectAni.transform.position = transform.position;
        Manager.Sound.PlaySFX(SkillSounds[0]);
        curAttackEffectAni.Play("PriestSkill");
        int count = Physics2D.OverlapBoxNonAlloc(transform.position, Vector2.one * 4f, 360f, colliders);
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].gameObject == gameObject)
                continue;

            if (colliders[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage((int) PlayerUtils.Critical(stat.curPower * 0.25f, data.critical, data.critMultiplier));
                Manager.Sound.PlaySFX(SkillSounds[1]);
            }
        }
    }

    bool isDash = true;
    private void OnDash()
    {
        if (!state.IsSkill)
            return;
        if (!state.IsAttack)
            return;
        if (stat.SkillLevelUpDatas[SkillType.Dash] == 0)
            return;

        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            photonView.RPC("PriestDashRPC", RpcTarget.AllViaServer, input);
        }
    }
    private void Teleport()
    {
        Manager.Sound.PlaySFX(dashSounds[1]);
        // 이거 하기전에 체크부터 해야됨 어떻게?
        if (data.dashRange > distanceToMouse.magnitude)
        {
            if (PlayerUtils.CheckForCollision(transform.position, directionToMouse, distanceToMouse.magnitude, layerMask) != null)
            {
                transform.position = PlayerUtils.CheckForCollision(transform.position, directionToMouse, distanceToMouse.magnitude, layerMask);
            }
            else
            {
                transform.position = (Vector2) mousePos;
            }
        }
        else
        {
            if (PlayerUtils.CheckForCollision(transform.position, directionToMouse, data.dashRange, layerMask) != null)
            {
                transform.position = PlayerUtils.CheckForCollision(transform.position, directionToMouse, data.dashRange, layerMask);
            }
            else
            {
                transform.position = (Vector2) transform.position + directionToMouse * data.dashRange;
            }
        }
    }
    IEnumerator DashCool()
    {
        if (isDash)
        {
            isDash = false;
            curAttackEffectAni.Play("PriestDash");
            Teleport();
            TeleportDamage();
            TeleportHeal();
            yield return new WaitForSeconds(data.dashCoolTime[stat.SkillLevelUpDatas[SkillType.Dash]]);
            isDash = true;
            yield break;
        }
    }
    int enemyCount;
    PlayerHealth myTeamHealth;
    private void TeleportDamage()
    {
        SilenceDebuff silence = new SilenceDebuff("침묵", 2);
        int enemyCount = Physics2D.OverlapCircleNonAlloc(transform.position, data.dashRange, colliders);
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 directionToEnemy = (colliders[i].transform.position - transform.position).normalized;
            if (colliders[i].CompareTag("Player") && colliders[i].TryGetComponent(out PhotonView photonView))
            {
                Team myTeam = gameObject.GetComponent<PhotonView>().Owner.GetTeam();
                if (photonView.Owner.GetTeam() == myTeam)
                {
                    myTeamHealth = photonView.gameObject.GetComponent<PlayerHealth>();
                    continue;
                }
            }
            if (colliders[i].TryGetComponent(out IDamageable damageable))
            {
                StartCoroutine(TeleportDotDamage(damageable, colliders[i]));
                colliders[i].TryGetComponent(out StatusEffectManager statusEffectManager);
                statusEffectManager.AddEffect(silence);
            }

        }
    }
    private IEnumerator TeleportDotDamage(IDamageable damageable, Collider2D collider)
    {
        int count = 0;
        while (count < 4)
        {
            HitAniPlay("PriestDash", collider.transform.position);
            damageable.TakeFixedDamage((int) (stat.curPower * data.dashDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Dash]]));
            Manager.Sound.PlaySFX(dashSounds[3]);
            yield return new WaitForSeconds(1f);
            count++;
        }
        yield break;
    }
    private void TeleportHeal()
    {
        if (enemyCount < 3)
        {
            Manager.Sound.PlaySFX(dashSounds[2]);
            HitAniPlay("PriestHeal", transform.position);
            health.Heal(stat.MaxHp * (enemyCount + 1) * 0.1f, false);
            myTeamHealth.Heal(stat.MaxHp * (enemyCount + 1) * 0.1f, false);
        }
        else
        {
            Manager.Sound.PlaySFX(dashSounds[2]);
            HitAniPlay("PriestHeal", myTeamHealth.transform.position);
            health.Heal(stat.MaxHp * 3 * 0.1f, false);
            myTeamHealth.Heal(stat.MaxHp * (enemyCount + 1) * 0.1f, false);
        }
    }
    bool isUtimate = true;
    private void OnUtimate()
    {
        if (!state.IsSkill)
            return;
        if (!state.IsAttack)
            return;
        if (stat.SkillLevelUpDatas[SkillType.Utimate] == 0)
            return;

        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            photonView.RPC("PriestUtiRPC", RpcTarget.AllViaServer);
        }
    }

    private void UtimateDamage()
    {
        int enemyCount = Physics2D.OverlapCircleNonAlloc(transform.position, data.utiRange, colliders);
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 directionToEnemy = (colliders[i].transform.position - transform.position).normalized;
            if (colliders[i].gameObject == gameObject)
                continue;
            if (colliders[i].TryGetComponent(out IDamageable damageable))
            {
                HitAniPlay("PriestUti", new Vector2(colliders[i].transform.position.x, colliders[i].transform.position.y + 2));
                damageable.TakeDamage((int) PlayerUtils.Critical(stat.curPower * data.utiDmageMultiplier[stat.SkillLevelUpDatas[SkillType.Utimate]]
                    , stat.curCritical, stat.curCriticalMultiplier * 1.2f));
                Manager.Sound.PlaySFX(utiSounds[1]);
            }

        }

    }
    private IEnumerator UtimateCool()
    {
        if (isUtimate)
        {
            Manager.Sound.PlaySFX(utiSounds[0]);
            isUtimate = false;
            state.Stop();
            animator.SetTrigger("Utimate");
            UtimateRange.SetActive(true);
            yield return new WaitForSeconds(data.utiStartupTime);
            UtimateRange.SetActive(false);
            UtimateDamage();
            yield return new WaitForSeconds(data.utiCooldownTime);
            state.Normal();
            yield return new WaitForSeconds(data.utiCoolTime[stat.SkillLevelUpDatas[SkillType.Utimate]]);
            isUtimate = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ArcherAttack : MonoBehaviourPun, IAttackDisabler
{
    [SerializeField] Animator animator;
    [SerializeField] PlayerData data;
    [SerializeField] PlayerStatsManager stat;
    [SerializeField] PlayerStates states;
    [SerializeField] LayerMask rayMask;
    [SerializeField] List<AudioClip> attackSounds;
    [SerializeField] List<AudioClip> SkillSounds;
    [SerializeField] List<AudioClip> dashSounds;
    [SerializeField] List<AudioClip> utiSounds;
    [SerializeField] List<AudioClip> HitSound;
    [SerializeField] AudioSource AudioSource;

    GameObject effectGameObject;

    bool isAttack = true;
    bool isUtimate = true;
    bool isSkill = true;
    bool isDash = true;

    float endTime = 0.5f;
    int seed;
    public Animator curEffectAni;
    public Animator curAttackEffectAni;
    ArrowController curArrow; // 생성된 프리팹
    // 생성해야될 프리팹
    [SerializeField] PooledObject attackPrefab;
    [SerializeField] PooledObject utiPrefab;
    [SerializeField] PooledObject skillPrefab;
    [SerializeField] PooledObject effectAnimator;
    [SerializeField] GameObject attacEffectAni;
    int random;
    Vector3 mousePos;
    Vector2 distanceToMouse;
    Vector2 direction;
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

    public void SFXPlay(int i)
    {
        PlaySFX(HitSound[i]);
    }
    private void Awake()
    {
        curAttackEffectAni = Instantiate(attacEffectAni).GetComponent<Animator>();
    }
    public void HitAniPlay(string name, Vector2 endPos)
    {
        PooledObject effect = Manager.Pool.GetPool(effectAnimator, endPos, Quaternion.identity);
        curEffectAni = effect.gameObject.GetComponent<Animator>();

        curEffectAni.transform.position = endPos;
        curEffectAni.Play(name);
    }
    private void MouseCheck()
    {
        //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distanceToMouse = ((Vector2) mousePos - (Vector2) transform.position);
        direction = ((Vector2) mousePos - (Vector2) transform.position).normalized;
    }
    Vector3 arrowPos;
    #region RPCs
    [PunRPC]
    public void AttackRPC(Vector2 mousePos)
    {
        this.mousePos = mousePos;
        MouseCheck();
        StartCoroutine(AttackCoolTime());
    }

    [PunRPC]
    public void ArcherSkill(Vector2 mousePos, int seed, int damage)
    {
        this.mousePos = mousePos;
        this.seed = seed;
        MouseCheck();
        StartCoroutine(SkillCool(damage));
    }

    [PunRPC]
    public void ArcherUltimate(Vector2 mousePos, int damage)
    {
        this.mousePos = mousePos;
        MouseCheck();
        StartCoroutine(UtiCharging(damage));
    }
    #endregion

    private void CreateArrow(PooledObject arrowPrefab)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        arrowPos = new Vector3(transform.position.x, transform.position.y + 0.5f, 0);

        PooledObject effect = Manager.Pool.GetPool(arrowPrefab, arrowPos, rotation);
        curArrow = effect.gameObject.GetComponent<ArrowController>();
    }

    private void FireArrow(ArrowController arrow, float SkillMultiplier, float range, string name, int sound)
    {
        arrow.damage = PlayerUtils.Critical(stat.curPower * SkillMultiplier, stat.curCritical, stat.curCriticalMultiplier);
        arrow.StartFireArrow(arrowPos, (Vector2) transform.position + direction * range, photonView.Owner, gameObject, name, sound);
    }

    private void OnAttack()
    {
        // 서버없는 디버그용
        if (!PhotonNetwork.IsConnected)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            AttackRPC(input);
            return;
        }

        // 서버용
        if (photonView.IsMine && states.IsMovingAttack)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            photonView.RPC("AttackRPC", RpcTarget.AllViaServer, input);
        }
    }

    IEnumerator AttackCoolTime()
    {
        if (!states.IsAttack)
            yield break;
        if (isAttack)
        {
            random = Random.Range(0, 4);
            PlaySFX(attackSounds[random]);
            isAttack = false;
            animator.SetTrigger("Attack");
            CreateArrow(attackPrefab);
            FireArrow(curArrow, data.attDamageMultiplier[0], data.attRange, "ArcherAttack", 0);
            yield return new WaitForSeconds(data.attCoolTime[0]);
            isAttack = true;
        }
    }

    private void OnUtimate()
    {
        if (stat.SkillLevelUpDatas[SkillType.Utimate] == 0)
            return;
        if (!PhotonNetwork.IsConnected)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ArcherUltimate(input, (int) data.utiDmageMultiplier[stat.SkillLevelUpDatas[SkillType.Utimate]]);
            return;
        }

        if (states.IsSkill && photonView.IsMine && isUtimate)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            photonView.RPC("ArcherUltimate", RpcTarget.AllViaServer, input, (int) (stat.curPower * data.skillDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Skill]]));
        }

    }

    IEnumerator UtiCharging(int damage)
    {
        isUtimate = false;
        states.Stop();
        animator.SetTrigger("Utimate");
        PlaySFX(utiSounds[0]);
        yield return new WaitForSeconds(data.utiStartupTime);
        PlaySFX(utiSounds[1]);
        CreateArrow(utiPrefab);
        FireArrow(curArrow, damage, data.utiRange, "ArcherUti", 3);
        states.Normal();
        yield return new WaitForSeconds(data.utiCoolTime[stat.SkillLevelUpDatas[SkillType.Utimate]]);
        isUtimate = true;
    }

    private void OnSkill()
    {
        if (stat.SkillLevelUpDatas[SkillType.Skill] == 0)
            return;
        if (!PhotonNetwork.IsConnected)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int seed = System.DateTime.Now.Millisecond;
            ArcherSkill(input, seed, (int) (stat.curPower * data.skillDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Skill]]));
            return;
        }
        if (states.IsSkill && photonView.IsMine && isSkill && states.IsMovingAttack)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int seed = System.DateTime.Now.Millisecond;
            photonView.RPC("ArcherSkill", RpcTarget.AllViaServer, input, seed, (int) (stat.curPower * data.skillDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Skill]]));
        }
    }

    private void Skill(int damage)
    {
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            CreateArrow(skillPrefab);
            curArrow.damage = PlayerUtils.Critical(damage, stat.curCritical, stat.curCriticalMultiplier);
            curArrow.Init(transform.position, RangeCheck(), 3f, 6f, 3f, photonView.Owner, seed + i, gameObject, "ArcherSkill");
            PlaySFX(HitSound[1]);
        }

    }

    private Vector2 RangeCheck()
    {
        if (data.skillRange * data.skillRange > distanceToMouse.sqrMagnitude)
        {
            return (Vector2) transform.position + distanceToMouse;
        }
        else
        {
            return (Vector2) transform.position + direction * data.skillRange;
        }
    }

    IEnumerator SkillCool(int damage)
    {
        isSkill = false;
        PlaySFX(SkillSounds[0]);
        animator.SetTrigger("Skill");
        yield return new WaitForSeconds(data.skillStartupTime);
        Skill(damage);
        yield return new WaitForSeconds(data.skillCoolTime[stat.SkillLevelUpDatas[SkillType.Skill]]);
        isSkill = true;
    }

    private void OnDash()
    {
        if (stat.SkillLevelUpDatas[SkillType.Dash] == 0)
            return;

        if (!PhotonNetwork.IsConnected)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ArcherDash(input);
            return;
        }

        if (states.IsSkill && photonView.IsMine && states.IsAttack && states.IsMovingAttack)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            photonView.RPC("ArcherDash", RpcTarget.AllViaServer, input);
        }
    }

    [PunRPC]
    public void ArcherDash(Vector2 mousePos)
    {
        this.mousePos = mousePos;
        MouseCheck();
        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        Damageables.Clear();
        if (isDash)
        {
            PlaySFX(dashSounds[0]);
            curAttackEffectAni.transform.position = transform.position;
            curAttackEffectAni.Play("ArcherDash");
            isDash = false;
            float startTime = 0f;
            Vector2 endPos = PlayerUtils.CheckForCollision(transform.position, direction, data.dashRange, rayMask);
            states.Stop();
            while (startTime < endTime)
            {
                startTime += Time.deltaTime;
                float t = startTime / endTime;
                Vector2 newPos = Vector2.Lerp(transform.position, endPos, t);
                transform.position = newPos;
                Dashing();
                yield return null;
            }
            PlaySFX(dashSounds[1]);
            curAttackEffectAni.Play("ArcherDash2");

            Collider2D[] enemys = PlayerUtils.AttackCheck(360, stat.curPower * data.dashDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Dash]], data.dashRange, stat.curCritical, stat.curCriticalMultiplier, 0, gameObject, colliders);
            for (int i = 0; i < enemys.Length; i++)
            {
                if (colliders[i].CompareTag("Player") && colliders[i].TryGetComponent(out PhotonView photonView))
                {
                    if (!PhotonNetwork.IsConnected)
                        continue;

                    // 팀전
                    if (PhotonNetwork.CurrentRoom.GetGameMode() == false)
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
                    HitAniPlay("ArcherDash", enemys[i].transform.position);
                    PlaySFX(HitSound[2]);
                }
            }
            states.Normal();
            yield return new WaitForSeconds(data.dashCoolTime[stat.SkillLevelUpDatas[SkillType.Dash]]);
            isDash = true;
        }
    }

    Collider2D[] colliders = new Collider2D[10];
    HashSet<IDamageable> Damageables = new HashSet<IDamageable>();
    private void Dashing()
    {
        Vector2 size = new Vector2(1f, 1f);
        int count = Physics2D.OverlapBoxNonAlloc(transform.position, size, 360, colliders);
        float damage = PlayerUtils.Critical(stat.curPower * (data.dashDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Dash]] - 0.1f), data.critical, data.critMultiplier);
        curAttackEffectAni.transform.position = transform.position;
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].gameObject == gameObject)
                continue;
            if (colliders[i].TryGetComponent(out IDamageable damageable))
            {
                if (!Damageables.Contains(damageable))
                {
                    damageable.TakeDamage((int) damage);
                    damageable.LastHitter = gameObject;
                    Damageables.Add(damageable);
                }
            }
        }

    }
}

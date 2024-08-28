using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class WizardAttack : MonoBehaviourPun, IAttackDisabler
{
    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] PlayerData data;
    [SerializeField] PlayerStatsManager stat;
    [SerializeField] PlayerStates state;
    [SerializeField] LineRenderer dashRangeLine;
    [SerializeField] LineRenderer dashAttackRangeLine;
    [SerializeField] LayerMask layerMask;
    [SerializeField] LighteningScript skillEffect;
    [SerializeField] List<AudioClip> attackSounds;
    [SerializeField] AudioClip SkillSounds;
    [SerializeField] AudioClip dashSounds;
    [SerializeField] List<AudioClip> utiSounds;
    [SerializeField] AudioSource AudioSource;

    PlayerHealth health;
    Animator skillani;
    [Header("Cached Values")]
    Vector3 mousePos;
    Vector2 distanceToMouse;
    Vector2 directionToMouse;
    Collider2D[] colliders = new Collider2D[10];
    Collider2D[] Dashcolliders = new Collider2D[10];
    bool isUltimateReady = true;
    [Header("Prefab")]
    [SerializeField] GameObject wizardSkillEffectPrefab;
    [SerializeField] GameObject attacEffectAni;

    public Animator curAttackEffectAni;
    private void Awake()
    {
        curAttackEffectAni = Instantiate(attacEffectAni).GetComponent<Animator>();
    }

    [PunRPC]
    private void WizardAttackRPC(Vector2 mousePos)
    {
        this.mousePos = mousePos;
        MouseCheck();
        StartCoroutine(AttackCool());
    }
    [PunRPC]
    private void WizardSkillRPC(Vector2 mousePos)
    {
        this.mousePos = mousePos;
        MouseCheck();
        skillDamage = StartCoroutine(SkillDamage());
    }
    [PunRPC]
    private void WizardDashRPC(Vector2 mousePos)
    {
        this.mousePos = mousePos;
        MouseCheck();
        StartCoroutine(DashCool());
    }
    [PunRPC]
    private void WizardUtiRPC()
    {
        if (isUltimateReady)
        {
            MagicPowerOpen();
        }
        else
        {
            MagicPowerClose();
        }
    }
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


    private void Start()
    {
        skillani = Instantiate(wizardSkillEffectPrefab).GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && photonView.IsMine)
        {
            StopCoroutine(skillDamage);
            UtiDebuffDamage = (int) (stat.MaxHp * data.utiDmageMultiplier[stat.SkillLevelUpDatas[SkillType.Utimate] + 8]);
            skillEffect.gameObject.SetActive(false);
        }

    }
    private void MouseCheck()
    {
        //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distanceToMouse = ((Vector2) mousePos - (Vector2) transform.position);
        directionToMouse = ((Vector2) mousePos - (Vector2) transform.position).normalized;
    }
    bool isAttack = true;
    private void OnAttack()
    {
        if (!state.IsAttack)
            return;
        if (!isAttack)
            return;
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            photonView.RPC("WizardAttackRPC", RpcTarget.AllViaServer, input);
        }
    }
    public void Attack()
    {
        int count = Physics2D.OverlapBoxNonAlloc(mousePos, Vector2.one * 0.5f, 360f, colliders);
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].gameObject == gameObject)
                continue;

            if (colliders[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage((int) PlayerUtils.Critical(stat.curPower * data.attDamageMultiplier[0], stat.curCritical, stat.curCriticalMultiplier));
            }
        }
    }
    private void SkillEffect()
    {
        skillani.gameObject.transform.position = new Vector3(mousePos.x, mousePos.y + 1.2f, 0);
        Debug.Log(skillani.gameObject.transform.position);
        if (isUltimateReady == true)
        {
            random = Random.Range(0, 2);
            PlaySFX(attackSounds[random]);
            skillani.Play("WizardNomalAttack");
        }
        else
        {
            random = Random.Range(2, 4);
            PlaySFX(attackSounds[random]);
            skillani.Play("WizardUtiAttack");
        }

    }
    int random;
    IEnumerator AttackCool()
    {
        isAttack = false;
        yield return new WaitForSeconds(data.attStartupTime);
        SkillEffect();
        Attack();
        state.MoveAttack();
        yield return new WaitForSeconds(data.attCoolTime[0]);
        state.AttackNomal();
        isAttack = true;
        yield break;
    }
    bool isSkill = true;
    private void OnSkill()
    {
        if (stat.SkillLevelUpDatas[SkillType.Skill] == 0)
            return;
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            if (isSkill)
            {
                Vector2 input = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                photonView.RPC("WizardSkillRPC", RpcTarget.AllViaServer, input);
            }
        }
    }
    IEnumerator SkillCool()
    {

        isSkill = false;
        yield return new WaitForSeconds(data.skillCoolTime[stat.SkillLevelUpDatas[SkillType.Skill]]);
        isSkill = true;
        yield break;
    }
    Collider2D enemy;
    IDamageable enemydamageable;
    StatusEffectManager blindness;
    Coroutine skillDamage;
    IEnumerator SkillDamage()
    {
        int countTime = 10;
        int count = Physics2D.OverlapBoxNonAlloc(mousePos, Vector2.one, 360f, colliders);
        BlindnessEffect effect = new BlindnessEffect("암흑", 0.2f);
        for (int i = 0; i < count; i++)
        {
            if (colliders[i].gameObject == gameObject)
                continue;
            if (colliders[i].TryGetComponent(out IDamageable damageable))
            {
                enemydamageable = damageable;
                enemy = colliders[i];
                break;
            }
        }
        if (isUltimateReady == false)
        {
            UtiDebuffDamage = (int) (UtiDebuffDamage * data.skillDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Skill] + 4]);
        }
        StartCoroutine(SkillCool());
        while (countTime > 0)
        {
            if ((enemy.transform.position - transform.position).magnitude < data.skillRange)
            {
                skillEffect.gameObject.SetActive(true);
                skillEffect.target = enemy.gameObject;
                PlaySFX(SkillSounds);
                // 6. 마나개방(궁) 켜진상태면 체력소모및 데미지 증가 있음 - 궁 켰을때 따로 조건문 달아주면될듯
                if (isUltimateReady == false)
                {
                    enemydamageable.TakeDamage((int) PlayerUtils.Critical(1.6f * stat.curPower * data.skillDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Skill]],
                         stat.curCritical, stat.curCriticalMultiplier));
                }
                else
                {
                    enemydamageable.TakeDamage((int) PlayerUtils.Critical(stat.curPower * data.skillDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Skill]],
                        stat.curCritical, stat.curCriticalMultiplier));
                }
                if (blindness != null)
                    blindness.AddEffect(effect);
                yield return new WaitForSeconds(0.5f);
                countTime--;
            }
        }
        skillEffect.gameObject.SetActive(false);
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
            photonView.RPC("WizardDashRPC", RpcTarget.AllViaServer, input);
        }
    }
    private void Teleport()
    {
        PlaySFX(dashSounds);
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
    private void TeleportHeal()
    {

        float amount = Mathf.Ceil(stat.MaxHp * data.dashDamageMultiplier[stat.SkillLevelUpDatas[SkillType.Dash]]);
        Debug.Log(amount);
        health.Heal(amount, false);
    }
    IEnumerator DashCool()
    {
        if (isDash)
        {
            isDash = false;
            curAttackEffectAni.transform.position = new Vector2(transform.position.x, transform.position.y + 0.5f);
            curAttackEffectAni.Play("WizardDash");
            Teleport();
            TeleportHeal();
            yield return new WaitForSeconds(data.dashCoolTime[stat.SkillLevelUpDatas[SkillType.Dash]]);
            isDash = true;
        }
    }
    bool isUtiCool = true;
    private void OnUtimate()
    {
        if (stat.SkillLevelUpDatas[SkillType.Utimate] == 0)
            return;
        if (!isUtiCool)
            return;
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            photonView.RPC("WizardUtiRPC", RpcTarget.AllViaServer);
        }
    }
    private void MagicPowerOpen()
    {
        curAttackEffectAni.transform.position = transform.position;
        PlaySFX(utiSounds[1]);
        curAttackEffectAni.Play("WizardUti");
        isUltimateReady = false;
        float amount = Mathf.Round(data.utiDmageMultiplier[stat.SkillLevelUpDatas[SkillType.Utimate]] * health.Health);
        health.TakeFixedDamage((int) amount);
        damgePerSeconde = StartCoroutine(DamgePerSeconde());
        stat.UpdateStat(StatType.Power, data.utiDmageMultiplier[stat.SkillLevelUpDatas[SkillType.Utimate] + 4], true);
    }
    Coroutine damgePerSeconde;
    int UtiDebuffDamage;
    IEnumerator DamgePerSeconde()
    {
        yield return new WaitForSeconds(1f);
        UtiDebuffDamage = (int) (stat.MaxHp * data.utiDmageMultiplier[stat.SkillLevelUpDatas[SkillType.Utimate] + 8]);
        while (true)
        {
            PlaySFX(utiSounds[2]);
            if (health.Health > UtiDebuffDamage)
            {
                health.TakeFixedDamage(UtiDebuffDamage);
                Debug.Log($"초당 고정 데미지 : {UtiDebuffDamage}");
                yield return new WaitForSeconds(1f);
            }
            else
            {
                health.TakeFixedDamage(health.Health - 1);
                Debug.Log($"마지막 : {health.Health - 1}");
                MagicPowerClose();
                StartCoroutine(UtiCool());
                yield break;
            }
        }
    }
    IEnumerator UtiCool()
    {
        isUtiCool = false;
        yield return new WaitForSeconds(data.utiCoolTime[0]);
        isUtiCool = true;
    }
    private void MagicPowerClose()
    {

        if (health.Health > 1)
        {
            curAttackEffectAni.Play("WizardUti");
            isUltimateReady = true;
        }
        StopCoroutine(damgePerSeconde);
        stat.UpdateStat(StatType.Power, -data.utiDmageMultiplier[stat.SkillLevelUpDatas[SkillType.Utimate] + 4], true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Photon.Pun;
using Photon.Realtime;
using Tae.Inventory;

namespace Tae
{
    public class ExcaliburOwner : MonoBehaviourPun
    {
        [Header("LayerMasks")]
        private LayerMask exitMask;
        private LayerMask wallMask;

        [Header("Attachments")]
        private ExcaliburWeapon weapon;
        private GuideArrow guideArrow;

        [Header("Data")]
        [SerializeField] ExcaliburSkillSO[] excaliburSkills;

        [Header("Prefabs")]
        [SerializeField] GameObject flamePrefab;
        [SerializeField] GameObject excaliburPrefab;
        [SerializeField] GameObject damageCirclePrefab;
        [SerializeField] GameObject excaliburAttackPrefab;
        public GameObject curexcaliburAttackPrefab;

        [Header("Attack")]
        HashSet<Collider2D> colliderHashSet = new HashSet<Collider2D>();

        [Header("Skills")]
        private Collider2D[] colliders = new Collider2D[10];
        [SerializeField] float skillCooldown = 15f;
        private float cooldownTimer;
        public float CooldownTimer { get { return cooldownTimer; } set { cooldownTimer = value; cooldownChangedEvent?.Invoke(cooldownRatio); } }
        private float curSkillCooldown;
        private float cooldownRatio
        {
            get
            {
                if (curSkillCooldown == 0)
                    return 0;
                return cooldownTimer / curSkillCooldown;
            }
        }
        public event Action<float> cooldownChangedEvent;
        [Header("Skill1")]
        // Skill1
        [SerializeField] float dashRange = 5f;
        [SerializeField] float dashSpeed = 10f;
        [SerializeField] float skill1DamageMultiplier = 250f;
        [SerializeField] float skill1Radius = 1f;
        [Header("Skill2")]
        // Skill2
        [SerializeField] GameObject chargeEffect;
        [SerializeField] float maxChargeTime = 5f;
        [SerializeField] float skill2Radius = 5f;
        [SerializeField] float skill2MinDamage = 190f;
        [SerializeField] float skill2MaxDamage = 800f;
        private bool skill2EndFlag;
        [Header("Skill3")]
        // Skill3
        [SerializeField] GameObject speedUpEffect;
        [SerializeField] float skill3Duration = 3f;
        [SerializeField] float skill3SpeedUpAmt;

        // Skill Rotation
        [Header("Skill Queue")]
        [SerializeField] ExcaliburSkillUI skillUI;
        [SerializeField] ExcaliburSkillPoolSO skillPool;
        private List<ExcaliburSkillSO> remainingSkills = new List<ExcaliburSkillSO>();
        private Queue<ExcaliburSkillSO> skillQueue = new Queue<ExcaliburSkillSO>();

        [Header("Misc")]
        private float t;
        Coroutine flameRoutine;
        [SerializeField] InputAction useSkillAction;
        private bool isCharging;
        private bool isUsingSkill;
        private bool usedAttack;
        private PlayerStates playerStates;
        private PlayerStatsManager playerStats;
        private Transform exitLocation;

        private void Awake()
        {
            exitMask = 1 << LayerMask.NameToLayer("Exit");
            wallMask = 1 << LayerMask.NameToLayer("Wall");
            playerStates = GetComponent<PlayerStates>();
            playerStats = GetComponent<PlayerStatsManager>();
            weapon = GetComponentInChildren<ExcaliburWeapon>(true);
            guideArrow = GetComponentInChildren<GuideArrow>(true);
            SetupInputCallbacks();
        }

        private void SetupInputCallbacks()
        {
            useSkillAction.performed += ctx =>
            {
                UseSkill();
            };

            useSkillAction.started += ctx =>
            {
                isCharging = true;
            };

            useSkillAction.canceled += ctx =>
            {
                isCharging = false;
            };
        }

        private void Start()
        {
            curexcaliburAttackPrefab = excaliburAttackPrefab;
            if (!photonView.IsMine && PhotonNetwork.IsConnected == true)
                return;

            InitSkillQueue();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                DropExcalibur();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SlipExcalibur();
            }
        }

        private void OnEnable()
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected == true)
                return;

            skillUI = FindObjectOfType<ExcaliburSkillUI>(true);
            skillUI.SetOwner(this);
            EquipExcalibur();
        }

        private void OnDisable()
        {
            UnEquipExcalibur();
        }

        private void EquipExcalibur()
        {
            if (PhotonNetwork.IsConnected)
                photonView.RPC("EquipExc", RpcTarget.All);
            else
                EquipExc();
            //guideArrow.StartNavigation(exitLocation);
            skillUI.gameObject.SetActive(true);
            useSkillAction.Enable();
            ApplyExcaliburStats();
            playerStates.IsExcaliburEquipped(true);
        }

        private void UnEquipExcalibur()
        {
            if(PhotonNetwork.IsConnected)
                photonView.RPC("UnEquipExc", RpcTarget.All);

            // Skill
            if (isUsingSkill)
                SkillFinished();
            CooldownTimer = 0;
            skillUI.gameObject.SetActive(false);
            speedUpEffect.SetActive(false);
            chargeEffect.SetActive(false);

            // Components
            //guideArrow.StopNavigation();
            useSkillAction.Disable();
            RemoveExcaliburStats();
            playerStates.IsExcaliburEquipped(false);

            StopAllCoroutines();
        }

        private void ApplyExcaliburStats()
        {
            playerStats.UpdateStat(StatType.Power, 0.05f, true);
            playerStats.UpdateStat(StatType.Armor, 0.05f, true);
            playerStats.UpdateStat(StatType.Speed, -0.05f, true);
        }

        private void RemoveExcaliburStats()
        {
            playerStats.UpdateStat(StatType.Power, -0.05f, true);
            playerStats.UpdateStat(StatType.Armor, -0.05f, true);
            playerStats.UpdateStat(StatType.Speed, 0.05f, true);
        }

        #region RPCs
        [PunRPC]
        public void EquipExc()
        {
            weapon.gameObject.SetActive(true);
        }

        [PunRPC]
        public void UnEquipExc()
        {
            weapon.gameObject.SetActive(false);
        }

        [PunRPC]
        public void ExcSkill2Start()
        {
            chargeEffect.SetActive(true);
        }

        [PunRPC]
        public void ExcSkill2End()
        {
            // Add Explosion effect
            DrawCircle(skill2Radius);
            chargeEffect.SetActive(false);
        }

        [PunRPC]
        public void ExcSkill3Start()
        {
            speedUpEffect.SetActive(true);
        }

        [PunRPC]
        public void ExcSkill3Attack()
        {
            DrawCircle(1);
        }

        [PunRPC]
        public void ExcSkill3End()
        {
            speedUpEffect.SetActive(false);
        }
        #endregion

        private void OnAttack()
        {
            if (!enabled)
                return;

            weapon.Attack();
            usedAttack = true;
        }

        private void UseSkill()
        {
            if (!enabled)
                return;

            if (cooldownTimer > 0 || isUsingSkill)
                return;

            if (skillQueue.TryDequeue(out var skillToUse))
            {
                isUsingSkill = true;
                switch (skillToUse.id)
                {
                    case 1:
                        Debug.Log("Used Skill1");
                        playerStates.Stop();
                        Skill1();
                        break;
                    case 2:
                        Debug.Log("Used Skill2");
                        playerStates.Stop();
                        Skill2();
                        break;
                    case 3:
                        Debug.Log("Used Skill3");
                        Skill3();
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (enabled == false)
                return;

            if (exitMask.Contain(collision.gameObject.layer))
            {
                GameOver();
            }
        }

        public void GameOver()
        {
            EventManager.Instance.GameOver(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.GetTeam());
        }


        #region Drop Excalibur
        [PunRPC]
        public void RequestDropExcalibur(bool isLeft)
        {
            ExcaliburItem excalibur = PhotonNetwork.InstantiateRoomObject("ExcaliburDrop", transform.position, Quaternion.identity).GetComponent<ExcaliburItem>();
            excalibur.Drop(isLeft);
        }

        public void DropExcalibur()
        {
            enabled = false;
            bool isLeft = Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x;
            photonView.RPC("RequestDropExcalibur", RpcTarget.MasterClient, isLeft);
        }

        [PunRPC]
        public void RequestSlipExcalibur(bool isLeft)
        {
            ExcaliburItem excalibur = PhotonNetwork.InstantiateRoomObject("ExcaliburDrop", transform.position, Quaternion.identity).GetComponent<ExcaliburItem>();
            excalibur.Slip(isLeft);
        }

        public void SlipExcalibur()
        {
            enabled = false;
            bool isLeft = Camera.main.ScreenToWorldPoint(Input.mousePosition).x < transform.position.x;
            photonView.RPC("RequestSlipExcalibur", RpcTarget.MasterClient, isLeft);
        }
        #endregion

        #region Skills
        private void DrawCircle(float radius) // TODO: Change to effect
        {
            Transform circle = Instantiate(damageCirclePrefab, transform.position, Quaternion.identity).GetComponent<Transform>();
            circle.localScale = new Vector3(radius * 2, radius * 2, 1);
        }

        private void OnDrawGizmos() // Show Skill2 Range
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, skill2Radius);
        }

        #region Skill1
        /*
         * 화염 돌격
        마우스 커서가 있는곳으로 전진,
        전진 거리는 캐릭터 5개 정도의 거리
        전진 하는 경로안에 타격 가능한 개체가 있을경우 데미지를 입힘.
        (플레이어의 이동은 막히지않습니다.)
        지나간 경로에 통과가 가능하나 플레이어와 닿을경우 데미지를 주는 불 오브젝트 5초간 생성
        */
        public void Skill1() // 
        {
            StartCoroutine(Skill1Routine());
        }

        private IEnumerator Skill1Routine()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - (Vector2) transform.position).normalized;
            Vector2 startPos = (Vector2) transform.position;

            // Calculate targetPos by checking if there is a wall
            Vector2 targetPos = startPos + direction * dashRange;
            RaycastHit2D hit = Physics2D.Raycast(startPos, direction, dashRange, wallMask);
            if (hit.collider != null)
            {
                targetPos = hit.point - direction * 0.5f;
            }

            // Dash to targetPos
            float distance = (targetPos - (Vector2) transform.position).magnitude;
            float timeToTarget = distance / dashSpeed;
            float time = 0f;
            flameRoutine = StartCoroutine(FlameRoutine());
            Manager.Sound.PlaySFX(Manager.Sound.SoundSO.EXSkill1SFX);
            colliderHashSet.Clear();
            while ((targetPos - (Vector2) transform.position).sqrMagnitude > 0.1f && time <= timeToTarget)
            {
                Skill1Hitbox();
                float distanceThisFrame = dashSpeed * Time.deltaTime;
                transform.position += (Vector3) direction * distanceThisFrame;
                time += Time.deltaTime;
                yield return null;
            }
            StopCoroutine(flameRoutine);
            SkillFinished();
        }

        private void Skill1Hitbox()
        {
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, skill1Radius, colliders);
            for (int i = 0; i < count; i++)
            {
                if (colliders[i].gameObject == gameObject || colliderHashSet.Contains(colliders[i]))
                    continue;

                if (colliders[i].TryGetComponent(out IDamageable damageable))
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        damageable.TakeDamage((int) (10 * skill1DamageMultiplier * 0.01f));
                        damageable.LastHitter = gameObject;
                    }
                    colliderHashSet.Add(colliders[i]);
                }
            }
        }

        [PunRPC]
        public void SpawnFlame(Vector2 spawnPos, int ownerID)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Flame flame = PhotonNetwork.InstantiateRoomObject("Flame", spawnPos, Quaternion.identity).GetComponent<Flame>();
                flame.GetComponent<PhotonView>().RPC("Init", RpcTarget.AllBuffered, ownerID);
            }

            if(!PhotonNetwork.IsConnected)
            {
                Flame flame = Instantiate((GameObject)Resources.Load("Flame"), spawnPos, Quaternion.identity).GetComponent<Flame>();
            }
        }

        private IEnumerator FlameRoutine()
        {
            float spawnInterval = 0.1f;
            while (true)
            {
                if (PhotonNetwork.IsConnected)
                    photonView.RPC("SpawnFlame", RpcTarget.MasterClient, (Vector2) transform.position, PhotonNetwork.LocalPlayer.ActorNumber);
                else
                    SpawnFlame(transform.position, 0);
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        #endregion

        #region Skill2
        /*
         * 쿠구구구 쾅
        스킬 시전 키를 누르면 차징이 시작되며 스킬을 시전하는 동안에는 캐릭터를 움직일 수 없음.
        스킬 시전키를 떼면 뗀 시점까지 측정된 데미지를 범위내 적들에게 입힘 범위내 적들은 데미지와 기절 상태이상을 입음.

        차징시간은 최대 5초이며, 5초가 넘으면 자동으로 시전됨.  시전 최초1초동안은 데미지 % 가 올라가지 않고 1초 이후부터 데미지가 190% 이상으로 올라감
        */
        public void Skill2()
        {
            StartCoroutine(Skill2Routine());
        }

        private IEnumerator Skill2Routine()
        {
            curexcaliburAttackPrefab.transform.position = transform.position;
            //chargeEffect.SetActive(true);
            if (PhotonNetwork.IsConnected)
                photonView.RPC("ExcSkill2Start", RpcTarget.All);
            else
                ExcSkill2Start();
            float time = 0;
            Manager.Sound.PlaySFX(Manager.Sound.SoundSO.EXSkill2ChargieSFX);
            while (time < maxChargeTime && isCharging/*!skill2EndFlag*/)
            {
                time += Time.deltaTime;
                yield return null;
            }

            float chargeRatio = 0;
            if (time > 1)
            {
                time -= 1;
                chargeRatio = time / (maxChargeTime - 1);
            }
            Manager.Sound.PlaySFX(Manager.Sound.SoundSO.EXSkill2ExplosionSFX);
            curexcaliburAttackPrefab.GetComponent<Animator>().Play("Skill02_use");
            float damageMultiplier = Mathf.Lerp(skill2MinDamage, skill2MaxDamage, chargeRatio) * 0.01f;
            int count = Physics2D.OverlapCircleNonAlloc(transform.position, skill2Radius, colliders);
            DrawCircle(skill2Radius);
            for (int i = 0; i < count; i++)
            {
                if (colliders[i].gameObject == gameObject)
                    continue;
                if (colliders[i].TryGetComponent(out IDamageable damageable))
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        damageable.TakeDamage(Mathf.CeilToInt(10 * damageMultiplier));
                        damageable.LastHitter = gameObject;
                    }
                }
            }

            SkillFinished();
            // chargeEffect.SetActive(false);
            if (PhotonNetwork.IsConnected)
                photonView.RPC("ExcSkill2End", RpcTarget.All);
            else
                ExcSkill2End();
        }
        #endregion

        #region Skill3
        /*
         * 왕가의 힘
        스킬 시전시 바로 3초간 이동속도가 1.7배 증가함.  
        3초 이내에 공격을 시전하면 내려찍어 데미지를 입히고 이동속도 버프는 종료됨. 
        공격을 당한 적은 1초간 기절함
        */
        public void Skill3()
        {
            StartCoroutine(Skill3Routine());
        }

        private IEnumerator Skill3Routine()
        {
            // Speed up player
            Manager.Sound.PlaySFX(Manager.Sound.SoundSO.EXSkill3SFX);
            PlayerSpeedUp();
            SkillFinished();
            usedAttack = false;
            float t = 0;
            Manager.Sound.WalkSFXPlay(Manager.Sound.SoundSO.EXSkill3MoveSoundSFX);
            while (t < skill3Duration && !usedAttack) // Stop speed up if attack finished or if 3 seconds pass
            {
                t += Time.deltaTime;
                yield return null;
            }

            if (t < skill3Duration) // Used attack before 3 seconds
            {
                playerStates.Stop();
                Skill3Attack();
            }

            PlayerSpeedDown();
            SkillFinished();
        }

        private void Skill3Attack()
        {
            if (PhotonNetwork.IsConnected)
                photonView.RPC("ExcSkill3Attack", RpcTarget.All);
            else
                ExcSkill3Attack();
            //DrawCircle(1);
        }

        private void PlayerSpeedUp()
        {
            //speedUpEffect.SetActive(true);
            if (PhotonNetwork.IsConnected)
                photonView.RPC("ExcSkill3Start", RpcTarget.All);
            else 
                ExcSkill3Start();
            
            // TODO: Implement Speed up
        }

        private void PlayerSpeedDown()
        {
            //speedUpEffect.SetActive(false);
            if(PhotonNetwork.IsConnected)
                photonView.RPC("ExcSkill3End", RpcTarget.All);
            else
                ExcSkill3End();

            // TODO: Implement Speed down
        }
        #endregion
        #endregion

        #region Skill Queue
        private IEnumerator CooldownRoutine(float cooldown)
        {
            curSkillCooldown = cooldown;
            CooldownTimer = cooldown;
            while (CooldownTimer > 0)
            {
                CooldownTimer -= Time.deltaTime;
                yield return null;
            }
            CooldownTimer = 0;
        }

        private void SkillFinished()
        {
            isUsingSkill = false;
            ExcaliburSkillSO nextSkill = GetRandomSkill();
            skillUI.NextSkill(nextSkill);
            skillQueue.Enqueue(nextSkill);
            StartCoroutine(CooldownRoutine(skillCooldown));
            playerStates.Normal();
        }

        private void InitSkillQueue()
        {
            for (int i = 0; i < 4; i++)
            {
                ExcaliburSkillSO skill = GetRandomSkill();
                skillQueue.Enqueue(skill);
                skillUI.AddSkill(skill);
            }
        }

        private ExcaliburSkillSO GetRandomSkill()
        {
            if (remainingSkills.Count == 0)
                RefillSkills();

            int random = UnityEngine.Random.Range(0, remainingSkills.Count);
            ExcaliburSkillSO skill = remainingSkills[random];
            remainingSkills.RemoveAt(random);
            return skill;
        }

        private void RefillSkills()
        {
            foreach (var skill in skillPool.skills)
            {
                remainingSkills.Add(skill);
            }
        }
        #endregion
    }
}


using System.Collections;
using UnityEngine;
using Pathfinding;
using Photon.Pun;
using System;
using System.Collections.Generic;
using ItemLootSystem;
using Tae.Inventory;

public enum MonsterType { Normal, Elite }

namespace Tae
{
    public class BaseMonster : MonoBehaviourPun
    {
        public bool debugMode;
        [SerializeField] MonsterType monsterType;

        // Stats
        [SerializeField] protected MonsterDataSO data;
        public int Health => data.phaseStats.hp[Manager.Game.GetPhaseNumber() - 1];
        public int Power => data.phaseStats.power[Manager.Game.GetPhaseNumber() - 1];
        public int Armor => data.phaseStats.armor[Manager.Game.GetPhaseNumber() - 1];
        public int Exp => data.phaseStats.exp[Manager.Game.GetPhaseNumber() - 1];

        // Properties
        [Header("Properties")]
        const float baseMoveSpeed = 2;
        private float moveSpeed => baseMoveSpeed * data.movespeedMultiplier;
        const float minIdleTime = 2;
        private float attackRange => data.attackRange;
        const float patrolDistance = 3;
        const float invincibilityDuration = 0.5f;
        private Vector2 myPos => (Vector2) transform.position;
        protected bool isFacingRight => model.transform.rotation == Quaternion.Euler(Vector3.zero);

        // Components
        [Header("Components")]
        [SerializeField] Transform model;
        [SerializeField] protected Animator animator;
        [SerializeField] protected LayerMask playerMask;
        [SerializeField] LayerMask wallMask;
        protected MonsterHealth monsterHealth;
        private Seeker seeker;

        // Animation States
        protected const string IDLE = "Idle";
        private const string WALK = "Walk";
        protected const string ATTACK1 = "Attack01";
        protected const string ATTACK2 = "Attack02";
        protected const string ATTACK3 = "Attack03";
        private const string HURT = "Hurt";
        private const string DIE = "Die";

        // State flags
        public bool patrolFinished;
        public bool attackFinished;
        public bool chaseFinished;
        public bool playerInSeekRange;
        public bool playerInAttackRange;
        public bool isInvincible;
        public bool hurtFinished;
        public bool isTouchingWall;

        // States
        public enum State { Idle, Patrol, Chase, Attack, Hurt, Dead }
        public enum Animation { Idle, Walk, Attack01, Attack02, Attack03, Hurt, Die }
        protected StateMachine<State> stateMachine = new StateMachine<State>();

        protected Transform playerToChase;

        protected Collider2D[] colliders = new Collider2D[10];

        public Action<BaseMonster> OnDied;

        // Item Drops
        [SerializeField] MonsterDropTable monsterDropTable;
        [SerializeField] DropedItem itemPrefab;
        private List<BaseItemData> itemsToDrop = new();
        private const float EQUIPMENT_DROP_RATE = 0.7f;
        private const float CONSUMABLE_DROP_RATE = 0.5f;

        private void Awake()
        {
            seeker = GetComponent<Seeker>();
            monsterHealth = GetComponent<MonsterHealth>();
        }

        protected virtual void Start()
        {
            stateMachine.AddState(State.Idle, new IdleState(this));
            stateMachine.AddState(State.Patrol, new PatrolState(this));
            stateMachine.AddState(State.Chase, new ChaseState(this));
            stateMachine.AddState(State.Attack, new AttackState(this));
            stateMachine.AddState(State.Hurt, new HurtState(this));
            stateMachine.AddState(State.Dead, new DeadState(this));

            stateMachine.Start(State.Idle);
        }

        protected virtual void Update()
        {
            stateMachine.Update();
        }

        protected void Flip(Vector2 direction)
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                photonView.RPC("FlipRPC", RpcTarget.All, direction);
            else
                FlipRPC(direction);
        }

        [PunRPC]
        public void FlipRPC(Vector2 direction)
        {
            if (direction.x >= 0) // RIGHT
            {
                model.rotation = Quaternion.Euler(Vector3.zero);
            }
            else // LEFT
            {
                model.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }
        }

        public void PlayAnim(byte anim)
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                photonView.RPC("PlayAnimRPC", RpcTarget.All, anim);
            else
                PlayAnimRPC(anim);
        }

        [PunRPC]
        public void PlayAnimRPC(byte anim)
        {
            Animation animation = (Animation) (int) anim;
            animator.Play(animation.ToString());
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(myPos, patrolDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(myPos, attackRange);
        }

        #region Movement
        private Coroutine idleRoutine;
        public void StartIdle()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
                return;

            idleRoutine = StartCoroutine(IdleRoutine());
        }

        public void StopIdle()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
                return;

            if(idleRoutine != null)
                StopCoroutine(idleRoutine);
        }

        private IEnumerator IdleRoutine()
        {
            animator.Play(IDLE);
            yield return new WaitForSeconds(minIdleTime);

            while (true)
            {
                float random = UnityEngine.Random.Range(0f, 1f);
                if (random >= 0.5f)
                {
                    stateMachine.ChangeState(State.Patrol);
                    yield break;
                }
                yield return new WaitForSeconds(1);
            }
        }

        private Coroutine patrolRoutine;
        public void StartPatrol()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
                return;

            patrolRoutine = StartCoroutine(PatrolRoutine());
        }

        private IEnumerator PatrolRoutine()
        {
            // 탐지 범위 내 한 좌표로 이동
            Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
            Vector2 startPos = myPos;
            Vector2 targetPos = myPos + randomDir * patrolDistance;

            PlayAnim(Convert.ToByte(Animation.Walk));
            Vector2 direction = (targetPos - myPos).normalized;
            Flip(direction);
            while ((targetPos - myPos).sqrMagnitude > 0.01f)
            {
                if (isTouchingWall)  // TODO: Fix infinite loop caused by wall
                    break;

                transform.Translate(direction * moveSpeed * Time.deltaTime);
                yield return null;
            }

            //이동이 종료된 후 1초간 Idle애니메이션 실행 후 원래 좌표로 재이동
            PlayAnim(Convert.ToByte(Animation.Idle));
            yield return new WaitForSeconds(1);

            PlayAnim(Convert.ToByte(Animation.Walk));
            direction = (startPos - myPos).normalized;
            Flip(direction);
            while ((startPos - myPos).sqrMagnitude > 0.01f)
            {
                transform.Translate(direction * moveSpeed * Time.deltaTime);
                yield return null;
            }

            patrolFinished = true;
        }

        public void StopPatrol()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
                return;

            StopCoroutine(patrolRoutine);
        }

        public IEnumerator PauseRoutine(int seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        private Coroutine chaseRoutine;
        public void StartChase()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
                return;

            chaseRoutine = StartCoroutine(ChaseRoutine());
        }

        private IEnumerator ChaseRoutine()
        {
            Vector2 startPos = myPos;
            Vector3 direction;

            PlayAnim(Convert.ToByte(Animation.Walk));
            while (playerToChase != null && playerInSeekRange)
            {
                direction = (playerToChase.position - transform.position).normalized;
                Flip(direction);
                Vector3 flipRotation = direction.x >= 0 ? Vector3.zero : new Vector3(0, 180, 0);
                model.rotation = Quaternion.Euler(flipRotation);

                float speed = moveSpeed * Time.deltaTime;
                transform.Translate(direction * speed);
                yield return null;
            }

            PlayAnim(Convert.ToByte(Animation.Idle));
            yield return new WaitForSeconds(2);

            PlayAnim(Convert.ToByte(Animation.Walk));
            direction = (startPos - myPos).normalized;
            Flip(direction);
            while ((startPos - myPos).sqrMagnitude > 0.01f)
            {
                transform.Translate(direction * moveSpeed * 2 * Time.deltaTime);
                yield return null;
            }

            chaseFinished = true;
        }

        public void StopChase()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
                return;

            StopCoroutine(chaseRoutine);
            chaseFinished = false;
        }

        private void OnPathComplete(Path path)
        {
            playerInAttackRange = true;
        }
        #endregion

        #region Combat
        protected virtual void StartAttack() { if (!PhotonNetwork.IsMasterClient) return; }
        protected virtual void StopAttack() { if (!PhotonNetwork.IsMasterClient) return; }

        public virtual void Attack01Frame() { }
        public virtual void Attack02Frame() { }
        public virtual void Attack03Frame() { }

        public void TakeDamage()
        {
            if (attackFinished == false)
                return;

            stateMachine.ChangeState(State.Hurt);
        }

        public void StartHurt()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
                return;

            hurtFinished = false;
            StartCoroutine(HurtRoutine());
        }

        private IEnumerator HurtRoutine()
        {
            animator.Play(HURT);
            //isInvincible = true;
            yield return new WaitForSeconds(invincibilityDuration);
            //isInvincible = false;
            hurtFinished = true;
        }

        public void Die()
        {
            OnDied?.Invoke(this);
            stateMachine.ChangeState(State.Dead);
        }

        private void GiveExp()
        {
            Debug.Log("Give exp");
            if (monsterHealth.LastHitter == null)
                return;

            if (monsterHealth.LastHitter.TryGetComponent(out PlayerStatsManager stats))
            {
                Debug.Log(stats.gameObject.name);
                stats.GainExp(Exp);
            }
        }

        public void StartDead()
        {
            if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
                return;

            StartCoroutine(DeadRoutine());
        }

        private IEnumerator DeadRoutine()
        {
            PlayAnim(Convert.ToByte(Animation.Die));
            isInvincible = true;

            yield return new WaitForSeconds(1.5f);

            DropItems();
            GiveExp();
            GiveGold();
            PhotonNetwork.Destroy(gameObject);
        }

        protected void BoxColliderAttackFrame(BoxCollider2D collider, float multiplier)
        {
            Vector2 offset = collider.offset;
            if (!isFacingRight)
                offset.x *= -1;
            Vector2 center = (Vector2) transform.position + offset;
            int count = Physics2D.OverlapBoxNonAlloc(center, collider.size, 0, colliders, playerMask);

            for (int i = 0; i < count; i++)
            {
                if (colliders[i].TryGetComponent(out IDamageable damageable))
                {
                    int damage = (int)(Power * multiplier);
                    damageable.TakeDamage(damage);
                }
            }
        }
        #endregion

        #region Item Drops
        [ContextMenu("drop")]
        private void DropItems()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            ItemDropTable table = Manager.DropTable.GetItemDropTable(monsterType);

            // Cached values
            int numItems;
            ItemRarity rarity;
            EquipItemData equipment;
            BaseItemData item;

            UnityEngine.Random.InitState(DateTime.Now.Millisecond + Environment.TickCount);
            // 모든 몬스터를 잡았을 때 장비 아이템을 드랍할 확률은 70%다.
            float random = UnityEngine.Random.Range(0, 1f);
            if(random <= EQUIPMENT_DROP_RATE)
            {
                Debug.Log("entered");

                // 노말 몬스터는 1~2개, 엘리트 몬스터는 2~3개의 장비 아이템을 드랍한다.
                if(monsterType == MonsterType.Normal)
                    numItems = UnityEngine.Random.Range(1, 3);
                else
                    numItems = UnityEngine.Random.Range(2, 4);

                // 개수에 맞게 장비 아이템 추가
                for (int i = 0; i < numItems; i++)
                {
                    rarity = table.GetEquipmentPool(Manager.Game.Phase).GetRandomRarity();
                    equipment = Manager.DropTable.itemCollection.GetEquipmentWithRarity(rarity);
                    itemsToDrop.Add(equipment);
                }
            }

            //  몬스터를 잡았을 때 소비 아이템을 드랍할 확률은 50%다.
            random = UnityEngine.Random.Range(0, 1f);
            if (random <= CONSUMABLE_DROP_RATE)
            {
                // 노말 몬스터는 1 아니면 2개, 엘리트 몬스터는 1 아니면 3개의 소비 아이템을 드랍한다.
                random = UnityEngine.Random.Range(0, 1f);
                if (random <= 0.5)
                {
                    numItems = 1;
                }
                else
                {
                    if (monsterType == MonsterType.Normal)
                        numItems = 2;
                    else
                        numItems = 3;
                }

                // 개수에 맞게 소비 아이템 추가
                for (int i = 0; i < numItems; i++)
                {
                    rarity = table.GetItemPool(Manager.Game.Phase).GetRandomRarity();
                    item = Manager.DropTable.itemCollection.GetItemWithRarity(rarity);
                    itemsToDrop.Add(item);
                }
            }

            // 디버그 로그
            Debug.Log(itemsToDrop.Count);
            foreach (var itemToDrop in itemsToDrop)
            {
                SpawnNewItem(itemToDrop);
            }

            // 개수에 맞게 기타 아이템 생성
            List<StackableDrop> drops = monsterDropTable.GetRandomDrops();
            foreach (var etcItem in drops)
            {
                SpawnNewItem(etcItem.item, etcItem.count);
            }
        }

        private void SpawnNewItem(BaseItemData item, int throwCount = 1)
        {
            ItemFactory.Instance.SpawnItem(item, transform.position, throwCount);
        }

        private void GiveGold()
        {
            int goldToGive = monsterDropTable.GetGoldToDrop(monsterType);

            if (monsterHealth.LastHitter == null)
                return;

            if (monsterHealth.LastHitter.TryGetComponent(out PhotonView view))
            {
                InventoryManager.Instance.ObtainGold(goldToGive, view.Owner);
            }
        }
        #endregion

        #region Triggers/Collisions
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (wallMask.Contain(collision.gameObject.layer))
            {
                isTouchingWall = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (wallMask.Contain(collision.gameObject.layer))
            {
                isTouchingWall = false;
            }
        }

        public void OnSensorTriggerEnter(Collider2D collision)
        {
            if (playerMask.Contain(collision.gameObject.layer))
            {
                playerInSeekRange = true;
                playerToChase = collision.transform;
            }
        }

        public void OnSensorTriggerStay(Collider2D collision)
        {
            if (playerMask.Contain(collision.gameObject.layer))
            {
                float distanceToPlayer = (collision.transform.position - transform.position).magnitude;
                if (distanceToPlayer <= attackRange)
                {
                    playerInAttackRange = true;
                }
                else
                {
                    playerInAttackRange = false;
                }
            }
        }

        public void OnSensorTriggerExit(Collider2D collision)
        {
            if (playerMask.Contain(collision.gameObject.layer))
            {
                playerInSeekRange = false;
                playerInAttackRange = false;
            }
        }
        #endregion

        #region States
        private class MonsterState : BaseState<State>
        {
            protected BaseMonster monster;
        }

        private class IdleState : MonsterState
        {
            public IdleState(BaseMonster monster)
            {
                this.monster = monster;
            }

            public override void Enter()
            {
                if (monster.debugMode)
                    Debug.Log("Idle State Enter");
                monster.StartIdle();
            }

            public override void Transition()
            {
                if (monster.playerInSeekRange)
                {
                    ChangeState(State.Chase);
                }
            }

            public override void Exit()
            {
                if (monster.debugMode)
                    Debug.Log("Idle State Exit");
                monster.StopIdle();
            }
        }

        private class PatrolState : MonsterState
        {
            public PatrolState(BaseMonster monster)
            {
                this.monster = monster;
            }

            public override void Enter()
            {
                if (monster.debugMode)
                    Debug.Log("Patrol State Enter");
                monster.patrolFinished = false;
                monster.StartPatrol();
            }

            public override void Transition()
            {
                if (monster.patrolFinished)
                {
                    ChangeState(State.Idle);
                }
                if (monster.playerInSeekRange)
                {
                    ChangeState(State.Chase);
                }
            }

            public override void Exit()
            {
                if (monster.debugMode)
                    Debug.Log("Patrol State Exit");
                monster.StopPatrol();
            }
        }

        private class ChaseState : MonsterState
        {
            public ChaseState(BaseMonster monster)
            {
                this.monster = monster;
            }

            public override void Enter()
            {
                if (monster.debugMode)
                    Debug.Log("Chase State Enter");
                monster.StartChase();
            }

            public override void Transition()
            {
                if (monster.playerInAttackRange)
                {
                    ChangeState(State.Attack);
                }
                if (monster.chaseFinished)
                {
                    ChangeState(State.Idle);
                }
            }

            public override void Exit()
            {
                if (monster.debugMode)
                    Debug.Log("Chase State Exit");
                monster.StopChase();
            }
        }

        private class AttackState : MonsterState
        {
            public AttackState(BaseMonster monster)
            {
                this.monster = monster;
            }

            public override void Enter()
            {
                if (monster.debugMode)
                    Debug.Log("Attack State Enter");
                monster.StartAttack();
            }
            public override void Transition()
            {
                if (monster.attackFinished)
                {
                    ChangeState(State.Idle);
                }
            }

            public override void Exit()
            {
                if (monster.debugMode)
                    Debug.Log("Attack State Exit");
                monster.StopAttack();
                monster.attackFinished = false;
            }
        }

        private class HurtState : MonsterState
        {
            public HurtState(BaseMonster monster)
            {
                this.monster = monster;
            }

            public override void Enter()
            {
                if(monster.debugMode)
                    Debug.Log("Hurt State Enter");
                monster.StartHurt();
            }
            public override void Transition()
            {
                if (monster.hurtFinished)
                {
                    ChangeState(State.Idle);
                }
            }

            public override void Exit()
            {
                if (monster.debugMode)
                    Debug.Log("Hurt State Exit");
            }
        }

        private class DeadState : MonsterState
        {
            public DeadState(BaseMonster monster)
            {
                this.monster = monster;
            }

            public override void Enter()
            {
                if (monster.debugMode)
                    Debug.Log("Dead State Enter");
                monster.StartDead();
            }

            public override void Exit()
            {
                if (monster.debugMode)
                    Debug.Log("Dead State Exit");
            }
        }
        #endregion
    }
}
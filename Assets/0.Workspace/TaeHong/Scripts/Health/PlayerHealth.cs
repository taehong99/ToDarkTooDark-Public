using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;

namespace Tae
{
    public class PlayerHealth : BaseHealth, IPunObservable
    {
        [SerializeField] PlayerData data;
        [SerializeField] PlayerStates state;
        [SerializeField] PlayerStatsManager stat;
        [SerializeField] Animator animator;
        [SerializeField] GameObject particle;
        [SerializeField] RectTransform maxhpBG;
        [SerializeField] SpriteRenderer renderer;
        [SerializeField] PlayerRevive playerRevive;
        bool isUndead = false;
        public bool IsUndead { get { return isUndead; } set { isUndead = value; } }
        private void Awake()
        {
            base.Awake();
            MaxHealth = (int) data.hp;
        }
        private void Start()
        {
            HealOverTimeRoutine = StartCoroutine(HealOverTime());
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(MaxHealth);
                stream.SendNext(Health);
                stream.SendNext(MaxShield);
                stream.SendNext(Shield);
            }
            else // stream.IsReading
            {
                MaxHealth = (int) stream.ReceiveNext();
                Health = (int)stream.ReceiveNext();
                MaxShield = (int) stream.ReceiveNext();
                Shield = (int)stream.ReceiveNext();
            }
        }

        public override void Heal(float amount, bool isPercent)
        {
            if (!isUndead)
            {
                base.Heal(amount, isPercent);
            }
            else
            {
                TakeDamage((int) (amount * 0.01f * MaxHealth));
            }
        }

        public override void TakeDamage(int amount)
        {
            if (isDead)
                return;

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("ApplyDamage", RpcTarget.All, amount);
            }
                
            else
                ApplyDamage(amount);
        }

        public override void TakeFixedDamage(int amount)
        {
            if (isDead)
                return;

            if (PhotonNetwork.IsConnected)
                photonView.RPC("TakeFixedDamageRPC", RpcTarget.All, amount);
            else
                TakeFixedDamageRPC(amount);
        }

        public override void GetKnockedBack(Vector2 direction, float distance, float duration)
        {
            if (isDead)
                return;

            if (PhotonNetwork.IsConnected)
                photonView.RPC("KnockBackRPC", RpcTarget.All, direction, distance, duration);
            else
                KnockBackRPC(direction, distance, duration);
        }

        [PunRPC]
        public void KnockBackRPC(Vector2 direction, float distance, float duration)
        {
            // Don't get knocked back when dead
            if (isDead)
                return;

            // Only owner
            if (!photonView.IsMine)
                return;

            StartCoroutine(KnockBackRoutine(direction, distance, duration));
        }

        [PunRPC]
        public void ApplyDamage(int amount)
        {
            // Only owner calculates damage
            if (!photonView.IsMine)
                return;

            // Don't take damage when dead
            if (state.IsDie)
                return;

            int damage = (int) (amount * ((100 - stat.curArmor) * 0.01f));
            photonView.RPC("TakeDamageRPC", RpcTarget.AllViaServer, damage);
        }

        [PunRPC]
        public void TakeDamageRPC(int amount)
        {
            StopCoroutine(HealOverTimeRoutine);
            if (startHealOverTime != null)
                StopCoroutine(startHealOverTime);

            base.TakeDamage(amount);

            if (Health > 0)
            {
                StartCoroutine(TakeDamageRoutine());
                startHealOverTime = StartCoroutine(StartHealOverTime());
            }
        }

        // 지속데미지
        [PunRPC]
        public void TakeFixedDamageRPC(int amount)
        {
            // Only owner
            if (!photonView.IsMine)
                return;

            // Don't get damaged when dead
            if (state.IsDie)
                return;

            // 초당 회복을 멈춰 줌
            StopCoroutine(HealOverTimeRoutine);
            // 만약 회복이 멈춰 있던 상황이였다면 초당회복량 시작하는거 다시 초기화
            if (startHealOverTime != null)
                StopCoroutine(startHealOverTime);
            base.TakeFixedDamage(amount);
            startHealOverTime = StartCoroutine(StartHealOverTime());

        }
        // 지속데미지 텍스트 색깔 바꿔줌
        IEnumerator TakeDamageRoutine()
        {
            renderer.color = new Color(255, 0, 0);
            yield return new WaitForSeconds(0.1f);
            renderer.color = new Color(255, 255, 255);

        }
        // 스턴 넣는 함수
        public override void ApplyStun(float duration)
        {
            if (duration == 0)
                return;

            if (PhotonNetwork.IsConnected)
                photonView.RPC("StunRPC", RpcTarget.All, duration);
            else
                StunRPC(duration);
        }

        [PunRPC]
        public void StunRPC(float duration)
        {
            // Don't get stunned when dead
            if (state.IsDie)
                return;

            // 스턴 이펙트 활성화
            particle.SetActive(true);
            // 스턴이라는 클래스를 가져와서 반응해줌
            Stun stun = new Stun("Stun", duration, particle);
            gameObject.GetComponent<StatusEffectManager>().AddEffect(stun);

            // 엑칼 떨구기 
            if (photonView.IsMine && TryGetComponent(out ExcaliburOwner owner) && owner.enabled)
            {
                owner.SlipExcalibur();
            }
        }

        protected override void Die()
        {
            gameObject.layer = LayerMask.NameToLayer("DeadBody");
            state.DieStates();
            animator.SetTrigger("Die");

            // 팀전이면 부활기능 작동
            if(PhotonNetwork.CurrentRoom.GetGameMode() == false)
                playerRevive.Activate();

            if(photonView.IsMine && PhotonNetwork.CurrentRoom.GetGameMode() == false)
            {
                Manager.Game.SpectateTeammate();
            }

            // 엑칼 떨구기
            if (TryGetComponent(out ExcaliburOwner owner) && owner.enabled)
            {
                owner.DropExcalibur();
            }
        }

        public void Revive()
        {
            Heal(50, true);
            if ( PhotonNetwork.CurrentRoom.GetGameMode() == false)
                Manager.Game.ReturnCameraToSelf();

            photonView.RPC("SyncRevive", RpcTarget.AllViaServer);
        }

        [PunRPC]
        public void SyncRevive()
        {
            Debug.Log("Revived");
            gameObject.layer = LayerMask.NameToLayer("Player");
            state.ReviveState();
            animator.SetTrigger("Hurt");
            playerRevive.Deactivate();
        }

        Coroutine HealOverTimeRoutine;
        IEnumerator HealOverTime()
        {
            while (!state.IsDie)
            {
                yield return new WaitForSeconds(1f);
                if (Health < MaxHealth)
                {
                    Heal((int) stat.curHpRegen, false);
                }
                else
                {
                    yield return null;
                }
            }
        }
        Coroutine startHealOverTime;
        // 맞고 20초 후에 초당 회복량 다시 시작
        IEnumerator StartHealOverTime()
        {
            yield return new WaitForSeconds(20);
            HealOverTimeRoutine = StartCoroutine(HealOverTime());
        }

        public void PotionHealOverTime(float percent, float seconds)
        {
            StartCoroutine(PotionHealRoutine(percent, seconds));
        }

        private IEnumerator PotionHealRoutine(float percent, float seconds)
        {
            for (int i = 0; i < seconds; i++)
            {
                Heal(percent, true);
                yield return new WaitForSeconds(1);
            }
        }

        // 실드 생성
        public void CreateShield(int amount, float duration)
        {
            StartCoroutine(ShieldRotine(amount, duration));
        }

        IEnumerator ShieldRotine(int amount, float duration)
        {
            MaxShield = amount;
            Shield = amount;
            yield return new WaitForSeconds(duration);
            
            Shield = 0;
            MaxShield = 0;
        }
        public void TakeDotDamage(float precent, int time, bool isPrecent)
        {
            StartCoroutine(DotDamageRoutine(precent, time, isPrecent));
        }
        IEnumerator DotDamageRoutine(float precent, int time, bool isPrecent)
        {
            int count = 0;
            while (time > count)
            {
                if (isPrecent)
                {
                    TakeFixedDamage((int) (MaxHealth * precent));
                    yield return new WaitForSeconds(1f);
                    count++;
                }
                else
                {
                    TakeFixedDamage((int)precent);
                    yield return new WaitForSeconds(1f);
                    count++;
                }
            }
        }
    }
}


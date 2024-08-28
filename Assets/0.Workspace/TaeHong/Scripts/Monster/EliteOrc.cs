using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tae
{
    public class EliteOrc : BaseMonster
    {
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
                if(random <= 0.67f) // 스킬1 67%
                {
                    PlayAnim(Convert.ToByte(Animation.Attack02));
                    yield return new WaitForSeconds(1f);
                }
                else // 스킬2 33%
                {
                    PlayAnim(Convert.ToByte(Animation.Attack03));
                    StartCoroutine(DashRoutine());
                    yield return new WaitForSeconds(1f);
                }
            }

            // 공격후 잠깐 대기
            PlayAnim(Convert.ToByte(Animation.Idle));
            yield return new WaitForSeconds(1.5f);
            attackFinished = true;
        }

        private IEnumerator DashRoutine()
        {
            Vector3 direction = (playerToChase.position - transform.position).normalized;
            Vector3 originalPos = transform.position;
            Vector3 targetPos = transform.position + direction * 2f;
            float time = 0;
            while(time <= 0.5f)
            {
                transform.position = Vector3.Lerp(originalPos, targetPos, time);
                time += Time.deltaTime;
                yield return null;
            }
        }

        // Slash Attack
        public override void Attack01Frame()
        {
            BoxColliderAttackFrame(attack01Collider, 1);
        }

        // Spin Attack
        public override void Attack02Frame()
        {
            BoxColliderAttackFrame(attack02Collider, 1.3f);
        }

        // Slam Attack
        public override void Attack03Frame()
        {
            BoxColliderAttackFrame(attack03Collider, 2);
        }
    }
}
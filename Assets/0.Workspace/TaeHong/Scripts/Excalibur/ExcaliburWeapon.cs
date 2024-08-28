using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;

namespace Tae
{
    public class ExcaliburWeapon : MonoBehaviourPun
    {
        // Components
        [SerializeField] ExcaliburOwner owner;
        [SerializeField] PhotonPlayerController playerController;
        [SerializeField] BoxCollider2D hitbox;
        [SerializeField] LayerMask hittableMask;
        private Transform pivot;

        // Cached Variables
        private Vector3 mousePos;
        private Vector3 directionToMouse;
        private Vector3 rotation;
        private Vector3 downRotation;
        private Vector3 upRotation;
        private Coroutine attackResetRoutine;
        private float attackCooldownTimer;
        private float attackResetTimer;
        private Collider2D[] colliderCache = new Collider2D[10];

        // Values
        [SerializeField] float attackDuration;
        [SerializeField] float attackAngles = 90f;
        [SerializeField] float attackCooldown = 1.5f;
        [SerializeField] float attackResetTime = 3f;

        // States
        private bool isAttacking;
        private bool isSwordDown;
        private bool isFacingRight;

        private void Awake()
        {
            pivot = transform.GetChild(0);
        }

        private void Start()
        {
            downRotation = Vector3.back * attackAngles;
            upRotation = Vector3.forward * attackAngles;
        }

        private void OnEnable()
        {
            playerController.playerFlipped += FaceMouse;
        }

        private void OnDisable()
        {
            playerController.playerFlipped += FaceMouse;
            ReturnToOriginalPosition();
        }

        public void Attack()
        {
            if (attackCooldownTimer > 0)
                return;

            if (isFacingLeft)
            {
                owner.curexcaliburAttackPrefab.transform.position = new Vector2(transform.position.x - 1f, transform.position.y);
            }
            else
            {
                owner.curexcaliburAttackPrefab.transform.position = new Vector2(transform.position.x + 1f, transform.position.y);
            }
            owner.curexcaliburAttackPrefab.GetComponent<Animator>().Play("ExcaliburAttack");
            Manager.Sound.PlaySFX(Manager.Sound.SoundSO.EXAttackSFX);
            StartCoroutine(AttackCooldownRoutine());
            if (!isSwordDown)
            {
                attackResetRoutine = StartCoroutine(AttackResetRoutine());
                SlashDown();
            }
            else
            {
                StopCoroutine(attackResetRoutine);
                SlashUp();
            }
        }

        private void SlashDown()
        {
            StartSlash();
            pivot.DORotate(downRotation, attackDuration, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack).OnComplete(OnSlashFinish);
            isSwordDown = true;
        }

        private void SlashUp()
        {
            StartSlash();
            pivot.DORotate(upRotation, attackDuration, RotateMode.LocalAxisAdd).SetEase(Ease.InQuad).OnComplete(OnSlashFinish);
            isSwordDown = false;
        }

        private void StartSlash()
        {
            isAttacking = true;
            playerController.canFlip = false;
            int count = Physics2D.OverlapBoxNonAlloc(hitbox.transform.position, hitbox.size, 0, colliderCache, hittableMask);
            for (int i = 0; i < count; i++)
            {
                Debug.Log(colliderCache[i].name);
                if (colliderCache[i].TryGetComponent(out ExcaliburOwner excOwner))
                {
                    if (excOwner == owner)
                        continue;
                }

                if (colliderCache[i].TryGetComponent(out IDamageable damageable))
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        damageable.TakeDamage(16);
                        damageable.LastHitter = owner.gameObject;
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(hitbox.transform.position, hitbox.size);
        }

        private void OnSlashFinish()
        {
            isAttacking = false;
            playerController.canFlip = true;
        }

        private void ReturnToOriginalPosition()
        {
            pivot.localRotation = Quaternion.Euler(Vector3.zero);
            isSwordDown = false;
            attackCooldownTimer = 0;
            attackResetTimer = 0;
        }

        private IEnumerator AttackCooldownRoutine()
        {
            attackCooldownTimer = attackCooldown;
            while (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
                yield return null;
            }
            attackCooldownTimer = 0;
        }

        private IEnumerator AttackResetRoutine()
        {
            attackResetTimer = attackResetTime;
            while (attackResetTimer > 0)
            {
                attackResetTimer -= Time.deltaTime;
                yield return null;
            }
            attackResetTimer = 0;
            ReturnToOriginalPosition();
        }
        bool isFacingLeft = false;
        private void FaceMouse(bool isFacingLeft)
        {
            if (isAttacking)
                return;
            this.isFacingLeft = isFacingLeft;
            owner.curexcaliburAttackPrefab.GetComponent<SpriteRenderer>().flipX = isFacingLeft;

            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.y = isFacingLeft ? 180 : 0;
            transform.localRotation = Quaternion.Euler(rotation);
        }
    }
}
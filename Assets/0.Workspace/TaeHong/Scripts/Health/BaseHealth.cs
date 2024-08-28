using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Tae
{
    public class BaseHealth : MonoBehaviourPun, IDamageable, IKnockbackable
    {
        // Components
        [SerializeField] DamageTextSpawner damageTextSpawner;
        private LayerMask wallMask;

        // Properties
        private int curHealth;
        private int maxHealth;
        private int curShield;
        private int maxShield;
        public int Health { get => curHealth; set { curHealth = Math.Clamp(value, 0, maxHealth); healthChangedEvent?.Invoke(curHealth); } }
        public int MaxHealth
        {
            get => maxHealth;
            set
            {
                int diff = value > maxHealth ? (value - maxHealth) : 0;
                maxHealth = value;
                maxhealthChangedEvent?.Invoke(maxHealth);
                Health += diff;
            }
        }
        public int Shield { get => curShield; set { curShield = value; shiedChangedEvent?.Invoke(curShield); } }
        public int MaxShield { get => maxShield; set { maxShield = value; maxShiedChangedEvent?.Invoke(maxShield); } }

        public GameObject lastHitter = null;
        public GameObject LastHitter { get => lastHitter; set { lastHitter = value; SyncLastHitter(); } }

        protected bool isDead;
        public bool IsDead => isDead;

        // Events
        public event Action<int> healthChangedEvent;
        public event Action diedEvent;
        public event Action<int> maxhealthChangedEvent;
        public event Action<int> shiedChangedEvent;
        public event Action<int> maxShiedChangedEvent;
        // Methods
        protected void Awake()
        {
            wallMask = 1 << LayerMask.NameToLayer("Wall");
        }

        public virtual void TakeDamage(int amount)
        {
            if (isDead)
                return;

            if ( curShield <= 0)
            {
                Health -= amount;
                damageTextSpawner.SpawnMultText(Color.red, amount);
            }
            else
            {
                Shield -= amount;
                damageTextSpawner.SpawnText(Color.cyan, amount);
                if(curShield < 0)
                {
                    Health += curShield;
                    curShield = 0;
                }
            }

            if (curHealth <= 0)
            {
                Die();
                diedEvent?.Invoke();
            }
        }
        public virtual void TakeFixedDamage(int amount)
        {
            if (isDead)
                return;

            Health -= amount;
            damageTextSpawner.SpawnText(Color.gray, amount);
        }

        protected virtual void Die() { }

        public virtual void ApplyStun(float duration) { }
        public virtual void Heal(float amount, bool isPercent)
        {
            if (isDead)
                return;

            int amountToHeal;
            if (isPercent)
            {
                amountToHeal = (int) (amount * 0.01f * maxHealth);
            }
            else
            {
                amountToHeal = (int) amount;
            }
            Health += amountToHeal;
            damageTextSpawner.SpawnText(Color.green, amountToHeal);
        }

        public virtual void GetKnockedBack(Vector2 direction, float distance, float duration)
        {
            if (isDead)
                return;

            StartCoroutine(KnockBackRoutine(direction, distance, duration));
        }

        protected void SyncLastHitter()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            if (!PhotonNetwork.IsMasterClient)
                return;
            
            int id = 0;
            if(lastHitter.TryGetComponent<PhotonView>(out PhotonView hitterView))
            {
               id = hitterView.ViewID;
                Debug.Log($"lastHitter ID :{id}");
            }
            photonView.RPC("RPCHitter", RpcTarget.All, id);
        }

        [PunRPC]
        protected void RPCHitter(int id)
        {
            if (id == 0)
            {
                Debug.Log("Hitter Sync Fail : ID Value 0");
                return;
            }
            else
            {
                Debug.Log($"Recieve ID {id}");
                lastHitter = PhotonView.Find(id).gameObject;
            }
        }

        protected IEnumerator KnockBackRoutine(Vector2 direction, float distance, float duration)
        {
            Vector2 startPos = transform.position;
            Vector2 endPos = PlayerUtils.CheckForCollision(transform.position, direction, distance, wallMask);
            float t = 0f;
            while (t < duration)
            {
                transform.position = Vector2.Lerp(startPos, endPos, t / duration);
                t += Time.deltaTime;
                yield return null;
            }
        }

     
    }
}


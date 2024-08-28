using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tae
{
    public class MonsterHealth : BaseHealth
    {
        [SerializeField] BaseMonster monster;

        private void Awake()
        {
            base.Awake();
            MaxHealth = monster.Health;
        }

        private void OnEnable()
        {
            Manager.Game.PhaseManager.OnPhaseChanged += UpdateHealth;
        }

        private void OnDisable()
        {
            Manager.Game.PhaseManager.OnPhaseChanged -= UpdateHealth;
        }

        private void UpdateHealth(GamePhase gamePhase)
        {
            MaxHealth = monster.Health;
        }

        public override void TakeDamage(int amount)
        {
            Debug.Log($"original damage : {amount}");
            int calculatedDamage = Mathf.CeilToInt(amount * (100 - monster.Armor) * 0.01f);
            Debug.Log($"calculated damage : {calculatedDamage}");

            if (PhotonNetwork.IsConnected) 
                photonView.RPC("TakeDamageRPC", RpcTarget.All, calculatedDamage);
            else
                TakeDamageRPC(calculatedDamage);
        }

        [PunRPC]
        public void TakeDamageRPC(int amount)
        {
            if (monster.isInvincible || IsDead)
                return;

            base.TakeDamage(amount);
            monster.TakeDamage();

            if (Health <= 0)
            {
                Die();
            }
        }

        protected override void Die()
        {
            Debug.Log("DIED");
            monster.Die();
            isDead = true;
        }
    }
}
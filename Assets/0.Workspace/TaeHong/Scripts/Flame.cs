using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Tae
{
    public class Flame : MonoBehaviourPun
    {
        public int ownerID;
        [SerializeField] int flameDamage;
        [SerializeField] float damageInterval;
        Dictionary<Collider2D, Coroutine> stayOnFireRoutines = new Dictionary<Collider2D, Coroutine>();

        [PunRPC]
        public void Init(int ownerID)
        {
            this.ownerID = ownerID;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Offline
            if (!PhotonNetwork.IsConnected)
            {
                if (collision.TryGetComponent(out IDamageable damageable))
                {
                    // Prevent damage to owner
                    if (collision.GetComponent<ExcaliburOwner>() != null)
                        return;

                    damageable.TakeDamage(flameDamage);
                    Coroutine coroutine = StartCoroutine(StayOnFireRoutine(damageable));
                    stayOnFireRoutines.Add(collision, coroutine);
                }
            }
            // Online
            else
            {
                if (!PhotonNetwork.IsMasterClient)
                    return;

                if (collision.TryGetComponent(out IDamageable damageable))
                {
                    // Prevent damage to owner
                    if (collision.GetComponent<ExcaliburOwner>() != null)
                    {
                        if (collision.TryGetComponent(out PhotonView view) && view.OwnerActorNr == ownerID)
                            return;
                    }

                    damageable.TakeDamage(flameDamage);
                    Coroutine coroutine = StartCoroutine(StayOnFireRoutine(damageable));
                    stayOnFireRoutines.Add(collision, coroutine);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Offline
            if (!PhotonNetwork.IsConnected)
            {
                if (stayOnFireRoutines.ContainsKey(collision))
                {
                    StopCoroutine(stayOnFireRoutines[collision]);
                    stayOnFireRoutines.Remove(collision);
                }
            }
            // Online
            else
            {
                if (!PhotonNetwork.IsMasterClient)
                    return;

                if (stayOnFireRoutines.ContainsKey(collision))
                {
                    StopCoroutine(stayOnFireRoutines[collision]);
                    stayOnFireRoutines.Remove(collision);
                }
            }
        }

        private IEnumerator StayOnFireRoutine(IDamageable damageable)
        {
            while (true)
            {
                yield return new WaitForSeconds(damageInterval);
                damageable.TakeDamage(flameDamage);
                damageable.LastHitter = Manager.Game.MyPlayer;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Data;

namespace Tae
{
    public class PlayerRevive : MonoBehaviourPun, IPunObservable, IInteractable
    {
        [SerializeField] float reviveTime = 7;
        [SerializeField] GameObject interactionUI;
        [SerializeField] PlayerHealth playerHealth;
        [SerializeField] ReviveUI reviveUI;
        [SerializeField] int revivesRemaining = 2;
        private Coroutine reviveRoutine;

        // 플레이어가 죽으면 스크립트 활성화
        public void Activate()
        {
            photonView.RPC("SyncActivate", RpcTarget.AllViaServer, true);
        }

        // 플레이어가 죽으면 스크립트 비활성화
        public void Deactivate()
        {
            photonView.RPC("SyncActivate", RpcTarget.AllViaServer, false);
        }

        [PunRPC]
        public void SyncActivate(bool isActivate)
        {

            gameObject.SetActive(isActivate);
        }

        public void ShowUI()
        {
            interactionUI.SetActive(true);
        }

        public void HideUI()
        {
            interactionUI.SetActive(false);
            CancelRevive();
        }

        public void OnInteract(Interactor interactor)
        {
            // 부활 갯수 = 0
            if (revivesRemaining == 0)
                return;

            // 부활중 상호작용
            if (reviveRoutine != null)
            {
                CancelRevive();
            }
            else
            {
                StartRevive();
            }
        }

        // 팀원이 상호작용하면 부활시작
        private void StartRevive()
        {
            // Reviver hides interaction UI
            interactionUI.SetActive(false);

            // Revivee starts progress bar
            photonView.RPC("SyncStartRevive", photonView.Owner);
        }

        // 플레이어가 범위 밖으로 나가면 부활취소
        private void CancelRevive()
        {
            // Revivee cancels progress
            photonView.RPC("SyncCancelRevive", photonView.Owner);
        }

        // 부활시키기
        private void Revive()
        {
            playerHealth.Revive();
            photonView.RPC("SyncRevive", photonView.Owner);
        }

        [PunRPC]
        public void SyncStartRevive()
        {
            reviveRoutine = StartCoroutine(ReviveRoutine());
        }

        [PunRPC]
        public void SyncRevive()
        {
            revivesRemaining--;
            reviveUI.UpdateReviveCount(revivesRemaining);
        }

        [PunRPC]
        public void SyncCancelRevive()
        {
            if (reviveRoutine == null)
                return;

            StopCoroutine(reviveRoutine);
            reviveUI.StopProgressBar();
        }

        private IEnumerator ReviveRoutine()
        {
            float t = 0;
            reviveUI.StartProgressBar(reviveTime);
            while (t <= reviveTime)
            {
                reviveUI.UpdateProgressBar(t);
                t += Time.deltaTime;
                yield return null;
            }
            reviveUI.StopProgressBar();
            Revive();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(revivesRemaining);
            }
            else
            {
                revivesRemaining = (int)stream.ReceiveNext();
            }
        }
    }
}


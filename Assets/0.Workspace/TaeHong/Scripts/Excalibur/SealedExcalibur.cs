using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Tae.Inventory;
using ItemLootSystem;

namespace Tae
{
    public class SealedExcalibur : MonoBehaviourPun, IInteractable, ICollectable
    {
        [SerializeField] GameObject interactionUI;
        [SerializeField] GameObject pickupUI;

        [SerializeField] GameObject[] gems;
        [SerializeField] BaseItemData gemItem;

        private SpriteRenderer spriteRenderer;
        private Animator animator;
        private int gemsInserted;
        private bool canInsert = true;
        private bool excaliburTaken;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                Manager.Game.PhaseManager.OnPhaseChanged += OnPhase5;
        }

        private void Start()
        {
            if (!PhotonNetwork.IsConnected) // 오프라인(튜토리얼시) 자동 봉인해제 //08.09 13:25 PJH
                Unseal();
        }

        private void OnDisable()
        {
            Manager.Game.PhaseManager.OnPhaseChanged -= OnPhase5;
        }

        private void OnPhase5(GamePhase phase)
        {
            if (phase == GamePhase.Phase5)
            {
                if (!Manager.Event.excaliburPickedUp)
                {
                    Manager.Event.RevealExcaliburLocation(transform);
                }

                if (PhotonNetwork.IsConnected)
                {
                    // 페이즈 5일때
                    Manager.Sound.PlaySFX(Manager.Sound.SoundSO.OpenExcaliburSFX);
                    photonView.RPC("Unseal", RpcTarget.All);
                }
                else
                {
                    Unseal();
                }
            }
        }

        private void InsertGem()
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("InsertGemRPC", RpcTarget.All);
            }
            else
            {
                gems[gemsInserted].SetActive(true);
                gemsInserted++;
            }

            InventoryManager.Instance.ReduceConsumableCount(gemItem, 1);

            if (gemsInserted == 3)
            {
                if (PhotonNetwork.IsConnected)
                {
                    // 조기해금됐을때
                    Manager.Sound.PlaySFX(Manager.Sound.SoundSO.FastExcaliburSFX);
                    HideUI();
                    photonView.RPC("Unseal", RpcTarget.All);
                    ShowUI();
                }
                else
                {
                    Manager.Sound.PlaySFX(Manager.Sound.SoundSO.FastExcaliburSFX);
                    HideUI();
                    Unseal();
                    ShowUI();
                }
            }
        }

        [PunRPC]
        public void InsertGemRPC()
        {
            gems[gemsInserted].SetActive(true);
            gemsInserted++;
        }

        [PunRPC]
        public void Unseal()
        {
            canInsert = false;
        }

        [PunRPC]
        public void TakeExcalibur()
        {
            excaliburTaken = true;
            GetComponent<CircleCollider2D>().enabled = false;
            animator.Play("Off");
        }

        public void OnInteract(Interactor interactor)
        {
            // 보석이 아직 부족할때
            if (canInsert)
            {
                // 보석 1개 입력
                if (InventoryManager.Instance.GetItemCount(gemItem) > 0)
                    InsertGem();
            }
            else // 제단이 풀렸을때
            {
                OnCollect(interactor.collector);
            }
        }

        public void OnCollect(Collector collector)
        {
            if (excaliburTaken)
                return;

            collector.CollectExcalibur();
            HideUI();

            if (PhotonNetwork.IsConnected)
                photonView.RPC("TakeExcalibur", RpcTarget.All);
            else
                TakeExcalibur();
        }

        public void ShowUI()
        {
            if (canInsert)
                interactionUI.SetActive(true);
            else if (!excaliburTaken)
                pickupUI.SetActive(true);
        }

        public void HideUI()
        {
            if (canInsert)
                interactionUI.SetActive(false);
            else if (!excaliburTaken)
                pickupUI.SetActive(false);
        }
    }
}


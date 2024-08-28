using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using Tae.Inventory;

namespace Tae
{
    public class Interactor : MonoBehaviourPun
    {
        [SerializeField] LayerMask interactableMask;
        [HideInInspector] public Collector collector;
        public ExcaliburOwner excaliburOwner;

        private List<IInteractable> interactables = new List<IInteractable>();
        private IInteractable curInteractable;

        private void Awake()
        {
            collector = GetComponent<Collector>();
            excaliburOwner = GetComponentInParent<ExcaliburOwner>();
        }

        // 유저가 상호작용 키를 누를때
        private void OnInteract()
        {
            // 주인만 가능
            if (!photonView.IsMine && PhotonNetwork.IsConnected == true)
                return;

            // 상호작용 할게 없으면 리턴
            if (interactables.Count == 0)
                return;

            // 상호작용
            curInteractable.OnInteract(this);

            // Ressign current interactable
            if (curInteractable == null) // Interactable was destroyed
            {
                if (interactables.Count > 0)
                {
                    curInteractable = interactables[0];
                }
            }
            else // Interaction remained
            {
                interactables.Add(curInteractable);
                interactables.RemoveAt(0);
                curInteractable = interactables[0];
            }
        }
        
        // 범위에 들어오면
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!interactableMask.Contain(collision.gameObject.layer))
                return;

            if (collision.TryGetComponent(out IInteractable interactable))
            {
                if (curInteractable == null)
                    curInteractable = interactable;
                interactables.Add(interactable);
                interactable.ShowUI();
            }
        }

        // 범위에서 나가면
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!interactableMask.Contain(collision.gameObject.layer))
                return;

            if (collision.TryGetComponent(out IInteractable interactable))
            {
                interactable.HideUI();
                interactables.Remove(interactable);
                if(interactables.Count == 0)
                {
                    curInteractable = null;
                }
                else
                {
                    curInteractable = interactables[0];
                }
            }
        }
    }
}
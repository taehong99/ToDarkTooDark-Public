using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tae
{
    public class Exit : MonoBehaviour, IInteractable
    {
        [SerializeField] GameObject interactionUI;


        public event Action OnExitInteract;

        public void OnInteract(Interactor interactor)
        {
            if (!interactor.excaliburOwner.enabled || this != Manager.Game.MyExit)
                return;

            if(PhotonNetwork.IsConnected)
                interactor.excaliburOwner.GameOver();

            OnExitInteract?.Invoke();
        }

        public void ShowUI()
        {
            interactionUI.SetActive(true);
        }

        public void HideUI()

        {
            interactionUI.SetActive(false);
        }
    }
}
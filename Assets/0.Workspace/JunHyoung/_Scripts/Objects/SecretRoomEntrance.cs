using ItemLootSystem;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;


namespace MapGenerator
{
    // from Special Room to SecretRoom
    public class SecretRoomEntrance : MonoBehaviour, IInteractable ,IPunInstantiateMagicCallback
    {
        //출구 위치
        public Vector3 endPos;

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
          object[] instantiationData = info.photonView.InstantiationData;
            if (instantiationData != null && instantiationData.Length == 3)
            {
                float x = (float) instantiationData[0];
                float y = (float) instantiationData[1];
                float z = (float) instantiationData[2];
                endPos = new Vector3(x, y, z);
            }
        }

        void IInteractable.HideUI()
        {
            
        }

        void IInteractable.OnInteract(Interactor interactor)
        {
           interactor.transform.position = endPos;
        }

        void IInteractable.ShowUI()
        {
            
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Tae
{
    public class PlayerNavigation : MonoBehaviourPun, IOnEventCallback
    {
        public GuideArrow guideArrow;
        public Transform excaliburTrans;
        private void Awake()
        {
            guideArrow = GetComponentInChildren<GuideArrow>(true);
        }

        private void Start()
        {
            //fix this
            Manager.Game.playerDic.Add(photonView.ViewID, gameObject);
            //Debug.Log($"add viewID : {photonView.ViewID}");
        }

        private void OnEnable()
        {
            Manager.Event.ExcaliburLocationRevealEvent += NavigateToExcalibur;
            Manager.Event.ExcaliburLocationRevealEvent += FindExcaliburTrans;
            Manager.Event.ExcaliburPickUpEvent += OnExcaliburPickUp;
            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            Manager.Event.ExcaliburLocationRevealEvent -= NavigateToExcalibur;
            Manager.Event.ExcaliburLocationRevealEvent -= FindExcaliburTrans;
            Manager.Event.ExcaliburPickUpEvent -= OnExcaliburPickUp;
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void NavigateToExcalibur(Transform target)
        {
            guideArrow.StartNavigation(target);
        }

        private void OnExcaliburPickUp(int viewID)
        {
            if(photonView.ViewID == viewID)
            {
                Manager.Event.excaliburPickedUp = true;
                guideArrow.StartNavigation(Manager.Game.MyExit.transform);
                return;
            }

            
            if (!PhotonNetwork.IsConnected)   // 오프라인시(튜토리얼시) 픽업 이벤트 처리 // 08.09 13:00 PJH
            {
                Manager.Event.excaliburPickedUp = true;
                guideArrow.StartNavigation(Manager.Game.MyExit.transform);
            }
        }

        private void FindExcaliburTrans(Transform target)
        {
            excaliburTrans = target;
        }

        public void OnEvent(EventData photonEvent)
        {
            if(photonEvent.Code == EventManager.ExcaliburPickedUpEventCode)
            {
                int viewID = (int) photonEvent.CustomData;
                guideArrow.StartNavigation(Manager.Game.playerDic[viewID].transform);
            }
        }
        public void MagicCompassScroll()
        {
            StartCoroutine(DurationRoutine());
        }
        IEnumerator DurationRoutine()
        {
            guideArrow.StartNavigation(excaliburTrans);
            yield return new WaitForSeconds(3f);
            guideArrow.StopNavigation();

        }

    }
}
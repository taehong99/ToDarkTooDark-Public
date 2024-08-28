using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System;

public class FaintTimer : MonoBehaviourPun, IPunObservable
{
    [SerializeField] float faintTime = 25;
    [SerializeField] TextMeshProUGUI timer;
    public float elapsedTime;

    private Action OnTimerFinish;

    private Coroutine timerRoutine;

    private void Update()
    {
        timer.text = Mathf.FloorToInt(elapsedTime).ToString();
    }

    public void StartTimer(Action action = null)
    {
        if (!photonView.IsMine)
            return;

        gameObject.SetActive(true);
        if(action != null)
            OnTimerFinish = action;
        timerRoutine = StartCoroutine(TimerRoutine());
    }

    public void StopTimer()
    {
        if(timerRoutine != null)
        {
            StopCoroutine(timerRoutine);
            timerRoutine = null;
            OnTimerFinish = null;
            gameObject.SetActive(false);
        }
    }

    private IEnumerator TimerRoutine()
    {
        elapsedTime = faintTime;
        while(elapsedTime >= 0)
        {
            elapsedTime -= Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        OnTimerFinish?.Invoke();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameObject.activeSelf);
            stream.SendNext(elapsedTime);
        }
        else
        {
            gameObject.SetActive((bool) stream.ReceiveNext());
            elapsedTime = (float) stream.ReceiveNext();
        }
    }
}

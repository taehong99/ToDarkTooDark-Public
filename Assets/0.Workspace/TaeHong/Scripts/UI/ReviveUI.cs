using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class ReviveUI : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Slider bar;
    [SerializeField] TextMeshProUGUI reviveText;
    [SerializeField] TextMeshProUGUI reviveCountText;
    [SerializeField] TextMeshProUGUI reviveCount;

    public void UpdateReviveCount(int count)
    {
        photonView.RPC("SyncReviveCount", RpcTarget.All, count);
    }

    [PunRPC]
    public void SyncReviveCount(int count)
    {
        if (count == 0)
        {
            reviveText.color = Color.red;
            reviveCountText.color = Color.red;
            reviveCount.color = Color.red;
        }

        reviveCount.text = count.ToString();
    }

    public void StartProgressBar(float maxValue)
    {
        bar.gameObject.SetActive(true);
        bar.maxValue = maxValue;
    }

    public void UpdateProgressBar(float value)
    {
        bar.value = value;
    }

    public void StopProgressBar()
    {
        bar.gameObject.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(bar.gameObject.activeInHierarchy);
            stream.SendNext(bar.maxValue);
            stream.SendNext(bar.value);
        }
        else
        {
            bar.gameObject.SetActive((bool) stream.ReceiveNext());
            bar.maxValue = (float) stream.ReceiveNext();
            bar.value = (float) stream.ReceiveNext();
        }
    }
}

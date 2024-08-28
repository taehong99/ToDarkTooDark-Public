using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExcaliburSpawnTile : MonoBehaviour
{
    [SerializeField] GameObject excaliburPrefab;

    private void OnEnable()
    {
        Manager.Event.SpawnExcaliburEvent += SpawnExcalibur;
    }

    private void OnDisable()
    {
        Manager.Event.SpawnExcaliburEvent -= SpawnExcalibur;
    }

    public void SpawnExcalibur()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateRoomObject("ExcaliburSeal", transform.position, Quaternion.identity);
        }
    }
}

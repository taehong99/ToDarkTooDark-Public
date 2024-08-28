using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScene : BaseScene
{
    public override IEnumerator LoadingRoutine()
    {
        while (PhotonNetwork.InRoom)
            yield return null;
        Debug.Log("Load Main");
    }

    public void LoadRoom()
    {
        Manager.Scene.LoadScene("11 New Room Scene");
    }
}

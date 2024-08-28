using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class RoomScene : BaseScene
{
    [SerializeField] NewWaitingRoom waitingRoomPanel;
    public override IEnumerator LoadingRoutine()
    {
        waitingRoomPanel.gameObject.SetActive(true);
        yield return null;
        waitingRoomPanel.gameObject.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Manager.Scene.LoadScene("11 New Main Scene");
    }
}

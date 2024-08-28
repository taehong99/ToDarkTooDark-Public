using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using static MainMenuPanelController;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class WaitingRoomPhoton : MonoBehaviourPunCallbacks
{
    public bool GameMode { get; set; }

    [Header("Panel")]
    [SerializeField] NewWaitingRoom waitingRoomPanel;

    [Header("Hider")]
    [SerializeField] public GameObject connectionLoadPanel;

    [Header("Scene")]
    [SerializeField] RoomScene roomScene;

    #region 방 출입 관련
    public override void OnLeftRoom()
    {
        roomScene.LoadMainMenu();
        Debug.Log("Leave Room");
    }
    #endregion

    #region 로비 관련
    public override void OnJoinedLobby()
    {
        Debug.Log("Join Lobby");
    }
    #endregion

    #region 방 관련
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        waitingRoomPanel.PlayerEnterRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        waitingRoomPanel.PlayerLeftRoom(otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        waitingRoomPanel.MasterClientSwitched(newMasterClient);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        if(PhotonNetwork.InRoom)
            waitingRoomPanel.PlayerPropertiesUpdate(targetPlayer, changedProps);
    }
    #endregion

}

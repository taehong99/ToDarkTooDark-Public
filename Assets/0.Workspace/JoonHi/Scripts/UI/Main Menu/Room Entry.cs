using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    [SerializeField] TMP_Text Host;
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text roomMode;
    [SerializeField] TMP_Text currentPlayer;
    [SerializeField] Button roomButton;
    public LobbyPanel lobbyPanel;

    public RoomInfo roomInfo;

    private void Start()
    {
        roomButton.onClick.AddListener(SetRoom);
    }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        //Host.text = PhotonNetwork.PlayerList[roomInfo.masterClientId].NickName;

        roomName.text = roomInfo.Name;
        /*
        Debug.Log(roomInfo.CustomProperties.Keys.Count);
        foreach (string key in roomInfo.CustomProperties.Keys)
        {
            Debug.Log($"{roomInfo.Name} : {key}");
        }
        */
        if (roomInfo.CustomProperties.ContainsKey("GAMEMODE"))
        {
            bool gamemode = (bool) roomInfo.CustomProperties["GAMEMODE"];
            roomMode.text = gamemode ? "싱글 모드" : "팀 모드";
        }
        if (roomInfo.CustomProperties.ContainsKey("MASTERNAME"))
        {
            string hostname = (string) roomInfo.CustomProperties["MASTERNAME"];
            Host.text = hostname;
        }

        currentPlayer.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
    }

    public void JoinRoom()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomInfo.Name);
    }

    private void SetRoom()
    {
        lobbyPanel.joinRoomButton.onClick.RemoveAllListeners();
        lobbyPanel.joinRoomButton.interactable = roomInfo.PlayerCount < roomInfo.MaxPlayers;
        lobbyPanel.joinRoomButton.onClick.AddListener(JoinRoom);
    }
}

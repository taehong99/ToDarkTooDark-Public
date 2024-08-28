using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewRoomEntry : MonoBehaviour
{
    [SerializeField] Text Host;
    [SerializeField] Text roomName;
    [SerializeField] Text roomMode;
    [SerializeField] Text currentPlayer;
    [SerializeField] Button roomButton;

    [SerializeField] GameObject waitingPanel;
    [SerializeField] GameObject gameingPanel;
    public NewLobbyPanel LobbyPanel;

    public RoomInfo roomInfo;

    private void Start()
    {
        roomButton.onClick.AddListener(JoinRoom);
    }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;

        roomName.text = roomInfo.Name;
        if (roomInfo.CustomProperties.ContainsKey("GAMEMODE"))
        {
            bool gamemode = (bool) roomInfo.CustomProperties["GAMEMODE"];
            roomMode.text = gamemode ? "개인전" : "팀 모드";
        }
        if (roomInfo.CustomProperties.ContainsKey("MASTERNAME"))
        {
            string hostname = (string) roomInfo.CustomProperties["MASTERNAME"];
            Host.text = hostname;
        }
        if(roomInfo.CustomProperties.ContainsKey("GAMESTART"))
        {
            bool gamestate = (bool) roomInfo.CustomProperties["GAMESTART"];
            waitingPanel.SetActive(!gamestate);
            gameingPanel.SetActive(gamestate);
        }

        currentPlayer.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
        roomButton.interactable = roomInfo.PlayerCount < roomInfo.MaxPlayers;
    }

    public void JoinRoom()
    {
        //bool gamestate = (bool) roomInfo.CustomProperties["GAMESTART"];
        //if (gamestate == false && roomInfo.PlayerCount < roomInfo.MaxPlayers)
        //{
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinRoom(roomInfo.Name);
        //}
        //else if (gamestate == false && roomInfo.PlayerCount >= roomInfo.MaxPlayers)
        //{
        //    LobbyPanel.FullRoom();
        //}
    }

    /*
    private void SetRoom()
    {
        lobbyPanel.joinRoomButton.onClick.RemoveAllListeners();
        lobbyPanel.joinRoomButton.interactable = roomInfo.PlayerCount < roomInfo.MaxPlayers;
        lobbyPanel.joinRoomButton.onClick.AddListener(JoinRoom);
    }
    */
}

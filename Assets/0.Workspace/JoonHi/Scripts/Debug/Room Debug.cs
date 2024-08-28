using JH;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDebug : MonoBehaviourPunCallbacks
{
    [SerializeField] WaitingRoomCanvasPanelController waitingRoom;
    [SerializeField] NewWaitingRoom waitingRoom2;

    [SerializeField] bool debug;

    void Start()
    {
        if (debug)
        {
            PhotonNetwork.LocalPlayer.NickName = $"DebugPlayer {Random.Range(1000, 10000)}";
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
        
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master server. Attempting to join a random room...");
        string Name = $"To Dark, Too Dark {Random.Range(1000, 10000)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 4 };
        // PhotonNetwork.JoinOrCreateRoom($"PhotonTestRoom {Random.Range(1000, 10000)}", roomOptions, TypedLobby.Default);
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: Name, roomOptions: options);
    }

    public override void OnJoinedRoom()
    {
        // Called when successfully joined a room
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        // waitingRoom.gameObject.SetActive(true); 
        waitingRoom2.gameObject.SetActive(true);
    }

    public override void OnCreatedRoom()
    {
        // Called when a new room is successfully created
        Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // Called when disconnected from the server
        Debug.LogWarning($"Disconnected from Photon server: {cause}");
    }
}

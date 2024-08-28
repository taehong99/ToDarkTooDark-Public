using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

namespace JH
{
    public class MainCanvasPanelController : MonoBehaviourPunCallbacks
    {
        public enum MainCanvasPanel { Main, FastMatch, Lobby, Character, RoomCreate, Room }

        [Header("Hider")]
        [SerializeField] GameObject connectionLoadPanel;

        [Header("Panels")]
        [SerializeField] MainMenuPanel mainMenuPanel;
        [SerializeField] FastMatchPanel fastMatchPanel;
        [SerializeField] LobbyPanel lobbyPanel;
        [SerializeField] CreateRoomPanel createRoom;
        [Space]
        [SerializeField] RoomTempPanel roomtempPanel;

        public bool GameMode { get; set; }

        private ClientState state;

        private void Start()
        {
            connectionLoadPanel.SetActive(true);
            SetActivePanel(MainCanvasPanel.Main);

            GameMode = true;
        }

        private void Update()
        {
            ClientState curState = PhotonNetwork.NetworkClientState;
            if (state == curState)
                return;
            state = curState;
            Debug.Log(state);
        }

        public void SetActivePanel(MainCanvasPanel panel)
        {
            mainMenuPanel.gameObject.SetActive(panel == MainCanvasPanel.Main);
            fastMatchPanel.gameObject.SetActive(panel == MainCanvasPanel.FastMatch);
            lobbyPanel.gameObject.SetActive(panel == MainCanvasPanel.Lobby || panel == MainCanvasPanel.RoomCreate);
            createRoom.gameObject.SetActive(panel == MainCanvasPanel.RoomCreate);

            roomtempPanel.gameObject.SetActive(panel == MainCanvasPanel.Room);
        }

        #region photon 연결 여부
        public override void OnConnected()
        {
            connectionLoadPanel.SetActive(false);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (cause == DisconnectCause.ApplicationQuit)
                return;
            connectionLoadPanel.SetActive(true);
        }
        #endregion

        #region 방생성 관련
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Create room failed with error : {message}({returnCode})");
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Create room success");

            // 시작시 게임모드 확인 및 설정
            PhotonNetwork.CurrentRoom.SetGameMode(GameMode);
        }
        #endregion

        #region 방 출입 관련
        public override void OnJoinedRoom()
        {
            SetActivePanel(MainCanvasPanel.Room);
            Debug.Log("Join Room");

            // 만약 Random Join을 했을때 내가 MasterClient(생성자)인 경우
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetGameMode(GameMode);

                // 시작시 랜덤 시드 설정
                int seed = Random.Range(0, 10000);
                PhotonNetwork.CurrentRoom.SetSeed(seed);
                Debug.Log($"Created Room Seed : {seed}");
            }
            else
            {
                Debug.Log($"Joined Room Seed : {PhotonNetwork.CurrentRoom.GetSeed()}");
            }
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Join random room failed with error : {message}({returnCode})");
        }
        public override void OnLeftRoom()
        {
            SetActivePanel(MainCanvasPanel.Main);
            Debug.Log("Leave Room");
        }
        #endregion

        #region 로비 관련
        public override void OnJoinedLobby()
        {
            SetActivePanel(MainCanvasPanel.Lobby);
            Debug.Log("Join Lobby");
        }
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            lobbyPanel.UpdateRoomList(roomList);
        }
        public override void OnLeftLobby()
        {
            SetActivePanel(MainCanvasPanel.Main);
            Debug.Log("Leave Lobby");
        }
        #endregion

        #region 방 관련
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            roomtempPanel.PlayerEnterRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            roomtempPanel.PlayerLeftRoom(otherPlayer);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            roomtempPanel.MasterClientSwitched(newMasterClient);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
        {
            roomtempPanel.PlayerPropertiesUpdate(targetPlayer, changedProps);
        }
        #endregion
    }
}
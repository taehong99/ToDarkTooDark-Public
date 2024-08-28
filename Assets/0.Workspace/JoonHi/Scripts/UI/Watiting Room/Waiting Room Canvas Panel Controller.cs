using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

namespace JH
{
    public class WaitingRoomCanvasPanelController : MonoBehaviourPunCallbacks
    {
        [Header("Panels")]
        [SerializeField] SettingPopUpPanel settingPanel;

        [SerializeField] RectTransform playerContent;
        [SerializeField] PlayerEntry2 playerEntryPrefab;

        [Header("Buttons")]
        [SerializeField] Button startButton;
        [SerializeField] Button readyButton;
        [Space]
        [SerializeField] Button backButton;

        [Header("Room Info")]
        public TextMeshProUGUI roomName;

        private List<PlayerEntry2> playerList;

        private void Awake()
        {
            playerList = new List<PlayerEntry2>();
            backButton.onClick.AddListener(LeaveRoom);
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                PlayerEntry2 playerEntry = Instantiate(playerEntryPrefab, playerContent);
                playerEntry.playerReadyButton = readyButton;
                playerEntry.SetPlayer(player);
                playerList.Add(playerEntry);
            }

            roomName.text = PhotonNetwork.CurrentRoom.Name;

            // PhotonNetwork.IsMasterClient  내가 방의 방장인지
            startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            PhotonNetwork.LocalPlayer.SetReady(false);
            PhotonNetwork.LocalPlayer.SetLoad(false);
            AllPlayerReadyCheck();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void OnDisable()
        {
            for (int i = 0; i < playerContent.childCount; i++)
            {
                Destroy(playerContent.GetChild(i).gameObject);
            }
            playerList.Clear();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public void StartGame()
        {
            // 게임 화면 로딩 및 이동
            // PhotonNetwork.LoadLevel("GameScene");
        }

        public void PlayerEnterRoom(Player newPlayer)
        {
            PlayerEntry2 playerEntry = Instantiate(playerEntryPrefab, playerContent);
            playerEntry.SetPlayer(newPlayer);
            playerList.Add(playerEntry);

            AllPlayerReadyCheck();
        }

        public void PlayerLeftRoom(Player otherPlayer)
        {
            PlayerEntry2 playerEntry = null;
            foreach (PlayerEntry2 entry in playerList)
            {
                if (entry.Player.ActorNumber == otherPlayer.ActorNumber)
                {
                    playerEntry = entry;
                }
            }
            playerList.Remove(playerEntry);
            Destroy(playerEntry.gameObject);

            AllPlayerReadyCheck();
        }

        public void MasterClientSwitched(Player newMasterClient)
        {
            startButton.gameObject.SetActive(newMasterClient.IsLocal);
            newMasterClient.SetReady(true);
            readyButton.gameObject.SetActive(false);
            // 방 호스트 병경
            if (PhotonNetwork.InRoom)
            {
                Debug.Log($"Host Changed to {newMasterClient}");
                ExitGames.Client.Photon.Hashtable setValue = PhotonNetwork.CurrentRoom.CustomProperties;
                setValue["MASTERNAME"] = newMasterClient.NickName;
                PhotonNetwork.CurrentRoom.SetCustomProperties(setValue);
            }
        }

        public void PlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
        {
            PlayerEntry2 playerEntry = null;
            foreach (PlayerEntry2 entry in playerList)
                if (entry.Player.ActorNumber == targetPlayer.ActorNumber)
                {
                    playerEntry = entry;
                }
            playerEntry.ChangeCustomProperty(changedProps);

            AllPlayerReadyCheck();
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void AllPlayerReadyCheck()
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;

            int readyCount = 0;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.GetReady())
                {
                    readyCount++;
                }
            }

            if (readyCount == PhotonNetwork.PlayerList.Length)
            {
                startButton.interactable = true;
            }
            else
            {
                startButton.interactable = false;
            }
        }

        #region 네트워크 관련
        public override void OnJoinedRoom()
        {
            Debug.Log("Join Room");

            // 만약 Random Join을 했을때 내가 MasterClient(생성자)인 경우
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetGameMode(true);

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
            Debug.Log("Leave Room");
            PhotonNetwork.JoinLobby();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.LogError("PlayerEnter");
            PlayerEnterRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PlayerLeftRoom(otherPlayer);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            MasterClientSwitched(newMasterClient);
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
        {
            PlayerPropertiesUpdate(targetPlayer, changedProps);
        }
         
        public override void OnJoinedLobby()
        {
            Debug.Log("Join Lobby");
            // Manager.Lobby.nextMenu = LobbyManager.MenuState.Lobby;
            // Scene을 메인화면으로 전환 및 Lobby 활성화
        }

        /*
        public override void OnLeftLobby()
        {
            Debug.Log("Leave Lobby");
        }*/
        #endregion

#if UNITY_EDITOR
        [ContextMenu("Debug Game Mode")]
        private void DebugGameMode()
        {
            Debug.Log(PhotonNetwork.CurrentRoom.GetGameMode());
        }

        [ContextMenu("Debug Game Mode Change")]
        private void DebugGameModeChange()
        {
            PhotonNetwork.CurrentRoom.SetGameMode(!PhotonNetwork.CurrentRoom.GetGameMode());
        }

#endif
    }
}
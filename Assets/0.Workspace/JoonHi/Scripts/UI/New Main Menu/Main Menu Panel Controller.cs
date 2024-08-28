using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class MainMenuPanelController : MonoBehaviourPunCallbacks
{
    public enum MainPanel { SignIn, Main, Lobby, Room }

    [Header("Hider")]
    [SerializeField] public GameObject connectionLoadPanel;

    [Header("Panels")]
    [SerializeField] SignInPanel signinPanel;
    [SerializeField] NewMainMenuPanel mainMenuPanel;
    [SerializeField] NewLobbyPanel lobbyPanel;
    [SerializeField] RoomPanel roomPanel;
    [SerializeField] SettingsPanel settingsPanel;
    // [SerializeField] ExitPopUpPanel exitPanel;

    [Header("Scene")]
    [SerializeField] MainMenuScene mainScene;

    public bool GameMode { get; set; }

    private ClientState state;

    private void Start()
    {
        //connectionLoadPanel.SetActive(true);
        if(FirebaseManager.Auth == null || FirebaseManager.Auth.CurrentUser == null)
            SetActivePanel(MainPanel.SignIn);
        else if(!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();

        GameMode = true;

        lobbyPanel.gameObject.SetActive(false);
        roomPanel.gameObject.SetActive(false);
    }

    private void Update()
    {
        ClientState curState = PhotonNetwork.NetworkClientState;
        if (state == curState)
            return;
        state = curState;
        Debug.Log(state);
    }

    public void SetActivePanel(MainPanel panel)
    {
        Debug.Log($"SetActivePanel: {panel}");
        signinPanel.gameObject.SetActive(panel == MainPanel.SignIn);
        mainMenuPanel.gameObject.SetActive(panel == MainPanel.Main);
        lobbyPanel.gameObject.SetActive(panel == MainPanel.Lobby);
        roomPanel.gameObject.SetActive(panel == MainPanel.Room);
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.gameObject.SetActive(true);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        Debug.Log("GameQuit");
#endif
        Application.Quit();
    }

    #region photon 연결 여부
    public override void OnConnected()
    {
        Manager.Sound.PlayBGM(Manager.Sound.SoundSO.MainBGM);
        SetActivePanel(MainPanel.Main);
        mainMenuPanel.Welcome();
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
        //PhotonNetwork.CurrentRoom.SetGameMode(GameMode);
        //PhotonNetwork.CurrentRoom.SetPlayers(new int[4]);

        //// 시작시 랜덤 시드 설정
        //int seed = Random.Range(0, 10000);
        //PhotonNetwork.CurrentRoom.SetSeed(seed);

        // Manager.Scene.LoadScene("11 New Room Scene");
        //mainScene.LoadRoom();
    }
    #endregion

    #region 방 출입 관련
    public override void OnJoinedRoom()
    {
        // SetActivePanel(MainCanvasPanel.Room);
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

        SetActivePanel(MainPanel.Room);
        roomPanel.JoinedRoom();
        //mainScene.LoadRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join random room failed with error : {message}({returnCode})");
    }
    public override void OnLeftRoom()
    {
        SetActivePanel(MainPanel.Lobby);
        Debug.Log("Leave Room");
    }
    #endregion

    #region 로비 관련
    public override void OnJoinedLobby()
    {
        SetActivePanel(MainPanel.Lobby);
        connectionLoadPanel.gameObject.SetActive(false);
        Debug.Log("Join Lobby");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobbyPanel.UpdateRoomList(roomList);
    }
    public override void OnLeftLobby()
    {
        SetActivePanel(MainPanel.Main);
        connectionLoadPanel.gameObject.SetActive(false);
        Debug.Log("Leave Lobby");
    }
    #endregion

    #region 방 관련
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomPanel.PlayerEnterRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomPanel.PlayerLeftRoom(otherPlayer);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        roomPanel.MasterClientSwitched(newMasterClient);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        roomPanel.PlayerPropertiesUpdate(targetPlayer, changedProps);
    }

    public override void OnRoomPropertiesUpdate(PhotonHashTable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        roomPanel.RoomPropertiesUpdate(propertiesThatChanged);
    }
    #endregion
}

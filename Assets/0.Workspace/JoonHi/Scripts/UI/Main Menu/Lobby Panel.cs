using JH;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static JH.MainCanvasPanelController;

public class LobbyPanel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] MainCanvasPanelController mainCanvasPanelController;

    [Header("Panels")]
    [SerializeField] SettingPopUpPanel settingPanel;

    [Header("Buttons")]
    [SerializeField] Button settingButton;
    [SerializeField] Button backButton;
    [SerializeField] Button reloadLobbyButton;
    [Space]
    [SerializeField] Button createRoomButton;
    public Button joinRoomButton;

    [Header("Room List")]
    [SerializeField] RectTransform roomContent;
    [SerializeField] RoomEntry roomEntryPrefab;

    private Dictionary<string, RoomEntry> roomDictionary;
    public RoomEntry seletedRoom { get; set; }

    private void Awake()
    {
        roomDictionary = new Dictionary<string, RoomEntry>();
    }

    private void Start()
    {
        settingButton.onClick.AddListener(ShowSetting);
        backButton.onClick.AddListener(BacktoMain);
        reloadLobbyButton.onClick.AddListener(ReloadLobby);

        createRoomButton.onClick.AddListener(CreateRoom);
    }

    private void OnDisable()
    {
        for (int i = 0; i < roomContent.childCount; i++)
        {
            Destroy(roomContent.GetChild(i).gameObject);
        }
        roomDictionary.Clear();
    }
    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

    public void ReloadLobby()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        // Update room info
        foreach (RoomInfo roomInfo in roomList)
        {
            // 1. 방잉 사라지는 경우
            if (roomInfo.RemovedFromList || roomInfo.IsOpen == false || roomInfo.IsVisible == false)
            {
                if (roomDictionary.ContainsKey(roomInfo.Name) == false)
                    continue;

                RoomEntry roomEntry = roomDictionary[roomInfo.Name];
                roomDictionary.Remove(roomInfo.Name);
                Destroy(roomEntry);

                continue;
            }
            // 2. 방이 내용물이 바뀌는 경우
            if (roomDictionary.ContainsKey(roomInfo.Name))
            {
                // Dictionary에 있다는 뜻은 있었던 방이라는 뜻
                RoomEntry roomEntry = roomDictionary[roomInfo.Name];
                roomEntry.SetRoomInfo(roomInfo);
            }
            // 3. 방이 생기는 경우
            else
            {
                // Dictionary에 없다는 뜻은 없었던 방이라는 뜻
                RoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);
                roomEntry.lobbyPanel = this;
                roomEntry.SetRoomInfo(roomInfo);
                roomDictionary.Add(roomInfo.Name, roomEntry);
            }
        }
    }

    public void BacktoMain()
    {
        mainCanvasPanelController.SetActivePanel(MainCanvasPanel.Main);
    }

    public void CreateRoom()
    {
        mainCanvasPanelController.SetActivePanel(MainCanvasPanel.RoomCreate);
    }

    public void JoinRoom()
    {
        if(seletedRoom.roomInfo.PlayerCount < seletedRoom.roomInfo.MaxPlayers)
        {
            // PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinRoom(seletedRoom.roomInfo.Name);
        }
    }

    #region Show Popup

    public void ShowSetting()
    {
        settingPanel.ShowSetting();
    }
    #endregion
}

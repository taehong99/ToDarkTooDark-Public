using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NewLobbyPanel : MonoBehaviour
{
    [Header("Panel Controller")]
    [SerializeField] MainMenuPanelController panelController;

    [Header("Buttons")]
    [SerializeField] Button soloModeButton;
    [SerializeField] Button teamModeButton;
    [SerializeField] Button fastMatchButton;
    [SerializeField] Button createRoomButton;
    [Space]
    [SerializeField] Button reloadLobbyButton;
    [Space]
    [SerializeField] Button mainButton;
    [SerializeField] Button emoticonButton;
    [SerializeField] Button shopButton;
    [SerializeField] Button charactorButton;
    [SerializeField] Button settingButton;
    [Space]
    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;

    [Header("PopUp Panel")]
    [SerializeField] NewFastMatchPanel fastMatchPanel;
    [SerializeField] NewCreateRoomPanel createRoomPanel;
    [SerializeField] OutShopPanel shopPanel;
    [SerializeField] CharacterStoryMainPanel characterStoryPanel;
    [SerializeField] ConfirmPanel fullPanel;
    [SerializeField] ConfirmPanel battlePanel;

    [Header("Room List")]
    [SerializeField] RectTransform roomContent;
    [SerializeField] NewRoomEntry roomEntryPrefab;

    private bool solofilter;
    private bool teamfilter;

    private Dictionary<string, NewRoomEntry> roomDictionary;

    private void Awake()
    {
        roomDictionary = new Dictionary<string, NewRoomEntry>();
    }

    private void Start()
    {
        Manager.Sound.PlayBGM(Manager.Sound.SoundSO.LobbyBGM);

        ReloadLobby();

        soloModeButton.onClick.AddListener(Solo);
        teamModeButton.onClick.AddListener(Team);
        fastMatchButton.onClick.AddListener(FastMatch);
        createRoomButton.onClick.AddListener(CreateRoom);
        
        reloadLobbyButton.onClick.AddListener(ReloadLobby);
        
        mainButton.onClick.AddListener(ToMain);
        // emoticonButton.onClick.AddListener();
        shopButton.onClick.AddListener(Shop);
        charactorButton.onClick.AddListener(Charactor);
        settingButton.onClick.AddListener(Setting);

        //gameObject.SetActive(false);
    }

    private void Solo()
    {
        switch(solofilter)
        {
            case true:
                solofilter = false;
                break;
            case false:
                solofilter = true;
                teamfilter = false;
                break;
        }
        ReloadLobby();
    }

    private void Team()
    {
        switch (teamfilter)
        {
            case true:
                teamfilter = false;
                break;
            case false:
                solofilter = false;
                teamfilter = true;
                break;
        }
        ReloadLobby();
    }

    private void FastMatch()
    {
        fastMatchPanel.gameObject.SetActive(true);
    }

    private void CreateRoom()
    {
        createRoomPanel.gameObject.SetActive(true);
    }

    private void ReloadLobby()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby();
    }

    private void ToMain()
    {
        //PhotonNetwork.LeaveLobby();
        panelController.SetActivePanel(MainMenuPanelController.MainPanel.Main);
        Manager.Sound.PlayBGM(Manager.Sound.SoundSO.MainBGM);
    }

    private void Shop()
    {
        shopPanel.gameObject.SetActive(true);
        Manager.Sound.PlayBGM(Manager.Sound.SoundSO.StoreBGM);
    }

    private void Charactor()
    {
        characterStoryPanel.gameObject.SetActive(true);
    }

    private void Setting()
    {

    }

    private void OnDisable()
    {
        for (int i = 0; i < roomContent.childCount; i++)
        {
            Destroy(roomContent.GetChild(i).gameObject);
        }
        roomDictionary.Clear();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        // Update room info
        foreach (RoomInfo roomInfo in roomList)
        {
            // 1. 방이 사라지는 경우
            if (roomInfo.RemovedFromList || roomInfo.IsOpen == false || roomInfo.IsVisible == false)
            {
                if (roomDictionary.ContainsKey(roomInfo.Name) == false)
                    continue;

                NewRoomEntry roomEntry = roomDictionary[roomInfo.Name];
                roomDictionary.Remove(roomInfo.Name);
                Destroy(roomEntry);

                continue;
            }
            // 2. 필터링 확인
            if (solofilter || teamfilter)   // 필터링이 활성화 되어있는 경우
            {
                bool gamemode = (bool) roomInfo.CustomProperties["GAMEMODE"];
                Debug.Log($"{roomInfo.Name} : {(gamemode ? "개인전" : "팀 모드")}");
                if (solofilter == true && gamemode == true)
                {
                    // 계속
                }
                else if (teamfilter == true && gamemode == false)
                {
                    // 계속
                }
                else    // 필터링에 걸리는 경우
                {
                    continue;
                }
            }
            // 3. 방이 내용물이 바뀌는 경우
            if (roomDictionary.ContainsKey(roomInfo.Name))
            {
                // Dictionary에 있다는 뜻은 있었던 방이라는 뜻
                NewRoomEntry roomEntry = roomDictionary[roomInfo.Name];
                roomEntry.SetRoomInfo(roomInfo);
            }
            // 4. 방이 생기는 경우
            else
            {
                // Dictionary에 없다는 뜻은 없었던 방이라는 뜻
                NewRoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);
                roomEntry.SetRoomInfo(roomInfo);
                roomEntry.LobbyPanel = this;
                roomDictionary.Add(roomInfo.Name, roomEntry);
            }
        }
    }

    public void FullRoom()
    {
        fullPanel.gameObject.SetActive(true);
    }
    public void BattleRoom()
    {
        fullPanel.gameObject.SetActive(true);
    }
}

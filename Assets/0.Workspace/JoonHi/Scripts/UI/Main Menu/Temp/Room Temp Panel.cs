using JH;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class RoomTempPanel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] MainCanvasPanelController mainCanvasPanelController;

    [Header("Panels")]
    [SerializeField] SettingPopUpPanel settingPanel;
    [SerializeField] CharacterSelectPanel selectPanel;

    [SerializeField] RectTransform playerContent;
    [SerializeField] PlayerEntry playerEntryPrefab;

    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button readyButton;
    [Space]
    [SerializeField] Button backButton;
    [SerializeField] Button characterSelectButton;

    [Header("Room Info")]
    public TextMeshProUGUI roomName;

    private List<PlayerEntry> playerList;

    private void Awake()
    {
        playerList = new List<PlayerEntry>();
        backButton.onClick.AddListener(LeaveRoom);
        startButton.onClick.AddListener(StartGame);
        characterSelectButton.onClick.AddListener(ShowCharacterSelect);
    }

    private void OnEnable()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
            playerEntry.playerReadyButton = readyButton;
            //playerEntry.SetPlayer(player);
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
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    /*
    public void BacktoLobby()
    {
        mainCanvasPanelController.SetActivePanel(MainCanvasPanel.Lobby);
    }
    */

    public void StartGame()
    {
        startButton.onClick.RemoveAllListeners();
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("CBT_Build");
    }

    public void PlayerEnterRoom(Player newPlayer)
    {
        PlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerContent);
        //playerEntry.SetPlayer(newPlayer);
        playerList.Add(playerEntry);

        AllPlayerReadyCheck();
    }

    public void PlayerLeftRoom(Player otherPlayer)
    {
        PlayerEntry playerEntry = null;
        foreach (PlayerEntry entry in playerList)
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
            // ExitGames.Client.Photon.Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            // roomProps["MASTERNAME"] = newMasterClient.NickName;
            // PhotonNetwork.CurrentRoom.SetCustomProperties(newMasterClient.NickName, ["MASTERNAME"]);
            ExitGames.Client.Photon.Hashtable setValue = PhotonNetwork.CurrentRoom.CustomProperties;
            setValue["MASTERNAME"] = newMasterClient.NickName;
            PhotonNetwork.CurrentRoom.SetCustomProperties(setValue);
        }
    }

    public void PlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        PlayerEntry playerEntry = null;
        foreach (PlayerEntry entry in playerList)
        if (entry.Player.ActorNumber == targetPlayer.ActorNumber)
        {
            playerEntry = entry;
        }

        // Legacy
        //playerEntry.ChangeCustomProperties(changedProps);

        AllPlayerReadyCheck();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        // PhotonNetwork.JoinLobby();
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

    private void ShowCharacterSelect()
    {
        selectPanel.gameObject.SetActive(true);
    }

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

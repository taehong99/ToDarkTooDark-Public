using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;
using System;

public class RoomPanel : MonoBehaviour
{
    [Header("Panel Controller")]
    [SerializeField] MainMenuPanelController panelController;

    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button readyButton;
    [SerializeField] Button cancelReadyButton;
    [SerializeField] Button characterSelectButton;
    [SerializeField] Button settingsButton;
    [SerializeField] Button roomSettingsButton;
    [SerializeField] Button exitButton;

    [Header("Player Entries")]
    [SerializeField] PlayerEntry redPlayer1Entry;
    [SerializeField] PlayerEntry redPlayer2Entry;
    [SerializeField] PlayerEntry bluePlayer1Entry;
    [SerializeField] PlayerEntry bluePlayer2Entry;
    private PlayerEntry[] playerEntries = new PlayerEntry[4];
    
    [Header("Panels")]
    [SerializeField] RoomSettingPanel roomSettingPanel;
    [SerializeField] CharacterSettingPanel characterSelectPanel;

    [Header("Texts")]
    [SerializeField] Text roomName;
    [SerializeField] Text roomMode;

    private void Awake()
    {
        playerEntries[0] = redPlayer1Entry;
        playerEntries[1] = bluePlayer1Entry;
        playerEntries[2] = redPlayer2Entry;
        playerEntries[3] = bluePlayer2Entry;
    }

    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
        readyButton.onClick.AddListener(Ready);
        cancelReadyButton.onClick.AddListener(CancelReady);
        characterSelectButton.onClick.AddListener(ShowCharacterSelectPanel);
        roomSettingsButton.onClick.AddListener(ShowRoomSettingsPanel);
        exitButton.onClick.AddListener(LeaveRoom);
    }

    private void OnEnable()
    {
        // Show Player Entries when entering room
        UpdatePlayerEntries();

        // Update room info
        UpdateRoomInfo();

        // Activate button depending on role
        SetupButtons();

        PhotonNetwork.AutomaticallySyncScene = true;
        AllPlayersReadyCheck();
    }

    private void OnDisable()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    public void JoinedRoom()
    {
        PlayerEnterRoom(PhotonNetwork.LocalPlayer);
    }

    public void PlayerEnterRoom(Player newPlayer)
    {
        PhotonNetwork.LocalPlayer.SetReady(PhotonNetwork.IsMasterClient);
        Room curRoom = PhotonNetwork.CurrentRoom;
        Player player = newPlayer;
        int index = curRoom.GetOpenSlotIdx();

        // Assign player's team
        Team team = index % 2 == 0 ? Team.Red : Team.Blue;
        player.SetTeam(team);

        // Update current room player list
        int[] players = curRoom.GetPlayers();
        players[index] = newPlayer.ActorNumber;
        curRoom.SetPlayers(players);

        AllPlayersReadyCheck();
    }

    public void PlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.CurrentRoom.RemovePlayer(otherPlayer.ActorNumber);
        AllPlayersReadyCheck();
    }

    public void MasterClientSwitched(Player newMasterClient)
    {
        // 내가 새로운 방장이면
        if(PhotonNetwork.LocalPlayer == newMasterClient)
        {
            SetupButtons();
        }
        else
        {
            CancelReady();
        }
    }

    public void PlayerPropertiesUpdate(Player targetPlayer, PhotonHashTable changedProps)
    {
        foreach (var key in changedProps.Keys)
        {
            Debug.Log("Property key: " + key + ", new value: " + changedProps[key]);
        }

        PlayerEntry playerEntry = null;
        foreach (PlayerEntry entry in playerEntries)
        {
            if (entry.Player == null)
                continue;

            if (entry.Player == targetPlayer)
            {
                playerEntry = entry;
            }
        }

        if (changedProps.ContainsKey(CustomProperty.JOB))
        {
            Debug.Log("Player Job Updated");
            playerEntry?.UpdateSprite();
        }

        if (changedProps.ContainsKey(CustomProperty.READY))
        {
            Debug.Log("Player Ready Updated");
            playerEntry?.UpdateNameColor();
        }

        AllPlayersReadyCheck();
    }

    public void RoomPropertiesUpdate(PhotonHashTable changedProps)
    {
        foreach (var key in changedProps.Keys)
        {
            Debug.Log("Property key: " + key + ", new value: " + changedProps[key]);
        }

        if (changedProps.ContainsKey(CustomProperty.GAMEMODE))
        {
            string newGameMode = (bool)changedProps[CustomProperty.GAMEMODE] == true ? "개인전" : "팀전";
            Debug.Log("Game Mode Updated: " + newGameMode);
            roomMode.text = newGameMode;
        }
        else if (changedProps.ContainsKey(CustomProperty.PLAYERS))
        {
            UpdatePlayerEntries();
        }
    }

    #region Button Events
    public void StartGame()
    {
        startButton.interactable = false; // StartGame 한번만 실행
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("CBT_Build");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ShowCharacterSelectPanel()
    {
        characterSelectPanel.gameObject.SetActive(true);
    }

    public void ShowRoomSettingsPanel()
    {
        roomSettingPanel.gameObject.SetActive(true);
    }

    private void Ready()
    {
        readyButton.gameObject.SetActive(false);
        cancelReadyButton.gameObject.SetActive(true);
        PhotonNetwork.LocalPlayer.SetReady(true);
    }

    private void CancelReady()
    {
        readyButton.gameObject.SetActive(true);
        cancelReadyButton.gameObject.SetActive(false);
        PhotonNetwork.LocalPlayer.SetReady(false);
    }
    #endregion

    // Utils
    [ContextMenu("UpdatePlayerEntries")]
    private void UpdatePlayerEntries()
    {
        Debug.Log("Update Player Entries");
        int[] players = PhotonNetwork.CurrentRoom.GetPlayers();
        Debug.Log($"Players = {players[0]} {players[1]} {players[2]} {players[3]}");
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != -1)
            {
                Player player = PhotonNetwork.CurrentRoom.GetPlayer(players[i]);
                Team team = i % 2 == 0 ? Team.Red : Team.Blue;
                playerEntries[i].SetPlayer(player, team);
            }
            else
            {
                playerEntries[i].RemovePlayer();
            }
        }
    }

    private void SetupButtons()
    {
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        readyButton.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
        cancelReadyButton.gameObject.SetActive(false);
        roomSettingsButton.interactable = PhotonNetwork.IsMasterClient;

        // Only master
        if (PhotonNetwork.IsMasterClient)
            startButton.interactable = false;
    }

    private void UpdateRoomInfo()
    {
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        roomMode.text = (bool) PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"] == true ? "개인전" : "팀전";
    }

    public void AllPlayersReadyCheck()
    {
        // Only room owner needs to check if everyone is ready
        if (!PhotonNetwork.IsMasterClient)
            return;

        // Count how many players are ready
        Player[] playerList = PhotonNetwork.PlayerList;
        int readyCount = 0;
        foreach (var player in playerList)
        {
            if (player.GetReady())
            {
                readyCount++;
            }
        }

        // If everyone is ready, owner can start game
        bool allReady = readyCount == playerList.Length;
        bool minReady = readyCount >= (PhotonNetwork.CurrentRoom.GetGameMode() ? 2 : 2);

        if (allReady && minReady)
            startButton.interactable = true;
        else
            startButton.interactable = false;
    }
}

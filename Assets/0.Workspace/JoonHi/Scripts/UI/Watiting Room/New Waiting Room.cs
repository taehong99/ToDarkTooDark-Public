using JH;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;


public class NewWaitingRoom : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] SettingPopUpPanel settingPanel;

    [SerializeField] RectTransform playerContent;
    [SerializeField] PlayerEntry2 playerEntryPrefab;

    [Header("Buttons")]
    [SerializeField] Button startButton;
    [SerializeField] Button readyButton;
    [SerializeField] Button characterButton;
    [SerializeField] Button roomSettingButton;
    [Space]
    [SerializeField] Button backToLobbyButton;

    [Header("PopUp Panel")]
    [SerializeField] RoomSettingPanel roomSettingPanel;
    [SerializeField] CharacterSettingPanel characterSettingPanel;

    [Header("Room Info")]
    public Text roomName;
    [SerializeField] Text roomMode;

    private List<PlayerEntry2> playerList;

    private void Awake()
    {
        playerList = new List<PlayerEntry2>();

        startButton.onClick.AddListener(StartGame);
        readyButton.onClick.AddListener(Ready);
        // characterButton.onClick.AddListener();
        roomSettingButton.onClick.AddListener(Setting);

        backToLobbyButton.onClick.AddListener(LeaveRoom);
    }

    private void OnEnable()
    {
        ChangeRoomMode();
        Debug.Log("Enabled");
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayerEntry2 playerEntry = Instantiate(playerEntryPrefab, playerContent);
            playerEntry.playerReadyButton = readyButton;
            playerEntry.SetPlayer(player);
            playerList.Add(playerEntry);
        }

        roomName.text = PhotonNetwork.CurrentRoom.Name;

        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);
        AllPlayerReadyCheck();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void StartGame()
    {
        startButton.onClick.RemoveAllListeners();
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("CBT_Build");
    }

    private void Setting()
    {

    }

    private void Ready()
    {
        PhotonNetwork.LocalPlayer.SetReady(!PhotonNetwork.LocalPlayer.GetReady());
    }

    public void PlayerEnterRoom(Player newPlayer)
    {
        Debug.Log("EnterRoom");
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
        {
            startButton.gameObject.SetActive(false);
            return;
        }

        startButton.gameObject.SetActive(true);
        readyButton.gameObject.SetActive(false);
        PhotonNetwork.LocalPlayer.SetReady(true);
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

    private void ChangeRoomMode()
    {
        roomMode.text = PhotonNetwork.CurrentRoom.GetGameMode() ? "개인전" : "팀 전";
    }

    private void ShowCharacterSelect()
    {
        characterSettingPanel.gameObject.SetActive(true);
    }
}

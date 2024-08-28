using JH;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewCreateRoomPanel : MonoBehaviour
{
    [Header("Panel Controller")]
    [SerializeField] MainMenuPanelController panelController;

    [Header("Buttons")]
    [SerializeField] Button soloModeButton;
    [SerializeField] Button teamModeButton;
    [SerializeField] Toggle secretRoomButton;
    [Space]
    [SerializeField] Button createRoomButton;
    [SerializeField] Button cancelCreateRoomButton;

    [Header("InputField")]
    [SerializeField] InputField roomNameInputField;
    [SerializeField] InputField roomPWInputField;

    private bool gamemode;
    public bool secretRoom { get; private set; }

    private void Start()
    {
        soloModeButton.onClick.AddListener(SetSingleMode);
        teamModeButton.onClick.AddListener(SetDualMode);
        // secretRoomButton.onValueChanged.AddListener(SetRoomSecret);
        createRoomButton.onClick.AddListener(CreateRoomConfirm); 

        cancelCreateRoomButton.onClick.AddListener(CreateRoomCancel);

        //gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        SetSingleMode();
        // secretRoomButton.isOn = false;
        // secretRoom = false;
    }

    public void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text;
        if (roomName == "")
        {
            roomName = $"To Dark, Too Dark {Random.Range(1000, 10000)}";
        }

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;

        ExitGames.Client.Photon.Hashtable property = new()
        {
            { "GAMEMODE", panelController.GameMode },
            { "MASTERNAME", PhotonNetwork.LocalPlayer.NickName },
            { CustomProperty.PLAYERS, new int[] { -1, -1, -1, -1 } }
        };
        options.CustomRoomProperties = property;
        options.CustomRoomPropertiesForLobby = new string[] { "GAMEMODE", "MASTERNAME", CustomProperty.PLAYERS };
        PhotonNetwork.CreateRoom(roomName, options);
        gameObject.SetActive(false);
        // roomNameInputField.text = "";
        // roomPWInputField.text = "";
        // secretRoomButton.isOn = false;
        // secretRoom = false;
    }

    public void CreateRoomCancel()
    {
        // CreateRoom 상태 초기화
        roomNameInputField.text = "";
        // roomPWInputField.text = "";
        // secretRoomButton.isOn = false;
        // secretRoom = false;
        gameObject.SetActive(false);
    }

    public void SetSingleMode()
    {
        soloModeButton.targetGraphic.color = soloModeButton.colors.selectedColor;
        teamModeButton.targetGraphic.color = teamModeButton.colors.disabledColor;
        //soloModeButton.enabled = false;
        //soloModeButton.interactable = false;
        //teamModeButton.enabled = true;
        //teamModeButton.interactable = true;

        panelController.GameMode = true;
    }

    public void SetDualMode()
    {
        teamModeButton.targetGraphic.color = teamModeButton.colors.selectedColor;
        soloModeButton.targetGraphic.color = soloModeButton.colors.disabledColor;
        //soloModeButton.enabled = true;
        //soloModeButton.interactable = true;
        //teamModeButton.enabled = false;
        //teamModeButton.interactable = false;

        panelController.GameMode = false;
    }

    public void SetRoomSecret(bool secret)
    {
        secretRoom = secret;
    }
}

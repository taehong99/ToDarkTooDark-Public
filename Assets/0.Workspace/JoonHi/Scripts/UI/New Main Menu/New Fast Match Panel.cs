using JH;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.CinemachineTriggerAction.ActionSettings;

public class NewFastMatchPanel : MonoBehaviour
{
    [Header("Panel Controller")]
    [SerializeField] MainMenuPanelController panelController;

    [Header("Buttons")]
    [SerializeField] Button soloModeButton;
    [SerializeField] Button teamModeButton;
    [Space]
    [SerializeField] Button fastStartButton;
    [SerializeField] Button cancelfaststartButton;
    [Space]
    [SerializeField] Button charactorChangeButton;

    [Header("Panels")]
    [SerializeField] CharacterSettingPanel charactersettingPanel;

    [Header("Character")]
    [SerializeField] Text characterName;
    [SerializeField] Image characterImage;

    private void Awake()
    {
        //gameObject.SetActive(false);
    }

    private void Start()
    {
        soloModeButton.onClick.AddListener(SetSingleMode);
        teamModeButton.onClick.AddListener(SetDualMode);

        fastStartButton.onClick.AddListener(RandomMatching);
        cancelfaststartButton.onClick.AddListener(FastStartCancel);

        charactorChangeButton.onClick.AddListener(CharacterSelect);
    }

    private void OnEnable()
    {
        //ChangeCharacter();
        charactersettingPanel.OnChangeJob.AddListener(ChangeCharacter);
        SetSingleMode();
    }

    private void OnDisable()
    {
        charactersettingPanel.OnChangeJob.RemoveListener(ChangeCharacter);
    }

    public void FastStartCancel()
    {
        gameObject.SetActive(false);
    }

    public void RandomMatching()
    {
        string Name = $"To Dark, Too Dark {Random.Range(1000, 10000)}";
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
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: Name, roomOptions: options);
        gameObject.SetActive(false);
    }

    public void SetSingleMode()
    {
        soloModeButton.targetGraphic.color = soloModeButton.colors.selectedColor;
        teamModeButton.targetGraphic.color = teamModeButton.colors.disabledColor;
        // soloModeButton.enabled = false;
        //soloModeButton.interactable = false;
        // teamModeButton.enabled = true;
        //teamModeButton.interactable = true;

        panelController.GameMode = true;
    }

    public void SetDualMode()
    {
        teamModeButton.targetGraphic.color = teamModeButton.colors.selectedColor;
        soloModeButton.targetGraphic.color = soloModeButton.colors.disabledColor;
        // soloModeButton.enabled = true;
        //soloModeButton.interactable = true;
        // teamModeButton.enabled = false;
        //teamModeButton.interactable = false;

        panelController.GameMode = false;
    }

    private void CharacterSelect()
    {
        charactersettingPanel.gameObject.SetActive(true);
    }

    public void ChangeCharacter()
    {
        PlayerJob job = PhotonNetwork.LocalPlayer.GetJob();
        foreach (CharacterSelectUI chara in charactersettingPanel.charList)
        {
            if (chara.charadata.job == job)
            {
                chara.SetSelect();
                characterImage.sprite = chara.charadata.CharacterSprite;
                characterName.text = chara.charadata.CharacterName;
            }
            else
            {
                chara.SetNonSelect();
            }
        }
    }
}

using JH;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static JH.MainCanvasPanelController;  

namespace JH
{
    public class CreateRoomPanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] MainCanvasPanelController mainCanvasPanelController;

        [Header("Panels")]
        [SerializeField] SettingPopUpPanel settingPanel;
        [SerializeField] RoomTempPanel roomTempPanel;

        [Header("Buttons")]
        [SerializeField] Button settingButton;
        [SerializeField] Button backButton;
        [Space]
        [SerializeField] Button createRoomButton;
        [Space]
        [SerializeField] Button singleModeButton;
        [SerializeField] Button dualTeamModeButton;

        [Header("InputField")]
        [SerializeField] TMP_InputField roomNameInputField;

        private void Start()
        {
            settingButton.onClick.AddListener(ShowSetting);
            backButton.onClick.AddListener(CreateRoomCancel);

            createRoomButton.onClick.AddListener(CreateRoomConfirm);

            singleModeButton.onClick.AddListener(SetSingleMode);
            dualTeamModeButton.onClick.AddListener(SetDualMode);
        }

        private void OnEnable()
        {
            SetSingleMode();
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

            ExitGames.Client.Photon.Hashtable property = new ();
            property.Add("GAMEMODE", mainCanvasPanelController.GameMode);
            property.Add("MASTERNAME", PhotonNetwork.LocalPlayer.NickName);
            options.CustomRoomProperties = property;
            options.CustomRoomPropertiesForLobby = new string[] { "GAMEMODE", "MASTERNAME" };
            PhotonNetwork.CreateRoom(roomName, options);
            roomNameInputField.text = "";
        }

        public void CreateRoomCancel()
        {
            // CreateRoom 상태 초기화
            roomNameInputField.text = "";
            mainCanvasPanelController.SetActivePanel(MainCanvasPanel.Lobby);
        }

        public void SetSingleMode()
        {
            singleModeButton.enabled = false;
            singleModeButton.interactable = false;
            dualTeamModeButton.enabled = true;
            dualTeamModeButton.interactable = true;
            mainCanvasPanelController.GameMode = true;
        }

        public void SetDualMode()
        {
            singleModeButton.enabled = true;
            singleModeButton.interactable = true;
            dualTeamModeButton.enabled = false;
            dualTeamModeButton.interactable = false;
            mainCanvasPanelController.GameMode = false;
        }

        #region Show Popup

        public void ShowSetting()
        {
            settingPanel.ShowSetting();
        }
        #endregion
    }
}
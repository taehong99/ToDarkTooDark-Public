using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static JH.MainCanvasPanelController;

namespace JH
{
    public class MainMenuPanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] MainCanvasPanelController mainCanvasPanelController;

        [Header("Panels")]
        [SerializeField] ShopPopUpPanel shopPanel;
        [SerializeField] AttendancePopUpPanel attendancePanel;
        [SerializeField] DeveloperNotesPopUpPanel developerNotesPanel;
        [SerializeField] SettingPopUpPanel settingPanel;
        [SerializeField] ExitPopUpPanel exitPanel;

        [Header("Buttons")]
        [SerializeField] Button nameConfirm;
        [SerializeField] Button shopButton;
        [SerializeField] Button attendanceButton;
        [SerializeField] Button developerNotesButton;
        [SerializeField] Button settingButton;
        [SerializeField] Button exitButton;
        [Space]
        [SerializeField] Button fastMatchButton;
        [SerializeField] Button LobbyButton;

        [Header("Input Field")]
        [SerializeField] TMP_InputField idInputField;

        [Header("Debug")]
        [SerializeField] TextMeshProUGUI debugName;

        private void Start()
        {
            nameConfirm.onClick.AddListener(ChangeName);

            shopButton.onClick.AddListener(ShowShop);
            attendanceButton.onClick.AddListener(ShowAttendance);
            developerNotesButton.onClick.AddListener(ShowDeveloperNotes);
            settingButton.onClick.AddListener(ShowSetting);
            exitButton.onClick.AddListener(ShowExit);

            fastMatchButton.onClick.AddListener(Fastmatch);
            LobbyButton.onClick.AddListener(Lobby);

            idInputField.text = $"Player {Random.Range(1000, 10000)}";
            Login();

            debugName.gameObject.SetActive(false);
            #if UNITY_EDITOR
            debugName.gameObject.SetActive(true);
            #endif
        }

        public void Fastmatch()
        {
            // PhotonNetwork.JoinLobby();
            mainCanvasPanelController.SetActivePanel(MainCanvasPanel.FastMatch);
        }

        public void Lobby()
        {
            PhotonNetwork.JoinLobby();
        }

        public void Login()
        {
            if (idInputField.text == "")
            {
                Debug.LogError("Empty nickname : Please input Name");
                return;
            }

            PhotonNetwork.LocalPlayer.NickName = idInputField.text;
            PhotonNetwork.ConnectUsingSettings();
            NameChanged();
            return;
        }

        public void ChangeName()
        {
            if (idInputField.text == "")
            {
                Debug.LogError("Empty nickname : Please input Name");
                return;
            }
            PhotonNetwork.LocalPlayer.NickName = idInputField.text;
            NameChanged();
            return;
        }

        #region Show Popup

        public void ShowShop()
        {
            shopPanel.ShowShop();
        }
        public void ShowAttendance()
        {
            attendancePanel.ShowAttendance();
        }
        public void ShowDeveloperNotes()
        {
            developerNotesPanel.ShowDeveloperNotes();
        }
        public void ShowSetting()
        {
            settingPanel.ShowSetting();
        }
        public void ShowExit()
        {
            exitPanel.ShowExit();
        }
        #endregion

        #region Debug
        public void NameChanged()
        {
            debugName.text = $"{PhotonNetwork.LocalPlayer.NickName} {PhotonNetwork.LocalPlayer.UserId}";
        }
        #endregion
    }
}
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static JH.MainCanvasPanelController;

namespace JH
{
    public class FastMatchPanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] MainCanvasPanelController mainCanvasPanelController;

        [Header("Panels")]
        [SerializeField] SettingPopUpPanel settingPanel;
        [SerializeField] CharacterSelectPanel selectPanel;

        [Header("Buttons")]
        [SerializeField] Button settingButton;
        [SerializeField] Button backButton;
        [Space]
        [SerializeField] Button characterSelectButton;
        [SerializeField] Button singleModeButton;
        [SerializeField] Button dualTeamModeButton;
        [Space]
        [SerializeField] Button randomMatching;

        // 안태홍 추가 
        [Header("PlayerSelect")]
        private PlayerJobData data;
        [SerializeField] JobDataSO jobData;
        [SerializeField] TextMeshProUGUI jobTitleText;
        [SerializeField] Image jobImage;

        private void Start()
        {
            settingButton.onClick.AddListener(ShowSetting);
            backButton.onClick.AddListener(BacktoMain);

            characterSelectButton.onClick.AddListener(ShowCharacterSelect);
            singleModeButton.onClick.AddListener(SetSingleMode);
            dualTeamModeButton.onClick.AddListener(SetDualMode);

            randomMatching.onClick.AddListener(RandomMatching);
        }

        private void OnEnable()
        {
            SetSingleMode();
            UpdatePlayerPanel();
        }

        public void RandomMatching()
        {
            string Name = $"To Dark, Too Dark {Random.Range(1000, 10000)}"; 
            RoomOptions options = new RoomOptions() { MaxPlayers = 4 };
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: Name, roomOptions: options);
        }

        public void BacktoMain()
        {
            mainCanvasPanelController.SetActivePanel(MainCanvasPanel.Main);
        }

        public void UpdatePlayerPanel()
        {
            data = jobData.GetJobData(PhotonNetwork.LocalPlayer.GetJob());
            jobTitleText.text = data._name;
            jobImage.sprite = data.sprite;
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

        private void ShowCharacterSelect()
        {
            selectPanel.gameObject.SetActive(true);
        }
        #endregion
    }
}

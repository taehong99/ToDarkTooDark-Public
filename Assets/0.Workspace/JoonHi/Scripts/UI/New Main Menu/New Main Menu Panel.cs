using JH;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewMainMenuPanel : MonoBehaviour
{
    [Header("Panel Controller")]
    [SerializeField] MainMenuPanelController panelController;

    [Header("Buttons")]
    [SerializeField] Button LobbyButton;
    [SerializeField] Button CreditButton;
    [SerializeField] Button TutorialButton;
    [SerializeField] Button settingButton;
    [SerializeField] Button exitButton;

    [Header("PopUp Panel")]
    [SerializeField] NicknamePanel nicknamePanel;

    [Header("Text")]
    [SerializeField] Text playerNameText;
    [SerializeField] string tutorialSceneName;

    private void Start()
    {
        LobbyButton.onClick.AddListener(ToLobby);
        CreditButton.onClick.AddListener(Credit);
        TutorialButton.onClick.AddListener(Tutorial);
        settingButton.onClick.AddListener(Setting);
        exitButton.onClick.AddListener(panelController.Quit);

        gameObject.SetActive(false);
        playerNameText.gameObject.SetActive(false);
    }

    private void ToLobby()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
            return;
        panelController.connectionLoadPanel.gameObject.SetActive(true);
        PhotonNetwork.JoinLobby();
    }

    private void Credit()
    {

    }

    private void Tutorial()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private void Setting()
    {
        
    }

    private IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(tutorialSceneName);

        panelController.connectionLoadPanel.SetActive(true);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void NewPlayer()
    {
        nicknamePanel.gameObject.SetActive(true);
    }

    public void Welcome()
    {
        playerNameText.gameObject.SetActive(true);
        playerNameText.text = $"\"{PhotonNetwork.LocalPlayer.NickName}\" 님!";
    }
}

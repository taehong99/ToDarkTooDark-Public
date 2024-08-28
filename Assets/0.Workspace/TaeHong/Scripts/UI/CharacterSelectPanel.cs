using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using JH;

public class CharacterSelectPanel : MonoBehaviour
{
    [SerializeField] FastMatchPanel fastMatchPanel;
    [SerializeField] JobDataSO jobData;
    [SerializeField] Button swordsmanSelectButton;
    [SerializeField] Button archerSelectButton;
    [SerializeField] Button closeButton;

    private void Awake()
    {
        swordsmanSelectButton.onClick.AddListener(SelectSwordsman);
        archerSelectButton.onClick.AddListener(SelectArcher);
        closeButton.onClick.AddListener(Close);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void SelectSwordsman()
    {
        PhotonNetwork.LocalPlayer.SetJob(PlayerJob.Swordsman);
        fastMatchPanel.UpdatePlayerPanel();
        Close();
    }

    private void SelectArcher()
    {
        PhotonNetwork.LocalPlayer.SetJob(PlayerJob.Archer);
        fastMatchPanel.UpdatePlayerPanel();
        Close();
    }
}

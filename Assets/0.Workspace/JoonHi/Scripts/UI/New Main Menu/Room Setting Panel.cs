using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomSettingPanel : MonoBehaviour
{
    [SerializeField] Dropdown modeDropdown;

    [SerializeField] Button applyButton;
    [SerializeField] Button closeButton;
    [SerializeField] Button cancelButton;

    private void Awake()
    {
        applyButton.onClick.AddListener(ApplyGameMode);
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        modeDropdown.value = PhotonNetwork.CurrentRoom.GetGameMode() ? 0 : 1;
    }

    private void ApplyGameMode()
    {
        if(modeDropdown.value == 0) // 개인전
        {
            if(PhotonNetwork.CurrentRoom.GetGameMode() == false)
                PhotonNetwork.CurrentRoom.SetGameMode(true);
        }
        else // 팀전
        {
            if (PhotonNetwork.CurrentRoom.GetGameMode() == true)
                PhotonNetwork.CurrentRoom.SetGameMode(false);
        }

        gameObject.SetActive(false);
    }
}

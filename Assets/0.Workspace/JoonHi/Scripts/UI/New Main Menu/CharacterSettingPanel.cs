using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterSettingPanel : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] Image selectedCharacterImage;

    [Header("Buttons")]
    [SerializeField] Button confirmButton;
    [SerializeField] Button closeButton;

    [Header("Text")]
    [SerializeField] Text selectedCharacterNameText;

    public List<CharacterSelectUI> charList = new List<CharacterSelectUI>();
    private PlayerJob curJob;

    public UnityEvent OnChangeJob = new UnityEvent();

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
        confirmButton.onClick.AddListener(ConfirmJob);
    }

    private void OnEnable()
    {
        curJob = PhotonNetwork.LocalPlayer.GetJob();
        UpdateCharacter(curJob);
    }

    public void UpdateCharacter(PlayerJob job)
    {
        curJob = job;
        foreach (CharacterSelectUI chara in charList)
        {
            if (chara.charadata.job == job)
            {
                chara.SetSelect();
                selectedCharacterImage.sprite = chara.charadata.CharacterSprite;
                selectedCharacterNameText.text = chara.charadata.CharacterName;
            }
            else 
            {
                chara.SetNonSelect();
            }
        }
    }

    private void ConfirmJob()
    {
        PhotonNetwork.LocalPlayer.SetJob(curJob);
        Debug.Log($"Job Changed to {PhotonNetwork.LocalPlayer.GetJob()}");
        OnChangeJob?.Invoke();
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Data")]
    public CharacterInfoData charadata;

    [Header("UI")]
    [SerializeField] Text charName;
    [SerializeField] Image charImage;
    [SerializeField] GameObject selectimage;
    [SerializeField] GameObject selectedimage;
    [SerializeField] Image background;
    [SerializeField] Color unselectColor;
    [SerializeField] Color selectColor;
    [SerializeField] Button selectButton;
    [SerializeField] GameObject LockPanel;

    [SerializeField] CharacterSettingPanel characterSettingPanel;

    private void Start()
    {
        charName.text = charadata.CharacterName;
        charImage.sprite = charadata.CharacterSprite;

        selectButton.onClick.AddListener(Updatedata);
        selectButton.interactable = false;
    }

    private void OnEnable()
    {
        IsLock();
    }

    private void Updatedata()
    {
        characterSettingPanel.UpdateCharacter(charadata.job);
    }

    public void SetNonSelect()
    {
        selectimage.SetActive(true);
        selectedimage.SetActive(false);
        background.color = unselectColor;
    }

    public void SetSelect()
    {
        selectimage.SetActive(false);
        selectedimage.SetActive(true);
        background.color = selectColor;
    }

    private void IsLock()
    {
        string userID = FirebaseManager.Auth.CurrentUser.UserId;
        FirebaseManager.DB
            .GetReference("Userdata")
            .Child(userID)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("취소");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("오류");
                    return;
                }
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();

                Userdata userData = JsonUtility.FromJson<Userdata>(json);
                // Debug.Log($"Priest : {userData.priest}");
                // Debug.Log($"Wizard : {userData.wizard}");
                LockButton(userData);
            }
            );
    }

    private void LockButton(Userdata userData)
    {
        switch(charadata.job)
        {
            case PlayerJob.Priest:
                if (userData.priest == false)
                {
                    selectButton.interactable = false;
                    LockPanel.SetActive(true);
                }
                else
                {
                    selectButton.interactable = true;
                    LockPanel.SetActive(false);
                }
                break;
            case PlayerJob.Wizard:
                if (userData.wizard == false)
                {
                    selectButton.interactable = false;
                    LockPanel.SetActive(true);
                }
                else
                {
                    selectButton.interactable = true;
                    LockPanel.SetActive(false);
                }
                break;
            default:
                selectButton.interactable = true;
                LockPanel.SetActive(false);
                break;
        }
    }
}

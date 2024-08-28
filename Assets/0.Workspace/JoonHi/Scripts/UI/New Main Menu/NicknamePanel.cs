using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Extensions;
using JH;
using Firebase.Database;
using Photon.Pun;

public class NicknamePanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Button confirmButton;
    [SerializeField] TMP_InputField nicknameInputField;

    [Header("Panels")]
    [SerializeField] NewMainMenuPanel mainMenuPanel;

    private void Start()
    {
        nicknameInputField.onValueChanged.AddListener(enableButton);
        confirmButton.onClick.AddListener(SaveNewUser);

        gameObject.SetActive(false);
    }

    public void enableButton(string name)
    {
        if (name.Length > 0)
            confirmButton.enabled = true;
        else
            confirmButton.enabled = false;
    }

    private void SaveNewUser()
    {
        Debug.Log("Saving New User Data");
        string userId = FirebaseManager.Auth.CurrentUser.UserId;

        Userdata user = new Userdata();
        user.nickName = nicknameInputField.text;

        string json = JsonUtility.ToJson(user);

        Debug.Log(userId);

        FirebaseManager.DB
            .GetReference("Userdata")
            .Child(userId)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }
                else if (task.IsFaulted)
                {
                    return;
                }
            }
            );

        Debug.Log("Save Data Success!");
        LoadUser(userId);
        //mainMenuPanel.Welcome();
        gameObject.SetActive(false);
    }

    private void LoadUser(string userID) //데이터 불러오기
    {
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
                Debug.Log(json);

                if (json == null)
                {
                    mainMenuPanel.NewPlayer();
                    return;
                }

                Userdata userData = JsonUtility.FromJson<Userdata>(json);
                Debug.Log(userData.nickName);
                Debug.Log(userData.token);
                Debug.Log(userData.excaliver);

                PhotonNetwork.LocalPlayer.NickName = userData.nickName;
                //mainMenuPanel.Welcome();
            }
            );
    }
}

using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
public class CharacterStoryMainPanel : MonoBehaviour
{
    [Header("Toggle")]
    [SerializeField] Toggle perchaseToggle;
    [Space]
    [SerializeField] Toggle swordToggle;
    [SerializeField] Toggle wandToggle;
    [SerializeField] Toggle bowToggle;

    [Header("Buttons")]
    [SerializeField] Button exitButton;

    [SerializeField] List<CharacterStorySelectPanel> characterList = new List<CharacterStorySelectPanel>();

    private Userdata userData;

    private void OnEnable()
    {
        initSetting();

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

                userData = JsonUtility.FromJson<Userdata>(json);

                ChangeList(true);
            }
            );
    }

    private void Start()
    {
        perchaseToggle.onValueChanged.AddListener(ChangeList);
        swordToggle.onValueChanged.AddListener(ChangeList);
        wandToggle.onValueChanged.AddListener(ChangeList);
        bowToggle.onValueChanged.AddListener(ChangeList);

        exitButton.onClick.AddListener(Close);
    }

    private void initSetting()
    {
        perchaseToggle.isOn = false;
        swordToggle.isOn = true;
        wandToggle.isOn = true;
        bowToggle.isOn = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void ChangeList(bool buf)
    {
        foreach(CharacterStorySelectPanel list in characterList)
        {
            switch(list.charadata.weapon)
            {
                case CharacterInfoData.Weapon.Sword:
                    list.gameObject.SetActive(swordToggle.isOn);
                    break;
                case CharacterInfoData.Weapon.Wand:
                    list.gameObject.SetActive(wandToggle.isOn);
                    break;
                case CharacterInfoData.Weapon.Bow:
                    list.gameObject.SetActive(bowToggle.isOn);
                    break;
            }

            if (perchaseToggle.isOn)
            {
                switch (list.charadata.job)
                {
                    case PlayerJob.Priest:
                        if (userData.priest == false)
                            list.gameObject.SetActive(false);
                        break;
                    case PlayerJob.Wizard:
                        if (userData.wizard == false)
                            list.gameObject.SetActive(false);
                        break;
                }
            }
        }
    }
}

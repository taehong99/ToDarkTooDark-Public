using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPurchaseConfirmPanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] OutShopPanel outShopPanel;

    [Header("UI")]
    [SerializeField] Image itemImage;
    [SerializeField] Text itemText;
    [SerializeField] Text itemCost;
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    public CharacterItem curItem;
    [SerializeField] ConfirmPanel successPanel;
    [SerializeField] ConfirmPanel failPanel;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Start()
    {
        yesButton.onClick.AddListener(Perchase);
        noButton.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        itemImage.sprite = curItem.charadata.CharacterSprite;
        itemText.text = curItem.charadata.CharacterName;
        itemCost.text = $"x{curItem.charadata.cost} 엑스칼리버";
    }

    private void OnDisable()
    {
        curItem = null;
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void Perchase()
    {
        Userdata userData;
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

                if (userData.excaliver >= curItem.charadata.cost)
                {
                    switch (curItem.charadata.job)
                    {
                        case PlayerJob.Priest:
                            userData.priest = true;
                            break;
                        case PlayerJob.Wizard:
                            userData.wizard = true;
                            break;
                    }
                    userData.excaliver -= curItem.charadata.cost;
                    SaveChange(userID, userData);
                }
                else 
                {
                    failPanel.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
            );
    }

    public void SaveChange(string userId,Userdata userData)
    {
        string json = JsonUtility.ToJson(userData);
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
                successPanel.gameObject.SetActive(true);
                Changed();
                gameObject.SetActive(false);
            }
            );
    }

    private void Changed()
    {
        curItem.CheckPurchase();
        outShopPanel.WalletCheck();
    }

}

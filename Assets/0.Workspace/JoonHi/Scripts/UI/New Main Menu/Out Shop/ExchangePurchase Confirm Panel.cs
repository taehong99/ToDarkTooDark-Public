using Firebase.Database;
using Firebase.Extensions;
using ItemLootSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExchangePurchaseConfirmPanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] OutShopPanel outShopPanel;

    [Header("UI")]
    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;
    [SerializeField] Slider numSlider;
    [SerializeField] Text costText;
    [SerializeField] Text exNumText;
    [SerializeField] Text sliderNumText;
    public ExchangeItem curItem;
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

        numSlider.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnEnable()
    {
        initValue();
    }

    private void OnDisable()
    {
        curItem = null;
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void initValue()
    {
        numSlider.value = 1;
        OnValueChanged(1);
    }

    private void OnValueChanged(float num)
    {
        costText.text = ((int)num * curItem.exchangeData.cost).ToString();
        exNumText.text = ((int)num).ToString();
        sliderNumText.text = ((int) num).ToString();
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

                if (userData.token >= curItem.exchangeData.cost * (int)numSlider.value)
                {
                    userData.token -= curItem.exchangeData.cost * (int)numSlider.value;
                    userData.excaliver += (int) numSlider.value;
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

    public void SaveChange(string userId, Userdata userData)
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
        outShopPanel.WalletCheck();
    }
}

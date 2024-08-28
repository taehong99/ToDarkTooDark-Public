using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using JH;

public class OutShopPanel : MonoBehaviour
{
    [Header("Shop Showcase")]
    [SerializeField] GameObject characterShop;
    [SerializeField] GameObject exchangeShop;

    [Header("Buttons")]
    [SerializeField] Button characterShopButton;
    [SerializeField] Button exchangeShopButton;
    [SerializeField] Button exchangeShopButton2;
    [SerializeField] Button closeButton;

    [Header("Wallet")]
    [SerializeField] Text ExcaliverNum;
    [SerializeField] Text TokenNum;

    [Header("PopUp Panel")]
    // [SerializeField] 
    [SerializeField] ConfirmPanel buySuccessPanel;
    [SerializeField] ConfirmPanel buyFailPanel;

    private Userdata userData;

    private void Awake()
    {
        characterShopButton.onClick.AddListener(CharacterShop);
        exchangeShopButton.onClick.AddListener(ExchangeShop);
        exchangeShopButton2.onClick.AddListener(ExchangeShop);

        closeButton.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        CharacterShop();
        WalletCheck();
    }

    private void OnDisable()
    {
        userData = null;
    }

    private void CharacterShop()
    {
        characterShop.SetActive(true);
        exchangeShop.SetActive(false);
    }

    private void ExchangeShop()
    {
        characterShop.SetActive(false);
        exchangeShop.SetActive(true);
    }

    public void WalletCheck()
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

                userData = JsonUtility.FromJson<Userdata>(json);

                ExcaliverNum.text = userData.excaliver.ToString();
                TokenNum.text = userData.token.ToString();
            }
            );
    }

    private void Close()
    {
        CharacterShop();
        gameObject.SetActive(false);
        Manager.Sound.PlayBGM(Manager.Sound.SoundSO.LobbyBGM);
    }
}

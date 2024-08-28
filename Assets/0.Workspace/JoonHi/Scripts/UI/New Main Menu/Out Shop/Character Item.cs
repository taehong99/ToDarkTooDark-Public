using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterItem : MonoBehaviour
{
    [Header("Data")]
    public CharacterInfoData charadata;

    [Header("UI")]
    [SerializeField] Text charName;
    [SerializeField] Image charImage;
    [SerializeField] Text charCost;
    [SerializeField] Button selectButton;
    [SerializeField] GameObject purchasedPanel;

    [Header("PopUp Panel")]
    [SerializeField] CharacterPurchaseConfirmPanel purchasePanel;

    private void Awake()
    {
        charName.text = charadata.CharacterName;
        charImage.sprite = charadata.CharacterSprite;
        charCost.text = charadata.cost.ToString();
    }

    private void Start()
    {
        CheckPurchase();
        selectButton.onClick.AddListener(Confirmation);
    }

    private void Confirmation()
    {
        purchasePanel.curItem = this;
        purchasePanel.gameObject.SetActive(true);
    }

    public void CheckPurchase()
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
        switch (charadata.job)
        {
            case PlayerJob.Priest:
                if (userData.priest == false)
                {
                    selectButton.interactable = true;
                    purchasedPanel.SetActive(false);
                }
                else
                {
                    selectButton.interactable = false;
                    purchasedPanel.SetActive(true);
                }
                break;
            case PlayerJob.Wizard:
                if (userData.wizard == false)
                {
                    selectButton.interactable = true;
                    purchasedPanel.SetActive(false);
                }
                else
                {
                    selectButton.interactable = false;
                    purchasedPanel.SetActive(true);
                }
                break;
            default:
                selectButton.interactable = false;
                purchasedPanel.SetActive(true);
                break;
        }
    }
}

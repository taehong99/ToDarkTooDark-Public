using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeItem : MonoBehaviour
{
    [Header("Data")]
    public ExchangeData exchangeData;

    [Header("UI")]
    // [SerializeField] Text exchangeName;
    // [SerializeField] Image exchangeImage;
    [SerializeField] Text exchangeCost;
    [SerializeField] Button selectButton;

    [Header("PopUp Panel")]
    [SerializeField] ExchangePurchaseConfirmPanel exPurchasePanel;

    private void Awake()
    {
        // exchangeName.text = charadata.CharacterName;
        // exchangeImage.sprite = charadata.CharacterSprite;
        exchangeCost.text = exchangeData.cost.ToString();
    }

    private void Start()
    {
        selectButton.onClick.AddListener(Confirmation);
    }

    private void Confirmation()
    {
        exPurchasePanel.curItem = this;
        exPurchasePanel.gameObject.SetActive(true);
    }
}

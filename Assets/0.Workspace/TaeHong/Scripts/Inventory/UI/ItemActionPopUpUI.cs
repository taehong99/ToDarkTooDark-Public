using ItemLootSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tae.Inventory
{
    public class ItemActionPopUpUI : BaseUI
    {
        private BaseItemData itemToRegister;

        // Buttons
        private Button registerButton;
        private Button drinkButton;
        private Button key1Button;
        private Button key2Button;
        private Button key3Button;
        private Button key4Button;

        public void Init()
        {
            base.Awake();

            registerButton = GetUI<Button>("RegisterButton");
            drinkButton = GetUI<Button>("DrinkButton");
            key1Button = GetUI<Button>("Key1Button");
            key2Button = GetUI<Button>("Key2Button");
            key3Button = GetUI<Button>("Key3Button");
            key4Button = GetUI<Button>("Key4Button");

            registerButton.onClick.AddListener(OnRegister);
            drinkButton.onClick.AddListener(OnDrink);
            key1Button.onClick.AddListener(() => OnRegisterToKey(1));
            key2Button.onClick.AddListener(() => OnRegisterToKey(2));
            key3Button.onClick.AddListener(() => OnRegisterToKey(3));
            key4Button.onClick.AddListener(() => OnRegisterToKey(4));
        }

        public void RightClickItem(BaseItemData item)
        {
            itemToRegister = item;
            ShowActions(); // 퀵슬롯에 등록할지 아니면 버릴지
        }

        private void OnRegister()
        {
            ShowKeys(); // 등록버튼 누르면 어떤 키로 등록할지
        }

        private void OnRegisterToKey(int keyNum)
        {
            InventoryManager.Instance.quickslotManager.RegisterItem(keyNum, itemToRegister);
            gameObject.SetActive(false);
        }

        private void OnDrink()
        {
            InventoryManager.Instance.UseConsumable(itemToRegister);
            gameObject.SetActive(false);
        }

        private void ShowActions()
        {
            registerButton.gameObject.SetActive(true);
            drinkButton.gameObject.SetActive(true);
            key1Button.gameObject.SetActive(false);
            key2Button.gameObject.SetActive(false);
            key3Button.gameObject.SetActive(false);
            key4Button.gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private void ShowKeys()
        {
            registerButton.gameObject.SetActive(false);
            drinkButton.gameObject.SetActive(false);
            key1Button.gameObject.SetActive(true);
            key2Button.gameObject.SetActive(true);
            key3Button.gameObject.SetActive(true);
            key4Button.gameObject.SetActive(true);
        }
    }
}
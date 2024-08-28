using ItemLootSystem;
using System.Collections;
using System.Collections.Generic;
using Tae.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemPopUpUI : BaseUI
{
    public List<EquipItemData> equipmentInventory => InventoryManager.Instance.EquipmentInventory;
    List<Image> images= new();
    Image slot1;
    Image slot2;
    Image slot3;
    Image slot4;
    List<Button> buttons= new ();
    private Button slot1Button;
    private Button slot2Button;
    private Button slot3Button;
    private Button slot4Button;

    public void Init()
    {
        base.Awake();

        slot1Button = GetUI<Button>("Slot1Button");
        slot2Button = GetUI<Button>("Slot2Button");
        slot3Button = GetUI<Button>("Slot3Button");
        slot4Button = GetUI<Button>("Slot4Button");
        buttons.Add(slot1Button);
        buttons.Add(slot2Button);
        buttons.Add(slot3Button);
        buttons.Add(slot4Button);

        slot1 = GetUI<Image>("Slot1Button");
        slot2 = GetUI<Image>("Slot2Button");
        slot3 = GetUI<Image>("Slot3Button");
        slot4 = GetUI<Image>("Slot4Button");
        images.Add(slot1);
        images.Add(slot2);
        images.Add(slot3);
        images.Add(slot4);
    }
    private void OnEnable()
    {
        //equipmentInventory = InventoryManager.Instance.equipmentInventory;
    }

    void SetupAnvil()
    {
        slot1Button.onClick.AddListener(() => Enhance(0)) ;
        slot2Button.onClick.AddListener(() => Enhance(1));
        slot3Button.onClick.AddListener(() => Enhance(2));
        slot4Button.onClick.AddListener(() => Enhance(3));
    }

    void SetupRename()
    {
        slot1Button.onClick.AddListener(() => Reroll(0));
        slot2Button.onClick.AddListener(() => Reroll(1));
        slot3Button.onClick.AddListener(() => Reroll(2));
        slot4Button.onClick.AddListener(() => Reroll(3));
    }

    void Enhance(int i)
    {
        Debug.Log(equipmentInventory[i].enhanceLevel);
        int random = Random.Range(-2, 5); // -2 ~ 4
        Debug.Log(random);
        random = Mathf.Clamp(random, 0, 100);
        equipmentInventory[i].enhanceLevel += random;
        Debug.Log(equipmentInventory[i].enhanceLevel);
        gameObject.SetActive(false);
    }

    void Reroll(int i )
    {
        ItemDataManager.Instance.EquipAffixesTable.AddAffixesByDolDol(equipmentInventory[i]);
        gameObject.SetActive(false);
    }

    private void ShowActions()
    {
        gameObject.SetActive(true);

    }
    private void ShowImage()
    {
        Debug.Log("Show image");
        Debug.Log(equipmentInventory.Count);
        for (int i = 0; i < equipmentInventory.Count; i++)
        {
            if (equipmentInventory[i] == null)
                continue;

            Debug.Log($"item {i} has image");
            images[i].sprite = equipmentInventory[i].icon;
        }
    }

    public void RightClickItem(BaseItemData item)
    {
        ShowActions();
        ShowImage();
        if(item.ID == 156)
        {
            SetupAnvil();
        }
        else if(item.ID == 157)
        {
            SetupRename();
        }
        InventoryManager.Instance.UseConsumable(item);
    }


}

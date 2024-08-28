using ItemLootSystem;
using Pathfinding.Ionic.Zip;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Tae;
using Tae.Inventory;
public class DolDori : MonoBehaviourPun ,IInteractable
{
    [SerializeField] Collider2D interactionRange;
    [SerializeField] GameObject doldolUI;

    [SerializeField] int randomCost = 300;
    [SerializeField] int premeuimCost = 1000;
    int curCost;

    [Space(15), Header("UI Componets")]
    [SerializeField] Text randomLabel;
    [SerializeField] Text premeiumLabel;
    [SerializeField] Text costLabel;
    [SerializeField] Button reRollButton;

    [Space(10), Header("EquipItem Data UI Field")]
    [SerializeField] InventorySlot slot;
    [SerializeField] GameObject slotBlocker;
    [SerializeField] Image itemIcon;
    [SerializeField] Text firstAffixLabel;
    [SerializeField] Text secondAffixLabel;
    [SerializeField] Text singleAffixLabel;

    EquipItemData equipData;
    public EquipItemData EquipData { get{return equipData; } set { equipData = value; EquipDataUpdate(); } }

    bool mode; // 랜덤모드, 프리미엄 모드
    Color labelHighlight;
    Interactor curInteractor = null;

    const string highlightColor = "#94ECFF";

    private void OnEnable()
    {
       InitUI();
       slot.OnChangeItem += ItemSlotChanged;
    }

    void OnDisable()
    {
        slot.OnChangeItem -= ItemSlotChanged;
    }

    void InitUI()
    {
        ColorUtility.TryParseHtmlString(highlightColor, out labelHighlight);
        itemIcon.color = Color.clear;
        firstAffixLabel.text = string.Empty;
        secondAffixLabel.text = string.Empty;
        singleAffixLabel.text = "없음";
        CalculateCost();
    }

    void CalculateCost()
    {
        curCost = mode ? premeuimCost : randomCost;
        costLabel.text = $"{curCost}";
       
        //curInteractor.GetComponent<Inventory>;
        //InventoryManager.Instance.GoldCount; //  플레이어가 소지한 골드 참조 후 curCost와 비교해서  costLabel.text과 버튼 활성화 비활성화
        reRollButton.interactable = (InventoryManager.Instance?.GoldCount >= curCost) ? true: false; // 어짜피 인벤토리가 전역으로 설정되어있으니깐 Interactor 유무는 필요하지 않은듯
    }

    // UI 칸에 들어와있는 ItemData -> Reroll -> Master를 통해 Instantiate ,
    // Box 랑 다르게 Reroll한 ItemData를 Master에게 먼저 보내고 직렬화/역직렬화하는게 추가로 처리되어야함
    
    void RerollAffix(EquipItemData equip)
    {
        if (equipData == null)
            return;

        ItemDataManager.Instance.EquipAffixesTable.AddAffixesByDolDol(equip, mode);
        EquipDataUpdate();
    }

    void DropItem(EquipItemData itemData)
    {
        if (equipData == null)
            return;

        slot.ClearSlot();
        byte[]  data = EquipItemData.Serialize(itemData);

        //EquipItemData newitemData = (EquipItemData) ItemDataManager.Instance.GetItemData(data);
        //Debug.Log($"CurrentAffix: {itemData.affixes[0].name} \n Send Data: {newitemData.affixes[0].name}");
        photonView.RPC("NetworkItemDrop", RpcTarget.MasterClient,data);
        EquipData = null;
    }

    // 아이템 형식 다르면 뱉음
    void SpitOutItem(BaseItemData itemData)
    {
        // 임시
        Debug.Log("퉤-엣");
    }


    [PunRPC]
    public void NetworkItemDrop(byte[] data)
    {
        EquipItemData itemData = (EquipItemData)ItemDataManager.Instance.GetItemData(data);
        object[] objData = ItemDataManager.Instance.ConvertToObjectArray(itemData);
        DropedItem dropedItem = PhotonNetwork.InstantiateRoomObject
            ("DroptedItem", transform.position, Quaternion.identity, 0, objData).GetComponent<DropedItem>();
        dropedItem.Drop(3.0f, 1.75f);
    }

    // Trigger 범위내 아이템을 알아서 빨아먹음(1개만)
    Collider2D[] hitColliders = new Collider2D[10];

    [ContextMenu("ConsumItem")]
    public void ConsumItem()
    {
        if (equipData != null)
            return;

        //temp
        float radius = 3f;
        // collider overlap 수행
        Physics2D.OverlapCircleNonAlloc(transform.position, radius, hitColliders, LayerMask.GetMask("Item"));

        // item Layer인 오브젝트 하나 선택
        if (hitColliders[0] != null)
        {
            Collider2D selectedItem = hitColliders[0];
            Transform catchedTransform = selectedItem.transform;

            StartCoroutine(MoveToDolDol(catchedTransform));

            // itemData 추출, Fillter
            DropedItem dropedItem = catchedTransform.GetComponent<DropedItem>();
            Debug.Log($"consume : {dropedItem.name}");
            if (dropedItem != null)
            {
                BaseItemData itemData = dropedItem.ItemData;
                FillteredItem(itemData);
            }
        }
    }

    void FillteredItem(BaseItemData itemData)
    {
        if(itemData as EquipItemData)
        {
            // UI칸에 띄워 놓고 대기
            EquipData = (EquipItemData)itemData;
        }
        else
        {
            //뱉기
            SpitOutItem(itemData);
        }
    }

    IEnumerator MoveToDolDol(Transform itemTransform)
    {
        float duration = 1f;
        float elapsedTime = 0f;

        Vector3 startingPos = itemTransform.position;
        Vector3 targetPos = transform.position;

        while (elapsedTime < duration)
        {
            //(ease-in curve)
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
           
            t = t * t; // t^2 for acceleration

            itemTransform.position = Vector3.Lerp(startingPos, targetPos, t);

            yield return null;
        }
        itemTransform.position = targetPos;

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Destroy(itemTransform.gameObject);
        else
            Destroy(itemTransform.gameObject);
    }

    // 아이템 데이터 변경시 icon,text 변경
    public void EquipDataUpdate()
    {
        if(equipData == null)
        {
            InitUI();
            return;
        }

        itemIcon.sprite = equipData.icon;
        itemIcon.color = Color.white;

        List<EquipAffix> affixes = equipData.affixes;

        if(affixes.Count == 0 || affixes == null)
        {
            firstAffixLabel.text = string.Empty;
            secondAffixLabel.text = string.Empty;
            singleAffixLabel.color = Color.white;
            singleAffixLabel.text = "없음";
        }
        else if(affixes.Count == 1)
        {
            firstAffixLabel.text = string.Empty;
            secondAffixLabel.text = string.Empty;
            singleAffixLabel.text = affixes[0].GetSingleName();
            singleAffixLabel.color = affixes[0].GetTextColor();
        }
        else
        {
            firstAffixLabel.text = affixes[0].GetFirstName();
            firstAffixLabel.color = affixes[0].GetTextColor();
            secondAffixLabel.text = affixes[1].GetSingleName();
            secondAffixLabel.color = affixes[1].GetTextColor();
            singleAffixLabel.text = string.Empty;
        }
    }

    void ItemSlotChanged(BaseItemData baseItemData)
    {
        EquipData =(EquipItemData)baseItemData;
        slotBlocker.SetActive(equipData != null);
    }


    // UI관련 스크립트를 따로 둬야하나 (UI Controller)
    /**********************************************
    *                 UI Events 
    ***********************************************/

    // Simple Button Events  

    public void PremiumModeChange(bool isOn)
    {
        mode = isOn;
        premeiumLabel.color =  mode ? labelHighlight :Color.white ;
        CalculateCost();
    }

    public void RandomModeChange(bool isOn)
    {
        randomLabel.color = mode ? labelHighlight : Color.white;
        CalculateCost();
    }

    public void OnCloseButtonClick()
    {
        doldolUI.gameObject.SetActive(false);
        DropItem(equipData);
        curInteractor = null;
    }

    [ContextMenu("Reroll")]
    public void OnRerollButtonClick()
    {
        RerollAffix(equipData);
        InventoryManager.Instance.SpendGold(curCost);
        CalculateCost();
    }

    public void ShowUI()
    {
        //doldolUI.gameObject.SetActive(true);
       // Debug.Log("DolDol ShowUI");
    }

    public void OnInteract(Interactor interactor)
    {
        doldolUI.gameObject.SetActive(!doldolUI.gameObject.activeSelf);
        CalculateCost();
        curInteractor = interactor;
    }

    public void HideUI()
    {
        //Debug.Log("DolDol HideUI");
        if(doldolUI.gameObject.activeSelf) //거리 벗어났을 때, 돌돌이 팝업 활성화 되어있음 비활성화
            OnInteract(null);
    }
}

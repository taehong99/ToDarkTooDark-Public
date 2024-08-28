using ItemLootSystem;
using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using Tae.Inventory;
using UnityEngine;

[AddComponentMenu("Tutorials/Items")]
public class ItemTutorial : TutorialBase
{
    [SerializeField] DialogueSequence firstSequence; // 인벤토리 오픈 시퀀스
    [SerializeField] ToastPopUpUI firstToast;

    [SerializeField] DialogueSequence secondSequence; // 장비 장착 시퀀스
    [SerializeField] ToastPopUpUI secondToast;

    [SerializeField] DialogueSequence thirdSequence; // 퀵슬롯 등록 시퀀스
    [SerializeField] float waitTime;

    [SerializeField] DialogueSequence fourthSequence;  // 포션 사용 시퀀스 

    [SerializeField] DialogueSequence fifthSequence; // 다음 튜토리얼 안내 시퀀스

    [Space(10)]
    [SerializeField] BaseItemData tutorialItem; // 튜토리얼용 소비템
    [SerializeField] BaseItemData equipItem; // 튜토리얼용 장비 템

    InventoryManager inventory;
    PlayerStatsManager stats;

    public override void Enter()
    {
        InventoryManager.Instance.gameObject.SetActive(true);
        inventory = InventoryManager.Instance;
        inventory.enabled = true;
        stats = TutorialManager.Instance.Player.GetComponentInParent<PlayerStatsManager>();
        InventoryManager.Instance.ObtainItem(equipItem);


        DialogueManager.Instance.SetDialogueSequence(firstSequence);
        DialogueManager.Instance.OnSequnceFinsh += AfterFirstSequence;
    }

    void AfterFirstSequence()
    {
        firstToast = Instantiate(firstToast, TutorialManager.Instance.PopUpCanvas.transform);
        checkInvenOpen = true;
    }

    void AfterInvenOpen()
    {
        DialogueManager.Instance.SetDialogueSequence(secondSequence);
        DialogueManager.Instance.OnSequnceFinsh += AfterSecondSequence;
    }

    void AfterSecondSequence()
    {
        secondToast = Instantiate(secondToast, TutorialManager.Instance.PopUpCanvas.transform);
        stats.StatChangedEvent += AfterEquipItem;
    }

    // 장비 장착 이후, 소비 슬롯에 대한 대화 설명
    void AfterEquipItem(StatType stat , float val)
    {
        stats.StatChangedEvent -= AfterEquipItem;
        InventoryManager.Instance.ObtainItem(tutorialItem);
        InventoryManager.Instance.ObtainItem(tutorialItem);

        DialogueManager.Instance.SetDialogueSequence(thirdSequence);
        DialogueManager.Instance.OnSequnceFinsh += AfterThirdSequence;
    }

    // 물약 소비 슬롯에 올릴 때 까지 기달리.. 지 않고 그냥 일정 시간만 wait
    void AfterThirdSequence()
    {
        StartCoroutine(WaitforSlotSet());
    }

  
    IEnumerator WaitforSlotSet()
    {
        yield return YieldCache.WaitForSeconds(waitTime);

        DialogueManager.Instance.SetDialogueSequence(fourthSequence);  // 이후 다음 대화 진행, 대화진행 끝나면 포션 사용 wait 이벤트 연결
        DialogueManager.Instance.OnSequnceFinsh += AfterFourthSequence;
    }

    void AfterFourthSequence()
    {
        // 물약 사용까지 기달림
        stats.levelChangedEvent += AfterUsePotion;
    }


    void AfterUsePotion(int level)
    {
        DialogueManager.Instance.SetDialogueSequence(fifthSequence);
        DialogueManager.Instance.OnSequnceFinsh += Exit;
    }

    bool checkInvenOpen;
    public override void Excute(TutorialManager tutorialManager)
    {

        if(checkInvenOpen && Input.GetKeyDown(KeyCode.B))
        {
            checkInvenOpen = false;
            AfterInvenOpen();
        }


        if (isCompleted)
        {
            tutorialManager.NextTutorial();
        }
    }

    public override void Exit()
    {
        isCompleted = true;
    }
}

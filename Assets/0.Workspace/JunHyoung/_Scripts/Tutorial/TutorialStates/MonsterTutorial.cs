using ItemLootSystem;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;

[AddComponentMenu("Tutorials/Monster")]
public class MonsterTutorial : TutorialBase
{
    [SerializeField] MonsterSpawner spawner;
    [SerializeField] SkillsUI skillUi;
    [SerializeField] List<SkillSlot> slots;


    [SerializeField] ToastPopUpUI firstToastPopUp;

    [Header("Item Setting")]
    [SerializeField] DropedItem dropedItem;
    [SerializeField] EquipItemData equipData;
    [SerializeField] Transform dropPos;

    [Space(10),Header("Dialogue")]
    [SerializeField] DialogueSequence firstSequence;
    [SerializeField] PopUpUI firstPopUP;

    [SerializeField] DialogueSequence secondSequence;
    [SerializeField] ToastPopUpUI secondToastPopUp;

    // 몬스터 4체 스폰

    // 미니 팝업

    // .. 몬스터 모두 처치시 미니팝업 삭제, 전용무기 드랍

    // 유저 활동 비활성화, NPC 유저 옆으로 스폰 , 대화시퀀스 진행

    // 대화 종료시 팝업

    // 팝업 종료시 대화 시퀀스

    // 대화 종료시 미니팝업 (1f) 보여주고 닫음

    Interactor interactor;

    public override void Enter()
    {
        spawner.SpawnMonsters();
        if(TutorialManager.Instance.Player.TryGetComponent<Interactor>(out interactor))
        {
            interactor.gameObject.SetActive(false); // 몬스터 생성시 잠시 상호작용 비 활성화해서 문 못열게함
        }
        spawner.allMonsterDied += AfterKillMonsters;
        firstToastPopUp = Instantiate(firstToastPopUp, TutorialManager.Instance.PopUpCanvas.transform);
    }

    void AfterKillMonsters()
    {
        firstToastPopUp.FadeOut();
        spawner.allMonsterDied -= AfterKillMonsters;
        interactor?.gameObject.SetActive(false);

        DropTutorialItem();

        TutorialManager.Instance.Player.enabled = false;
        DialogueManager.Instance.SetDialogueSequence(firstSequence);
        DialogueManager.Instance.OnSequnceFinsh +=AfterFirstSqeunce;
    }

    [ContextMenu("drop test")]
    void DropTutorialItem()
    {
        // 전용무기 드랍
        EquipItemData equipItemData = Instantiate(equipData);
        ItemDataManager.Instance.EquipAffixesTable.AddAffixesByDolDol(equipItemData, true);

        Debug.Log(equipItemData.GetItemNameWithSpaces());

        DropedItem drop = Instantiate(dropedItem, dropPos.position, Quaternion.identity);
        drop.ItemData = equipItemData;
        drop.Drop(3f, 1.75f);
    }

    void AfterFirstSqeunce()
    {
        Manager.UI.OnClose += AfterPopUpClose;
        Manager.UI.ShowPopUpUI(firstPopUP);
    }

    void AfterPopUpClose()
    {
        DialogueManager.Instance.SetDialogueSequence(secondSequence);
        DialogueManager.Instance.OnSequnceFinsh += AfterSecondSquence;
    }

    void AfterSecondSquence()
    {
        Instantiate(secondToastPopUp, TutorialManager.Instance.PopUpCanvas.transform); // auto FadeOut인지 확인할것
        TutorialManager.Instance.Player.enabled = true;
        skillUi.Init();
        TutorialManager.Instance.Player.GetComponent<PlayerStatsManager>().ClientGainExp(200);


        foreach(var slot in slots)
        {
            slot.OnSkilllevelUp += Exit;
        }

       // Manager.Event.DoorInteractionEvent += Exit; // 스킬 찍은거 콜백 대신에, 다음  문 열었을때로 이벤트 타이밍 변경.
        //StartCoroutine(WaitRoutine());
    }


    IEnumerator WaitRoutine()
    {
        yield return YieldCache.WaitForSecondsRealTime(3.0f);
        Exit();
    }

    void Exit(int temp)
    {
        foreach (var slot in slots)
        {
            slot.OnSkilllevelUp -= Exit;
        }

        Manager.Event.DoorInteractionEvent -= Exit;
        Exit();
    }


    public override void Excute(TutorialManager tutorialManager)
    {
        if(isCompleted)
            tutorialManager.NextTutorial();
    }

    public override void Exit()
    {
        isCompleted = true;
    }
}

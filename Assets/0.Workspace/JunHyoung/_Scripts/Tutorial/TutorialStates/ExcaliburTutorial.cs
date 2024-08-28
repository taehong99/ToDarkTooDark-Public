using DG.Tweening;
using MapGenerator;
using System.Collections;
using System.Collections.Generic;
using Tae;
using Tae.Inventory;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[AddComponentMenu("Tutorials/Excalibur")]
public class ExcaliburTutorial : TutorialBase
{
    [SerializeField] DialogueSequence firstSequence;
    [SerializeField] PopUpUI firstPopUp;

    [Space(10)]
    [SerializeField] DialogueSequence secondSequence;
    [SerializeField] PopUpUI secondPopUp;

    [Space(10)]
    [SerializeField] DialogueSequence thirdSequence;

    [Space(10)]
    [SerializeField] List<MonsterSpawner> spawners; // 엑스칼리버 획득이후 스폰시킬 몬스터들
    [SerializeField] Exit exitPoint; // 

    [SerializeField] GameObject NPC;

    public override void Enter()
    {
        NPC.gameObject.SetActive(true);
        Manager.Game.MyExit = exitPoint;
        StartCoroutine(TutorialManager.Instance.Cam.FollowTargetRoutine(transform, 2.0f));
        // 카메라 연출 이후 첫번째 트리거 위치까지 이동하기를 기다림
    }

    bool isTriggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;

        if(collision.CompareTag("Player"))
            AfterWaitFirstTrigger(true);
    }

    void AfterWaitFirstTrigger(bool val)
    {
        //첫번째 대화 시퀀스 진행
        isTriggered = true;
        DialogueManager.Instance.SetDialogueSequence(firstSequence);
        DialogueManager.Instance.OnSequnceFinsh += AfterFirstSequence;
    }

        //이후 팝업
    void AfterFirstSequence()
    {
        Manager.UI.OnClose += AfterFistPopUp;
        Manager.UI.ShowPopUpUI(firstPopUp);
    }

    // 팝업 닫히고 나면 다음 대화 진행
    void AfterFistPopUp()
    {
        DialogueManager.Instance.SetDialogueSequence(secondSequence);
        DialogueManager.Instance.OnSequnceFinsh += AfterSecoundSequence;
    }

    void AfterSecoundSequence()
    {
        NPC.gameObject.transform.DOLocalMove(NPC.gameObject.transform.localPosition + new Vector3(1, 1, 0), 0.75f);

        // 엑스칼리버 획득 이벤트 +=  StartCoroutine(AfterDrawExcaulibur());
        Manager.Event.ExcaliburPickUpEvent +=ExcaliburPickUpEvent;
    }

    void ExcaliburPickUpEvent(int val)
    {
        Manager.Event.ExcaliburPickUpEvent -= ExcaliburPickUpEvent;
        exitPoint.gameObject.SetActive(true);
        StartCoroutine(AfterDrawExcaulibur());
    }


    IEnumerator AfterDrawExcaulibur()
    {
        TutorialManager.Instance.Player.enabled = false;

        // 스포너 활성화

        StartCoroutine(TutorialManager.Instance.Cam.ActiveLetterBox(4.0f));

        for(int i=0; i<spawners.Count; i++)
        {
            TutorialManager.Instance.Cam.FollowTarget(spawners[i].transform);
            yield return YieldCache.WaitForSeconds(1.0f);
            Light2D light = spawners[i].GetComponentInChildren<Light2D>(); // 시간이 없으니...
            if(light != null)
            { 
                float targetIntensity = 1.5f; 
                float duration = 1.0f; 
                DOTween.To(() => light.intensity, x => light.intensity = x, targetIntensity, duration).SetEase(Ease.InOutBounce);
            }
           
            spawners[i].SpawnMonsters();
            yield return YieldCache.WaitForSeconds(1.0f);
        }

        // 출구 활성화
        exitPoint.gameObject.SetActive(true);
        TutorialManager.Instance.Cam.FollowTarget(exitPoint.transform);
        yield return YieldCache.WaitForSeconds(1.0f);


        TutorialManager.Instance.Cam.FollowTarget(TutorialManager.Instance.Player.transform);
        TutorialManager.Instance.Player.enabled = true;
        Exit();
    }

    public override void Excute(TutorialManager tutorialManager)
    {
        if(isCompleted)
        {
            tutorialManager.NextTutorial();
        }
    }

    public override void Exit()
    {
       isCompleted = true;
    }

}

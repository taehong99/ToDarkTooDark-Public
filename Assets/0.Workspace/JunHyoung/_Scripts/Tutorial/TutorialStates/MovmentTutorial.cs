using DG.Tweening;
using System.Collections;
using UnityEngine;
/// <summary>
///  3번째 튜토리얼. 조작 설명을 담당
///    처음에는 공격 기능
///    두번째는 스킬 기능
///    이름을 왜 무브먼트로 했을까... 하...
/// </summary>


[AddComponentMenu("Tutorials/Movement")]
public class Movmenttutorial : TutorialBase
{
    [SerializeField] DialogueSequence firstSequence;
    [SerializeField] PopUpUI firstPopUpUI;

    [SerializeField] PopUpUI secondPopUpUI;
    [SerializeField] DialogueSequence secondSequence;

    [SerializeField] ToastPopUpUI skillEToastPopUp;
    [SerializeField] ToastPopUpUI skillSpaceToastPopUp;
    [SerializeField] ToastPopUpUI skillQToastPopUp;


    [Space(10), Header("Attack tutorial")]
    [SerializeField] ToastPopUpUI attackToastPopUp;
    [SerializeField] BreakableObject box;
    [SerializeField] Transform boxSpawnPosistion;
    [SerializeField] Transform npc;

    [Space(10), Header("Skill tutorial")]
    [SerializeField] Transform dummy;
    [SerializeField] GameObject quickSlotUI;

    [Space(10)]
    [SerializeField] Transform nextNPCPos;

    public override void Enter()
    {
        // 유저 행동 해제, 기본 공격기능 해제
        TutorialManager.Instance.Player.enabled = true;
        TutorialManager.Instance.Player.GetComponent<PlayerStates>().IsAttack = true;
        SpawnBox();
    }

    void SpawnBox()
    {
        // 토스트 팝업 스폰, 상자에 공격시 팝업창 비활성화
        box = Instantiate(box, boxSpawnPosistion.position, Quaternion.identity);
        box.transform.PingPongTransform(this);
        attackToastPopUp = Instantiate(attackToastPopUp, TutorialManager.Instance.PopUpCanvas.transform);
        box.diedEvent += AfterBrokenBox;
    }

    void AfterBrokenBox()
    {
        attackToastPopUp.FadeOut(); // 토스트 팝업 삭제

        // 이후 2초후 첫번째 대화 시퀀스 진행, 클로즈업
        npc.gameObject.SetActive(true);
        TutorialManager.Instance.Cam.FollowTarget(npc);
        TutorialManager.Instance.Cam.ZoomIn(1.0f, 1.0f);

        DialogueManager.Instance.SetDialogueSequence(firstSequence);
        DialogueManager.Instance.OnSequnceFinsh += AfterFirstSequence;
    }

    void AfterFirstSequence()
    {
        // 대화 이후 원래 시야로 줌아웃. 스킬사용 기능해제
        TutorialManager.Instance.Cam.RollBackTarget();
        TutorialManager.Instance.Cam.RollBackZoom(1.0f);

        //TutorialManager.Instance.Player.GetComponent<PlayerStatsManager>().CurEXP += 400;
        Debug.Log("Player 스킬 사용 가능");

        // 유저 행동 비활성화, 카메라 시점 더미 몬스터에 2초 고정 & 클로즈업
        TutorialManager.Instance.Player.enabled = false;
        TutorialManager.Instance.Cam.ZoomIn(1.0f, 1.0f);
        StartCoroutine(DummyFocusCameraRoutine());

        //StartCoroutine(TutorialManager.Instance.Cam.FollowTargetRoutine(dummy, 2));
    }

    IEnumerator DummyFocusCameraRoutine()
    {
        //  유저에게 2초간 카메라 시점 원상복귀, 루틴 1초 이후 팝업창 활성화
        TutorialManager.Instance.Cam.FollowTarget(dummy);
        yield return YieldCache.WaitForSeconds(2);
        TutorialManager.Instance.Cam.RollBackTarget();
        OpenFirstPopUP();
    }

    void OpenFirstPopUP()
    {
        Manager.UI.OnClose += OpenSecoundPopUp;
        Manager.UI.ShowPopUpUI(firstPopUpUI); // '스킬' 과 '스킬유형' 설명 팝업
    }

    void OpenSecoundPopUp()
    {
        Manager.UI.ShowPopUpUI(secondPopUpUI);
        //Manager.UI.OnClose += AfterPopUpClose;
        //여기 이유는 모르겠는데 콜백이 늦게 들어와서 다음꺼 호출이 안됨
        StartCoroutine(FixedWait()); // 땜빵...
    }

    IEnumerator FixedWait()
    {
        yield return YieldCache.WaitForSecondsRealTime(2.0f);
        AfterPopUpClose();
    }

    void AfterPopUpClose()
    {
        TutorialManager.Instance.Player.enabled = true;

        // Dummy로 부터 콜백 받아서 맞은 스킬 유형에 따라 다음 팝업 활성/비활성화
        TutorialManager.Instance.Player.GetComponent<PlayerStatsManager>().DebugSkiilUp();
        quickSlotUI.SetActive(true);
        //quickSlotUI.GetComponentInChildren<SkillsUI>().Init();
        //skillQToastPopUp
        //skillEToastPopUp
        //skillRToastPopUp
        waitEFlag = true;
        skillEToastPopUp = Instantiate(skillEToastPopUp, TutorialManager.Instance.PopUpCanvas.transform);
        //Debug.Log("Dummy로 부터 콜백 받아서 맞은 스킬 유형에 따라 다음 팝업 활성/비활성화 구현할것"); <- Excute에서 flag 체크로 변경함
        // 더미 맞은 여부는 상관x

    }

    void StartSecoundSequence()
    {
        DialogueManager.Instance.SetDialogueSequence(secondSequence);
        DialogueManager.Instance.OnSequnceFinsh += AfterSecoundSequence;
    }

    void AfterSecoundSequence()
    {
        Exit();
    }


    bool waitQFlag = false;
    bool waitEFlag = false;
    bool waitSpaceFlag = false;


    // 최악이다. 최악 이래서 HFSM 쓰는구나
    public override void Excute(TutorialManager tutorialManager)
    {
        if(waitEFlag && Input.GetKeyUp(KeyCode.E))
        {
            waitSpaceFlag = true;
            waitEFlag = false;

            skillEToastPopUp.FadeOut();
            skillSpaceToastPopUp = Instantiate(skillSpaceToastPopUp, TutorialManager.Instance.PopUpCanvas.transform);
        }

        if (waitSpaceFlag && Input.GetKeyUp(KeyCode.Space))
        {
            waitQFlag = true;
            waitSpaceFlag = false;

            skillSpaceToastPopUp.FadeOut();
            skillQToastPopUp = Instantiate(skillQToastPopUp, TutorialManager.Instance.PopUpCanvas.transform);
        }

        if (waitQFlag && Input.GetKeyUp(KeyCode.Q))
        {
            waitQFlag = false;

            skillQToastPopUp.FadeOut();
            DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0f, 1.75f)
            .SetEase(Ease.InExpo)
            .SetUpdate(true) 
            .OnComplete(StartSecoundSequence);
           // StartSecoundSequence();
        }

        if (isCompleted)
        {
            tutorialManager.NextTutorial();
            return;
        }
    }




    public override void Exit()
    {
        isCompleted = true;

        //StartCoroutine(TutorialManager.Instance.Cam.FollowTargetRoutine(nextNPCPos, 2));
        npc.gameObject.SetActive(false);
        TutorialManager.Instance.Player.enabled = true;
        // 다음 튜토리얼은 시스템 및 보상 (Reward Tutorial)
    }


}

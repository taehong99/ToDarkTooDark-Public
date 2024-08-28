using DG.Tweening;
using System.Collections;
using UnityEngine;

/// <summary>
/// 첫 번째 튜토리얼
///     게임 배경설정 설명
///     플레이어 이동 조작
/// </summary>

[AddComponentMenu("Tutorials/TutorialStart")]
public class TutorialStart : TutorialBase
{
    [SerializeField] DialogueSequence sequence;

    [Space(10), Header("Game Objects")]
    [SerializeField] PopUpUI popUp;


    [Space(10), Header("Transforms")]
    [SerializeField] Transform PlayerSpawnPoint;
    [SerializeField] Transform NPCSpawnPoint;
    [SerializeField] Transform NPCMovePoint;

    [Space(10), Header("ETC Settings")]
    [SerializeField] float waitUntilDialogue = 5.0f;
    [SerializeField] float waitForPlayerMove = 1.0f;


    public override void Enter()
    {
        // 페이드 인 효과 (0~100, 3초)

        // 유저 캐릭터 레벨2, 모든 스킬렙 1로 초기화
        Debug.Log("Init Level");
        // 스탯매니저 참조

        // 기본 무기, 기본 갑옷 장착 상태
        Debug.Log("Init Equipment");
        // 인벤토리 매니저 참조

        TutorialManager.Instance.Player.GetComponent<PlayerStates>().IsAttack = false;

        // 카메라 포커스 to NPC
        NPCSpawnPoint.gameObject.SetActive(true);
        TutorialManager.Instance.Cam.FollowTarget(NPCSpawnPoint.transform);
        TutorialManager.Instance.Cam.ZoomIn(1.0f, 2.0f);

        // N초뒤 대화창 활성화
        StartCoroutine(ActiveDialogue());
    }

    public override void Excute(TutorialManager tutorialManager)
    {
        // nothing? 

        if (isCompleted)
        {
            tutorialManager.NextTutorial();
        }
    }

    public override void Exit()
    {
        // 이 다음은 GameRuleTutorial
        isCompleted = true;
    }


    // 대화창 활성화
    IEnumerator ActiveDialogue()
    {
        yield return YieldCache.WaitForSecondsRealTime(waitUntilDialogue);

        DialogueManager.Instance.OnSequnceFinsh += PopUp;
        DialogueManager.Instance.SetDialogueSequence(Instantiate(sequence));
    }

    // 대화창 종료 이후



    // 팝업창 활성화
    void PopUp()
    {
        TutorialManager.Instance.Cam.RollBackZoom(1.0f);
        Manager.UI.OnClose += PopUpClose;
        Manager.UI.ShowPopUpUI(popUp);
    }


    // 팝업창 활성화 이후
    public void PopUpClose()
    {
        StartCoroutine(NextPopUpRoutine());
    }


    IEnumerator NextPopUpRoutine()
    {
        // NPC 9의 속도로 지정된 위치로 이동, X 안하도록 수정
        //npc.transform.DOMove(NPCMovePoint.position, 2.0f);
        NPCSpawnPoint.gameObject.SetActive(false);
        NPCMovePoint.gameObject.SetActive(true);
        TutorialManager.Instance.Cam.FollowTarget(NPCMovePoint.transform);
        // 2초간 카메라 따라가다가 
        yield return YieldCache.WaitForSecondsRealTime(2.0f);


        //카메라 유저 시점으로 전환
        TutorialManager.Instance.Cam.FollowTarget(TutorialManager.Instance.Player.transform);
        yield return YieldCache.WaitForSecondsRealTime(waitForPlayerMove);

        // 1초 딜레이 이후 캐릭터 이동 가능
        TutorialManager.Instance.Player.enabled = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Exit();
    }
}

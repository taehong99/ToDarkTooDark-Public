using ItemLootSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Tutorials/Rewards")]
public class RewardTutorial : TutorialBase
{
    [SerializeField] PopUpUI firstPopUP; //Introduce Puzzle
    [SerializeField] DialogueSequence fisrtSequence;

    [SerializeField] ToastPopUpUI firstToast;
    [SerializeField] Transform nextNPCPos;


    [SerializeField] ToastPopUpUI secoundToast;
    [SerializeField] BaseBox tutorialBox;
    [SerializeField] Transform boxSpawnPos;

    public override void Enter()
    {
        Manager.UI.OnClose += () => { DialogueManager.Instance.OnSequnceFinsh += AfterFirstSequence; DialogueManager.Instance.SetDialogueSequence(fisrtSequence); };
        Manager.UI.ShowPopUpUI(firstPopUP);
    }

    void AfterFirstSequence()
    {
        nextNPCPos.gameObject.SetActive(true);
         StartCoroutine(TutorialManager.Instance.Cam.FollowTargetRoutine(nextNPCPos, 1f));

        firstToast = Instantiate(firstToast, TutorialManager.Instance.PopUpCanvas.transform);
        // 퍼즐 기믹 해제시 이벤트에 AfterPuzzleSolve 추가
    }

    //퍼즐기믹 종료 후 보물상자 스폰
    public IEnumerator AfterPuzzleSolve()
    {
        TutorialManager.Instance.Player.enabled = false;

        secoundToast = Instantiate(secoundToast, TutorialManager.Instance.PopUpCanvas.transform);
        tutorialBox = Instantiate(tutorialBox, boxSpawnPos.position, Quaternion.identity);
        StartCoroutine(TutorialManager.Instance.Cam.FollowTargetRoutine(boxSpawnPos, 1f));
        tutorialBox.transform.JellyPingPongTransform(this);
        tutorialBox.OnBoxOpen += AfterBoxOpen;


        yield return YieldCache.WaitForSeconds(1.0f);  

        TutorialManager.Instance.Player.enabled = true;
    }

    void AfterBoxOpen()
    {
        secoundToast.FadeOut();
        tutorialBox.OnBoxOpen -= AfterBoxOpen;

        Exit();
    }

    public override void Excute(TutorialManager tutorialManager)
    {
        if(isCompleted)
            tutorialManager.NextTutorial(); //다음 튜토리얼은 Moster(전투)
    }

    public override void Exit()
    {
        nextNPCPos.gameObject.SetActive(false);
        isCompleted = true;
    }

}

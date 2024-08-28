using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 두 번째 튜토리얼
///     대화 시퀀스와 팝업 이벤트로만 구성됨.
///     게임의 규칙 설명 파트
/// </summary>

[AddComponentMenu("Tutorials/GameRule")]
public class GameRuleTutorial : TutorialBase
{
    [SerializeField] List<DialogueSequence> sequences; // 대화 시퀀스 4개, 팝업 3개
    [SerializeField] List<PopUpUI> popUpUis;

    int curSequenceIndex = -1;
    int curPopUpIndex = -1;


    public override void Enter()
    {
        // 유저활동 비 활성화
        TutorialManager.Instance.Player.enabled = false;

        //팝업
        //DialogueManager.Instance.OnSequnceFinsh += NextPopUP;
        //Manager.UI.OnClose += NextDialogueSeqeunce;
        NextDialogueSeqeunce();
    }

    public override void Excute(TutorialManager tutorialManager)
    {
        if (isCompleted)
        {
            tutorialManager.NextTutorial();
        }
    }

    public override void Exit()
    {
        // 유저 조작 활성화
        isCompleted = true;
        TutorialManager.Instance.Player.enabled = true;
        // 다음 튜토리얼은 Movement
    }


    public void NextDialogueSeqeunce()
    {
        curSequenceIndex++;

        if (curSequenceIndex < sequences.Count)
        {
            DialogueManager.Instance.OnSequnceFinsh += NextPopUP;
            DialogueManager.Instance.SetDialogueSequence(sequences[curSequenceIndex]);
        }
        else
        {
            Exit();
        }
    }

    void NextPopUP()
    {
        curPopUpIndex++;

        if (curPopUpIndex < popUpUis.Count)
        {
            Manager.UI.OnClose += NextDialogueSeqeunce;
            Manager.UI.ShowPopUpUI(popUpUis[curPopUpIndex]);
        }
        else
        {
            Exit();
        }
    }
}

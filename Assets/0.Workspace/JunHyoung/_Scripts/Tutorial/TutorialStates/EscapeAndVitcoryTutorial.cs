using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;

[AddComponentMenu("Tutorials/Escape")]
public class EscapeAndVitcoryTutorial : TutorialBase
{
    [SerializeField] DialogueSequence sequence;
    [SerializeField] Exit exit;

    public override void Enter()
    {
        exit.OnExitInteract += StartSequence;
    }

    void StartSequence()
    {
        exit.OnExitInteract -= StartSequence;

        DialogueManager.Instance.SetDialogueSequence(sequence);
        DialogueManager.Instance.DisableNPCCanvas();
        DialogueManager.Instance.OnSequnceFinsh += Exit;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialBase : MonoBehaviour
{
    public abstract void Enter();

    public abstract void Excute(TutorialManager tutorialManager);

    public abstract void Exit();

    protected bool isCompleted;
}

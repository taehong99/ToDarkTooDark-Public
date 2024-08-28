using JH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPuzzleRoom2 : TutorialPuzzleRoom
{
    [SerializeField] RewardTutorial rewardTutorial;

    protected override void PlatePressCheck()
    {
        if (activatePlateCnt >= pressurePlates.Count)
            StartCoroutine(rewardTutorial.AfterPuzzleSolve());
    }
}

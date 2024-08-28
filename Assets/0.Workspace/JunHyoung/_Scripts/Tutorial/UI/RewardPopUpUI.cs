using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardPopUpUi : TweeningPopUpUI
{
    [SerializeField] string mainScene;

    private void OnDisable()
    {
        Manager.Scene.LoadScene(mainScene);
    }
}

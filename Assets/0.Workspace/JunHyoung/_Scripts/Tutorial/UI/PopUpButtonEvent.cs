using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpButtonEvent : MonoBehaviour
{
    [SerializeField] string sceneName = "";

    public void ChangeScene()
    {
        //Manager.Scene.LoadScene(sceneName);

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        Manager.UI.ClosePopUpUI();
    }

    public void ClosePopUp()
    {
        Manager.UI.ClosePopUpUI();
    }

}

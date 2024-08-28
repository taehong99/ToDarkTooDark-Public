using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PVButton : MonoBehaviour
{
    [SerializeField] PopUpUI popup;
    [SerializeField] PopUpUI creditPopup;
    [SerializeField] Button button;


    public void OpenPVPopUp()
    {
        Manager.UI.ShowPopUpUI(popup);
    }

    public void OpenCreditPopUp()
    {
        Manager.UI.ShowPopUpUI(creditPopup);
    }
}

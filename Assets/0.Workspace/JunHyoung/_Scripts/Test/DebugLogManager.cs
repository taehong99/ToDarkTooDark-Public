using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogManager : MonoBehaviour
{
    public ToastPopUpUI popupPrefab;
    public Transform popupParent;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        ShowPopup(logString, stackTrace, type);
    }

    void ShowPopup(string logString, string stackTrace, LogType type)
    {
        ToastPopUpUI popupInstance = Instantiate(popupPrefab, popupParent);

        switch (type)
        {
            case LogType.Error:
               popupInstance.text.color = Color.red;
                break;
            case LogType.Warning:
                popupInstance.text.color = Color.yellow;
                break;
            case LogType.Exception:
                 popupInstance.text.color = Color.red;
                break;
            default:
               popupInstance.text.color = Color.white; 
                break;
        }

        popupInstance.text.text = $"[{type} : {logString}]";
        popupInstance.subText.text = stackTrace;
    }
}

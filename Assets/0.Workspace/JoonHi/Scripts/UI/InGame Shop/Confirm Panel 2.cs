using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel2 : MonoBehaviour
{
    [Header("Buttons")]
    public Button confirmButton;
    public Button cancelButton;

    void Start()
    {
        cancelButton.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        confirmButton.onClick.AddListener(Close);
    }

    private void OnDisable()
    {
        confirmButton.onClick.RemoveAllListeners();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}

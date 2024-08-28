using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TweeningPopUpUI : PopUpUI
{
    [SerializeField] Button closeButton;

    [Range(0f, 3f)]
    [SerializeField] float popTime = 0.75f;

    [Header("Tweening Ease Setting")]
    [SerializeField] Ease popUpEase = Ease.InOutBack;
    [SerializeField] Ease closeEase = Ease.InBack;

    protected override void Awake()
    {
        //base.Awake(); Skip Binding UI Items
        StartCoroutine(PopUpRoutine());
        closeButton?.onClick.AddListener(() => StartCoroutine(ClosePopUpRoutine()));
    }


    IEnumerator PopUpRoutine()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, popTime).SetEase(popUpEase).SetUpdate(true);
        yield return YieldCache.WaitForSecondsRealTime(popTime);
    }


    IEnumerator ClosePopUpRoutine()
    {
        transform.DOScale(Vector3.zero, popTime).SetEase(closeEase).SetUpdate(true); // Animate to zero size 
        yield return YieldCache.WaitForSecondsRealTime(popTime);                     // .OnComplete(Close) 했을때 삭제된 객체 참조 오류 나서 아래와 같이 변경
        Close();
    }
}

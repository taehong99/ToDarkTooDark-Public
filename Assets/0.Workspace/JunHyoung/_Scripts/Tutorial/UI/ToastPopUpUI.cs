using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastPopUpUI : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI text;
    [SerializeField] public TextMeshProUGUI subText;
    [SerializeField] Image backGround;

    [SerializeField] float fadeInTime = 0.5f;
    [SerializeField] float fadeOutTime = 0.5f;
    [SerializeField] float displayTime = 1f;
    [SerializeField] float moveDistance = 50f;


    [SerializeField] bool autoFadeOut = true;

    private void OnEnable()
    {
        transform.localPosition -= new Vector3(0f, moveDistance, 0f);
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        Sequence fadeInSequence = DOTween.Sequence();
        fadeInSequence.Join(transform.DOLocalMoveY(transform.localPosition.y + moveDistance, fadeInTime));

        yield return fadeInSequence.WaitForCompletion();

        yield return YieldCache.WaitForSecondsRealTime(displayTime);

        if (autoFadeOut)
            yield return FadeOutRoutine();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        Sequence fadeOutSequence = DOTween.Sequence();
        fadeOutSequence.Join(transform.DOLocalMoveY(transform.localPosition.y + moveDistance, fadeOutTime));

        yield return YieldCache.WaitForSecondsRealTime(fadeOutTime);

        Destroy(gameObject);
    }
}

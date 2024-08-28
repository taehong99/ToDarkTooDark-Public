using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineBrain brain;
    [SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] List<Image> letterBoxes;

    public CinemachineVirtualCamera VCam { set { vCam = value; lensOrthSize = vCam.m_Lens.OrthographicSize; } }

    float lensOrthSize; //vCam.m_Lens.OrthographicSize
    Transform prevTraget;


    public void ZoomIn(float time, float intensity)
    {
        StartCoroutine(Zoom(vCam.m_Lens.OrthographicSize, vCam.m_Lens.OrthographicSize - intensity, time));
    }

    public void ZoomOut(float time, float intensity)
    {
        StartCoroutine(Zoom(vCam.m_Lens.OrthographicSize, vCam.m_Lens.OrthographicSize + intensity, time));
    }

    public void RollBackZoom(float time)
    {
        StartCoroutine(Zoom(vCam.m_Lens.OrthographicSize, lensOrthSize, time));
    }

    private IEnumerator Zoom(float from, float to, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            vCam.m_Lens.OrthographicSize = Mathf.Lerp(from, to, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        vCam.m_Lens.OrthographicSize = to;
    }

    public IEnumerator FollowTargetRoutine(Transform target ,float time)
    {
        prevTraget = vCam.Follow;
        vCam.Follow = target;
        yield return ActiveLetterBox(time);

        yield return YieldCache.WaitForSecondsRealTime(time);

        vCam.Follow = prevTraget;
    }

    public IEnumerator ActiveLetterBox(float time)
    {
        for (int i = 0; i < letterBoxes.Count; i++)
        {
            int index = i; 
            DOTween.To(() => letterBoxes[index].color.a,
                       x =>
                       {
                           Color c = letterBoxes[index].color;
                           c.a = x;
                           letterBoxes[index].color = c;
                       },
                       1.0f, 0.3f).SetUpdate(true);
        }

        yield return YieldCache.WaitForSecondsRealTime(time);

        for (int i = 0; i < letterBoxes.Count; i++)
        {
            int index = i;
            DOTween.To(() => letterBoxes[index].color.a,
                       x =>
                       {
                           Color c = letterBoxes[index].color;
                           c.a = x;
                           letterBoxes[index].color = c;
                       },
                       0.0f, 0.3f).SetUpdate(true);
        }
    }

    public void FollowTarget(Transform target)
    {
        StartCoroutine(ActiveLetterBox(1.0f));
        prevTraget = vCam.Follow;
        vCam.Follow = target;
    }

    public void RollBackTarget()
    {
        vCam.Follow = prevTraget;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FPSCounter : BaseUI
{
    private void OnEnable()
    {
        StartCoroutine(ShowFPSRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Start()
    {
        
    }

    private IEnumerator ShowFPSRoutine()
    {
        while (true)
        {
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(0.5f);

            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            int FPS = Mathf.RoundToInt(frameCount / timeSpan);
            GetUI<TextMeshProUGUI>("FPS Counter").text = $"{FPS} FPS";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlateController : MonoBehaviour
{
    public UnityEvent timerStart = new UnityEvent();
    public UnityEvent timerStop = new UnityEvent();

    public int waitTime;

    private IEnumerator timer;

    public void TimerStart()
    {
        timerStart.Invoke();
        timer = Timer();
        StartCoroutine(timer);
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(waitTime);
        timerStop.Invoke();
    }
}

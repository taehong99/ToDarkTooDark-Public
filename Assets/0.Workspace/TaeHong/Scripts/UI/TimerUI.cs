using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] Text timer;
    [SerializeField] Text phaseNum;
    GamePhaseManager phase => Manager.Game.PhaseManager;

    private void Start()
    {
        UpdatePhaseNum(GamePhase.Phase1);
        phase.OnPhaseChanged += UpdatePhaseNum;
    }

    private void OnDisable()
    {
        phase.OnPhaseChanged -= UpdatePhaseNum;
    }

    private void Update()
    {
        if(phase != null)
            timer.text = ConvertToTime(phase.elapsedTime);
    }

    private string ConvertToTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdatePhaseNum(GamePhase phase)
    {
        phaseNum.text = Manager.Game.GetPhaseNumber().ToString();
    }
}

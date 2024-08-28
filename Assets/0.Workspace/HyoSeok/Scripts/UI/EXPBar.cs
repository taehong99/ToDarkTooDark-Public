using Tae;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    [SerializeField] PlayerStatsManager Exp;
    [SerializeField] Slider expSlider;
    [SerializeField] TMP_Text expText;

    public void InitPlayerExpBar()
    {
        Exp = Manager.Game.MyStats;
        Start();
    }

    private void Start()
    {
        if (Exp == null)
            return;

        expSlider.maxValue = Exp.MaxEXP;
        expSlider.value = Exp.CurEXP;
        expText.text = "1 LV";
        Exp.maxEXPChangedEvent += UpdateMaxExp;
        Exp.EXPChangedEvent += UpdateCurExp;
        Exp.levelChangedEvent += UpdateCurLevel;

    }
    private void OnDisable()
    {
        Exp.maxEXPChangedEvent -= UpdateMaxExp;
        Exp.EXPChangedEvent -= UpdateCurExp;
        Exp.levelChangedEvent -= UpdateCurLevel;
    }
    private void UpdateMaxExp(int newMaxExp)
    {
        if (expSlider == null)
            return;
        expSlider.maxValue = newMaxExp;
    }
    private void UpdateCurExp(int newCurExp)
    {
        if (expSlider == null)
            return;
        expSlider.value = newCurExp;
    }
    private void UpdateCurLevel(int newLevel)
    {
        if (expText == null)
            return;
        expText.text = $"{Exp.CurLevel} LV";
    }
}

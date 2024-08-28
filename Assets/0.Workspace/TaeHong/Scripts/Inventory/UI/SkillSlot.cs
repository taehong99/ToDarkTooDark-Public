using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Tae.Inventory;
using UnityEngine.Experimental.GlobalIllumination;

public class SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] SkillType skillType;
    [SerializeField] Image skillIcon;

    [SerializeField] Button skillLevelUpButton;
    [SerializeField] Image level1Indicator;
    [SerializeField] Image level2Indicator;
    [SerializeField] Image level3Indicator;
    [SerializeField] Color defaultColor;
    [SerializeField] Color highlightColor;

    const int MAX_SKILL_LEVEL = 3;
    private int curSkillLevel;
    public event Action<SkillType> levelUpButtonPressed;
    public event Action OnSkilllevelUp;
    private Coroutine showTooltipRoutine;

    private void OnEnable()
    {
        skillLevelUpButton.onClick.AddListener(() => levelUpButtonPressed?.Invoke(skillType));
        skillLevelUpButton.onClick.AddListener(() => Debug.Log($"{name} clicked"));
    }

    private void OnDisable()
    {
        skillLevelUpButton.onClick.RemoveAllListeners();
    }

    public void UpdateIcon(Sprite sprite)
    {
        skillIcon.sprite = sprite;
    }

    public void SkillLevelUp(SkillType skillType)
    {
        if (this.skillType != skillType)
            return;

        curSkillLevel++;
        switch (curSkillLevel)
        {
            case 1:
                level1Indicator.color = highlightColor;
                break;
            case 2:
                level2Indicator.color = highlightColor;
                break;
            case 3:
                level3Indicator.color = highlightColor;
                break;
            default:
                break;
        }

        OnSkilllevelUp?.Invoke();
    }

    public void ShowLevelUpButton()
    {
        if (curSkillLevel == MAX_SKILL_LEVEL)
            return;

        skillLevelUpButton.gameObject.SetActive(true);
    }

    public void HideLevelUpButton()
    {
        skillLevelUpButton.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        showTooltipRoutine = StartCoroutine(ShowTooltipRoutine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (showTooltipRoutine == null)
            return;

        StopCoroutine(showTooltipRoutine);
        InventoryManager.Instance.skillTooltip.gameObject.SetActive(false);
    }

    private IEnumerator ShowTooltipRoutine()
    {
        if (Manager.Game.MyPlayer == null)
            yield break;

        yield return new WaitForSeconds(InventoryManager.TOOLTIP_DISPLAY_DELAY);
        Debug.Log("show tooltip");
        InventoryManager.Instance.skillTooltip.Init(skillType);
        InventoryManager.Instance.skillTooltip.gameObject.SetActive(true);
    }
}

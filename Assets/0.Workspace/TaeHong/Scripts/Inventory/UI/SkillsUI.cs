using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsUI : MonoBehaviour
{
    // Model
    [SerializeField] PlayerStatsManager playerStats;

    // View
    [SerializeField] SkillInfoCollectionSO skillInfoCollection;
    [SerializeField] SkillSlot basicSkillSlot;
    [SerializeField] SkillSlot dashSkillSlot;
    [SerializeField] SkillSlot ultimateSkillSlot;
    [SerializeField] SkillSlot passiveSkillSlot;

    // Presenter (this)
    private Dictionary<SkillType, SkillSlot> skillSlotDic = new();
    int skillPoints;
    private Coroutine levelUpRoutine;

    public void Init()
    {
        playerStats = Manager.Game.MyStats;
        Start();
        OnEnable();
        OnPlayerLevelUp(playerStats.CurLevel);
        UpdateIcons();
    }

    private void Start()
    {
        if (playerStats == null)
            return;

        skillSlotDic.Add(SkillType.Skill, basicSkillSlot);
        skillSlotDic.Add(SkillType.Dash, dashSkillSlot);
        skillSlotDic.Add(SkillType.Utimate, ultimateSkillSlot);
        skillSlotDic.Add(SkillType.Passive, passiveSkillSlot);
        SubscribeToSkillSlotEvents();
    }

    private void OnEnable()
    {
        if (playerStats == null)
            return;

        playerStats.levelChangedEvent += OnPlayerLevelUp;
    }

    private void UpdateIcons()
    {
        SkillInfo info = skillInfoCollection.GetMySkillInfo(Manager.Game.MyJob);
        basicSkillSlot.UpdateIcon(info.skillIcon);
        dashSkillSlot.UpdateIcon(info.dashIcon);
        ultimateSkillSlot.UpdateIcon(info.ultimateIcon);
        passiveSkillSlot.UpdateIcon(info.passiveIcon);
    }

    // 스킬슬롯 버튼에 레벨업 기능 연결
    private void SubscribeToSkillSlotEvents()
    {
        foreach(var slot in skillSlotDic)
        {
            slot.Value.levelUpButtonPressed += OnSkillLevelUp;
        }
    }

    private void OnSkillLevelUp(SkillType skillType)
    {
        playerStats.SkillLevelUp(skillType);
        skillSlotDic[skillType].SkillLevelUp(skillType);
        skillPoints--;

        DisableSkillLevelUp();
        AllowSkillLevelUp();
    }

    private void OnPlayerLevelUp(int level)
    {
        if (level <= 2)
        {
            skillPoints += 2;
        }
        else
        {
            skillPoints += 1;
        }

        if(levelUpRoutine == null)
            levelUpRoutine = StartCoroutine(SkillLevelUpRoutine());
    }

    // 스킬 포인트가 1개 이상일때 레벨업 UI 켜줌
    private IEnumerator SkillLevelUpRoutine()
    {
        AllowSkillLevelUp();
        while (skillPoints > 0)
        {
            yield return null;
        }
        DisableSkillLevelUp();
        levelUpRoutine = null;
    }

    private void AllowSkillLevelUp()
    {
        foreach (var slot in skillSlotDic)
        {
            slot.Value.ShowLevelUpButton();
        }
    }

    private void DisableSkillLevelUp()
    {
        foreach (var slot in skillSlotDic)
        {
            slot.Value.HideLevelUpButton();
        }
    }
}

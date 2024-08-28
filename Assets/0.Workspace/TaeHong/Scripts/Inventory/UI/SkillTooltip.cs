using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTooltip : MonoBehaviour
{
    [SerializeField] SkillInfoCollectionSO skillInfoCollection;

    [SerializeField] Image skillIcon;
    [SerializeField] Text skillName;
    [SerializeField] Text skillLevel;
    [SerializeField] Text skillKey;
    [SerializeField] Text skillCooldown;
    [SerializeField] Text skillDesc;
    [SerializeField] Text skillDamagePerLevel;
    [SerializeField] Text skillCooldownPerLevel;

    private SkillInfo skillInfo => skillInfoCollection.GetMySkillInfo(Manager.Game.MyJob);
    private PlayerData skillData;

    [SerializeField] RectTransform rectTransform;
    private float xOffset = 200;
    private float yOffset = 125;

    private void OnEnable()
    {
        RefreshContentSize();
    }

    public void Init(SkillType type)
    {
        if(skillData == null)
            skillData = Manager.Game.MyPlayer.GetComponent<PlayerData>();

        switch (type)
        {
            case SkillType.Skill:
                DisplaySkill();
                break;
            case SkillType.Dash:
                DisplayDash();
                break;
            case SkillType.Utimate:
                DisplayUltimate();
                break;
            case SkillType.Passive:
                DisplayPassive();
                break;
        }
    }

    private void RefreshContentSize()
    {
        IEnumerator Routine()
        {
            var csf = GetComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            yield return null;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        StartCoroutine(Routine());
    }

    private void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 anchoredPosition;
        // Convert the screen point to local point in the RectTransform
        RectTransform parentRectTransform = rectTransform.parent as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, mousePosition, null, out anchoredPosition);

        // Deafault offset
        Vector2 offset = new Vector2(xOffset, -yOffset);

        // Get the parent canvas size
        Vector2 parentSize = parentRectTransform.rect.size;


        // Dynamic offset pos via mousePos 
        if (mousePosition.x > parentSize.x - xOffset * 2)
        {
            offset.x = -xOffset;
        }

        if (mousePosition.y < parentSize.y * 0.5)
        {
            offset.y = yOffset;
        }

        // Update the RectTransform's anchored position
        rectTransform.anchoredPosition = anchoredPosition + offset;
    }

    private void DisplaySkill()
    {
        skillIcon.sprite = skillInfo.skillIcon;
        skillName.text = skillInfo.skillName;
        skillDesc.text = skillInfo.skillDescription;
        skillKey.text = "[E]";
        skillCooldown.text = $"재사용 대기시간 {skillData.skillCoolTime[1]}초";
        skillDamagePerLevel.text =
            $"데미지 배율 [" +
            $"{skillData.skillDamageMultiplier[1] * 100}%/" +
            $"{skillData.skillDamageMultiplier[2] * 100}%/" +
            $"{skillData.skillDamageMultiplier[3] * 100}%]";
        skillCooldownPerLevel.text =
            $"재사용 대기시간 [" +
            $"{skillData.skillCoolTime[1]}/" +
            $"{skillData.skillCoolTime[2]}/" +
            $"{skillData.skillCoolTime[3]}]";
    }

    private void DisplayDash()
    {
        skillIcon.sprite = skillInfo.dashIcon;
        skillName.text = skillInfo.dashName;
        skillDesc.text = skillInfo.dashDescription;
        skillKey.text = "[SPACE]";
        skillCooldown.text = $"재사용 대기시간 {skillData.dashCoolTime[1]}초";
        skillDamagePerLevel.text =
            $"데미지 배율 [" +
            $"{skillData.dashDamageMultiplier[1] * 100}%/" +
            $"{skillData.dashDamageMultiplier[2] * 100}%/" +
            $"{skillData.dashDamageMultiplier[3] * 100}%]";
        skillCooldownPerLevel.text =
            $"재사용 대기시간 [" +
            $"{skillData.dashCoolTime[1]}/" +
            $"{skillData.dashCoolTime[2]}/" +
            $"{skillData.dashCoolTime[3]}]";
    }

    private void DisplayUltimate()
    {
        skillIcon.sprite = skillInfo.ultimateIcon;
        skillName.text = skillInfo.ultimateName;
        skillDesc.text = skillInfo.ultimateDescription;
        skillKey.text = "[Q]";
        skillCooldown.text = $"재사용 대기시간 {skillData.utiCoolTime[1]}초";
        skillDamagePerLevel.text =
            $"데미지 배율 [" +
            $"{skillData.utiDmageMultiplier[1] * 100}%/" +
            $"{skillData.utiDmageMultiplier[2] * 100}%/" +
            $"{skillData.utiDmageMultiplier[3] * 100}%]";
        skillCooldownPerLevel.text =
            $"재사용 대기시간 [" +
            $"{skillData.utiCoolTime[1]}/" +
            $"{skillData.utiCoolTime[2]}/" +
            $"{skillData.utiCoolTime[3]}]";
    }

    private void DisplayPassive()
    {
        skillIcon.sprite = skillInfo.passiveIcon;
        skillName.text = skillInfo.passiveName;
        skillDesc.text = skillInfo.passiveDescription;
        skillKey.text = "[PASSIVE]";
        skillCooldown.text = "패시브 스킬";
        skillDamagePerLevel.text =
            $"데미지 배율 [" +
            $"{skillData.passiveMultiplier[1] * 100}%/" +
            $"{skillData.passiveMultiplier[2] * 100}%/" +
            $"{skillData.passiveMultiplier[3] * 100}%]";
        skillCooldownPerLevel.text = "";
    }
}

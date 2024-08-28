using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/SkillInfoCollection")]
public class SkillInfoCollectionSO : ScriptableObject
{
    public SkillInfo swordsmanSkillInfo;
    public SkillInfo archerSkillInfo;
    public SkillInfo priestSkillInfo;
    public SkillInfo wizardSkillInfo;

    public SkillInfo GetMySkillInfo(PlayerJob job)
    {
        switch(job)
        {
            case PlayerJob.Swordsman:
                return swordsmanSkillInfo;
            case PlayerJob.Archer:
                return archerSkillInfo;
            case PlayerJob.Priest:
                return priestSkillInfo;
            case PlayerJob.Wizard:
                return wizardSkillInfo;
            default:
                return swordsmanSkillInfo;
        }
    }
}

[System.Serializable]
public struct SkillInfo
{
    public Sprite attackIcon;
    public Sprite skillIcon;
    public Sprite dashIcon;
    public Sprite ultimateIcon;
    public Sprite passiveIcon;

    public string attackName;
    public string skillName;
    public string dashName;
    public string ultimateName;
    public string passiveName;

    public string attackDescription;
    public string skillDescription;
    public string dashDescription;
    public string ultimateDescription;
    public string passiveDescription;
}
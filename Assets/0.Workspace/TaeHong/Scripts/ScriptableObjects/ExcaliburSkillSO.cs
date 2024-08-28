using UnityEngine;

[CreateAssetMenu(menuName = "Skills/ExcaliburSkill")]
public class ExcaliburSkillSO : ScriptableObject
{
    public int id;
    public Sprite icon;
    public float cooldown;
}

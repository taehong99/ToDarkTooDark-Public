using UnityEngine;

/// <summary>
/// 미개봉 포션 데이터
/// </summary>
[CreateAssetMenu(menuName = "ItemData/UnknownPotionData")]
public class UnknownPotionData : ScriptableObject
{
    public int id;
    public Sprite sprite;
    public string potionName;
    public string tooltip;
}

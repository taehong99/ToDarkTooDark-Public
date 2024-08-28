using UnityEngine;

[CreateAssetMenu(menuName = "Item/Potion", fileName = "Potion")]
public class PotionSO : ScriptableObject
{
    public int potionID;
    public PotionGrade grade;
    public PlayerStatusEffect statusEffect;
    public float duration;
    public float coolTime;
    public float drop;
    public int sellPrice;
    public int buyPrice;
}

public enum PotionGrade
{
    Nomal,
    Epic,
    Rare,
    Unique,
    Legendary
}

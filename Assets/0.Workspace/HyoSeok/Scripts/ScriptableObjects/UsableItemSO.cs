using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/UsableItem", fileName = "UsableItem")]
public class UsableItemSO : ScriptableObject
{
    public int ItemID;
    // 아이템 이름
    public string itemName;
    // 설치 지속 시간
    public float installationDuration;
    // 효과 지속 시간
    public float effectDuration;
    // 쿨타임
    public float cooldown;
    // 상점 구매 가격
    public int purchasePrice;
    // 상점 판매 가격
    public int sellPrice;
}

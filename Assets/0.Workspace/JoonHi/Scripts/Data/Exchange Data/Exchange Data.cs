using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Exchange Info Data")]
public class ExchangeData : ScriptableObject
{
    public string ItemName;
    public Sprite ExchangeSprite;
    public Animator animator;       // UI 상호작용하기위 쓸 애니메이터
    public int cost;                // 상점 교환 가격
}
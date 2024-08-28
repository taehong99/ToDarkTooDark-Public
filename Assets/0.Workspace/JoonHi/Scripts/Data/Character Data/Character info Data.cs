using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/Character Info Data")]
public class CharacterInfoData : ScriptableObject
{
    public PlayerJob job;
    public string CharacterName;
    public Sprite CharacterSprite;
    public Animator animator;       // UI 상호작용하기위 쓸 애니메이터
    public int cost;                // 상점 캐릭터 가격
    public Weapon weapon;

    public enum Weapon { Sword, Wand, Bow};
}

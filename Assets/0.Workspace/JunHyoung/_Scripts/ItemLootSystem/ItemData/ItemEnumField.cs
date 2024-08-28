using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemLootSystem
{
    /* ////////// 비고 //////////
     *  
     *  - Legendary 등급 : 게임 시작 1분까지 드랍 X
     *  - Artifact 등급 : 게임 시작 4분까지 드랍 X
     *                  : 특수 유틸기능 존재(스탯 추가 증감등)
     * 
     *  - 엑수칼리버 : 게임 시작 4분까지 드랍 X
     * 
     * 
     //////////////////////////*/

   

    /// <summary>
    /// 아이템 레어도
    /// </summary>
    [SerializeField]
    public enum ItemRarity
    {
        Normal = 0,
        Rare = 1,
        Epic = 2,
        Unique = 3,
        Legendary = 4,
        Artifact = 5,
        Excalibur = 6
    }

    /// <summary>
    /// 아이템 분류
    /// </summary>
    [SerializeField]
    public enum ItemType
    {
        Consumable = 0,
        Equipment = 1,
        ETC = 2,
        Key = 3,
        Upgrade = 4,
    }

    /// <summary>
    /// 무기 분류
    /// </summary>
    [SerializeField]
    public enum WeaponType
    {
        NULL = 0,
        Sword = 1,
        Bow =2,
        TheCross =3,
        Wand = 4,
        ExuCaliver = 5,
        Excalibur =6
    }

    /// <summary>
    /// 장비군 (세트아이템과 같은데 쓰일 필드)
    /// </summary>
    [SerializeField]
    public enum EquipmentGroup
    {
        NULL = 0,
        Normal =1,
        Improvement =2,
        Betterment =3,
        Proper =4,
        Light =5,
        Durable =6,
        Essense =7,
        Overload = 8,
        Heaviness = 9,
        Legened = 10,
        Special = 11,
        Focus = 12,
        ExuCaliver = 13,
    }

    
    //[SerializeField]
    /*public enum StatType
    {
        /// <summary>
        /// 공격력
        /// </summary>
        ATK = 1,
        /// <summary>
        /// 방어력
        /// </summary>
        DEF = 2,
        /// <summary>
        /// 체력
        /// </summary>
        HP = 3,
        /// <summary>
        /// 초당 체력 회복
        /// </summary>
        HPGEN = 4,
        /// <summary>
        /// 이동 속도
        /// </summary>
        MovementSpeed =5,
        /// <summary>
        /// 크리티컬 확률
        /// </summary>
        CritRate = 6,
        /// <summary>
        /// 크리티컬 배율
        /// </summary>
        CritMult = 7,
    }
    */
}

/// <summary>
/// 장비 분류
/// </summary>
public enum EquipmentType
{
    NULL = 0,
    Armor = 1,
    Accessory = 2,
    Weapon = 3,
}


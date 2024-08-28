using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sound", fileName = "SoundData")]
public class SoundSO : ScriptableObject
{
    [Header("BGM/InGame")]
    public AudioClip StartBGM;
    public AudioClip PhaseBGM;

    [Header("BGM/OutGame")]
    public AudioClip WaitingRoomBGM;
    public AudioClip LobbyBGM;
    public AudioClip MainBGM;
    public AudioClip StoreBGM;
    public AudioClip TutorialBGM;

    [Header("SFX/Character")]
    public AudioClip CoinSFX;
    public AudioClip AccessoriesSFX;
    public AudioClip SkillUpSFX;
    public AudioClip ArmorSFX;
    public AudioClip LevelUpSFX;
    public AudioClip WeaponSFX;

    [Header("SFX/WaitingRoom")]
    public AudioClip ReadySFX;
    public AudioClip ReadyCacelSFX;

    [Header("SFX/ETC")]
    public AudioClip WinSFX;
    public AudioClip DefeatSFX;

    [Header("SFX/Excalibur")]
    public AudioClip EXAttackSFX;
    public AudioClip EXSkill1SFX;
    public AudioClip EXSkill2SFX;
    public AudioClip EXSkill2ChargieSFX;
    public AudioClip EXSkill2ExplosionSFX;
    public AudioClip EXSkill3SFX;
    public AudioClip EXSkill3MoveSoundSFX;
    public AudioClip ExJewelryInstalled1SFX;
    public AudioClip ExJewelryInstalled2SFX;
    public AudioClip ExJewelryInstalled3SFX;
    public AudioClip ExEquipped;

    [Header("SFX/Announcer")]
    public AudioClip SecretRoom1SFX;
    public AudioClip SecretRoom2SFX;
    public AudioClip SecretRoomLastSFX;
    public AudioClip GameStartSFX;
    public AudioClip GameEndSFX;
    public AudioClip OpenExcaliburSFX;
    public AudioClip FastExcaliburSFX;
}

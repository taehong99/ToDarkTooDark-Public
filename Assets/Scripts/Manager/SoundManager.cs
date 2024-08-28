using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] public SoundSO SoundSO;
    [SerializeField] public AudioSource bgmSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource walkSfxSource;
    [SerializeField] AudioSource wiazrdUtiSource;

    [Space(10), Header("Mouse SFX")][SerializeField] AudioClip mouseClip;

    public float BGMVolme { get { return bgmSource.volume; } set { bgmSource.volume = value; } }
    public float SFXVolme { get { return sfxSource.volume; } set { sfxSource.volume = value; } }
    public float WalkSFXVolme { get { return walkSfxSource.volume; } set { walkSfxSource.volume = value; } }
    public float wiazrdUtiVolm { get { return wiazrdUtiSource.volume; } set { wiazrdUtiSource.volume = value; } }
    public void WalkSFXPlay(AudioClip clip)
    {
        if (walkSfxSource.clip == clip)
            return;
        walkSfxSource.clip = clip;
        walkSfxSource.Play();
    }
    public void WalkSfxStop()
    {
        if (walkSfxSource.isPlaying == false)
            return;
        walkSfxSource.clip = null;
        walkSfxSource.Stop();

    }
    public void WizardSFXPlay(AudioClip clip)
    {
        if (wiazrdUtiSource.clip == clip)
            return;
        wiazrdUtiSource.clip = clip;
        wiazrdUtiSource.Play();
    }
    public void WizardSfxStop()
    {
        if (wiazrdUtiSource.isPlaying == false)
            return;
        wiazrdUtiSource.clip = null;
        wiazrdUtiSource.Stop();

    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying == false)
            return;

        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        if (sfxSource.isPlaying == false)
            return;

        sfxSource.Stop();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if(EventSystem.current.IsPointerOverGameObject())
                PlaySFX(mouseClip);
        }
    }

}

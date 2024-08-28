using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class VideoPopUpUI : PopUpUI, IPointerClickHandler
{
    [SerializeField] VideoClip clip;
    [SerializeField] VideoPlayer player;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.clickCount == 1)
        {
            if(player.isPlaying){ 
                player.Pause();
            }
            else
            {
                player.Play();
            }
        }

        if(eventData.clickCount == 2)
        {
            Manager.Sound.bgmSource.Play();
            Close();
        }
    }

    protected override void Awake()
    {
        //base.Awake(); // for Skip Bind

        Manager.Sound.StopBGM();
        player.clip  = clip;
        player.loopPointReached += EndReached; // video end call back
        player.Play();
       
    }

    void EndReached(VideoPlayer player)
    {
        Manager.Sound.bgmSource.Play();
        Close();
    }

}

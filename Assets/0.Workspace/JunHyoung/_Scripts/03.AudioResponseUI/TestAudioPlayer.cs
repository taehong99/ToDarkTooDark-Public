using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudioPlayer : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;

    void Start()
    {
        Manager.Sound.PlayBGM(audioClip);
    }

    private void OnDisable()
    {
        Manager.Sound.StopBGM();
    }
}

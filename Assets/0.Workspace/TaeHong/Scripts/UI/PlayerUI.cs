using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] Image portraitImage;
    [SerializeField] Sprite swordsmanSprite;
    [SerializeField] Sprite archerSprite;
    [SerializeField] Sprite priestSprite;
    [SerializeField] Sprite wizardSprite;

    private EXPBar expBar;
    private HealthBar healthBar;

    private void Awake()
    {
        expBar = GetComponent<EXPBar>();
        healthBar = GetComponent<HealthBar>();
    }

    public void Init()
    {
        expBar.InitPlayerExpBar();
        healthBar.InitPlayerHealthBar();

        switch (PhotonNetwork.LocalPlayer.GetJob())
        {
            case PlayerJob.Swordsman:
                portraitImage.sprite = swordsmanSprite;
                break;
            case PlayerJob.Archer:
                portraitImage.sprite = archerSprite;
                break;
            case PlayerJob.Priest:
                portraitImage.sprite = priestSprite;
                break;
            case PlayerJob.Wizard:
                portraitImage.sprite = wizardSprite;
                break;
        }
    }
}

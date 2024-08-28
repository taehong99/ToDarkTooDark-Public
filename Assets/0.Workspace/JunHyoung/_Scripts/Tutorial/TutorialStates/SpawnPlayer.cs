using ItemLootSystem;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : TutorialBase
{
    [SerializeField] GameObject player;
    [SerializeField] Transform PlayerSpawnPoint;
    [SerializeField] SkillsUI skillsUI; //24.08.12 PJH 플레이어 인스턴스 생성 후 수동으로 초기화 해주기 위해서 추가

    private PhotonPlayerController playerController;

    public override void Enter()
    {
        Manager.Game.MyPlayer = Instantiate(player, PlayerSpawnPoint.position, Quaternion.identity);
        playerController = Manager.Game.MyPlayer.GetComponent<PhotonPlayerController>();
        playerController.SetupTutorialPlayer();
        Manager.Game.MyStats = playerController.GetComponent<PlayerStatsManager>();
        Manager.Game.MyWeaponType = WeaponType.Sword;
        TutorialManager.Instance.Player = playerController;
        TutorialManager.Instance.Cam.VCam = playerController.VirtualCamera;
        TutorialManager.Instance.Cam.FollowTarget(playerController.transform);

        Exit();
    }

    public override void Excute(TutorialManager tutorialManager)
    {
        if(isCompleted)
        {
            tutorialManager.NextTutorial();
        }
    }

    public override void Exit()
    {
        isCompleted = true;
    }

    
}

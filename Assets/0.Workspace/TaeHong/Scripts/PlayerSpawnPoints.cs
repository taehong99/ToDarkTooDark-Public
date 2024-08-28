using ItemLootSystem;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoints : MonoBehaviour
{
    public Transform[] spawnPoints;

    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
    }

    public void SpawnPlayer(int playerIdx)
    {
        Vector3 spawnPos = spawnPoints[playerIdx].position;

        if (playerIdx != 1)
        {
            Manager.Game.MyPlayer = PhotonNetwork.Instantiate("Characters/PhotonPriest", spawnPos, Quaternion.identity);
            Manager.Game.MyWeaponType = WeaponType.TheCross;
            Manager.Game.MyJob = PlayerJob.Priest;
            PhotonNetwork.LocalPlayer.SetTeam(Team.Blue);
        }
        else
        {
            //Manager.Game.MyPlayer = PhotonNetwork.Instantiate("Characters/PhotonArcher", spawnPos, Quaternion.identity);
            Manager.Game.MyPlayer = PhotonNetwork.Instantiate("Characters/PhotonWizard", spawnPos, Quaternion.identity);
            Manager.Game.MyWeaponType = WeaponType.Wand;
            Manager.Game.MyJob = PlayerJob.Wizard;
            PhotonNetwork.LocalPlayer.SetTeam(Team.Red);
        }
        Manager.Game.MyStats = Manager.Game.MyPlayer.GetComponent<PlayerStatsManager>();

        //switch (Manager.Game.playerClass)
        //{
        //    case PlayerClass.Swordsman:
        //        PhotonNetwork.Instantiate("Characters/PhotonSwordsman", spawnPos, Quaternion.identity);
        //        break;
        //    case PlayerClass.Archer:
        //        PhotonNetwork.Instantiate("Characters/PhotonArcher", spawnPos, Quaternion.identity);
        //        break;
        //}
    }
}

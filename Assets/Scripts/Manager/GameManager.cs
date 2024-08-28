using MapGenerator;
using Photon.Pun;
using System.Collections.Generic;
using Tae;
using ItemLootSystem;
using UnityEngine;
using Photon.Realtime;

public class GameManager : Singleton<GameManager>
{
    private GamePhaseManager phase;
    public GamePhaseManager PhaseManager { get { return phase; } set { phase = value; } }
    public GamePhase Phase { get { return phase.CurrentPhase; } }

    public Dictionary<int, GameObject> playerDic = new Dictionary<int, GameObject>();

    public GameObject MyPlayer;
    public PlayerStatsManager MyStats;
    public WeaponType MyWeaponType;
    public PlayerJob MyJob;
    
    public Vector3 PlayerPos => MyPlayer.transform.position;

    public Exit MyExit; // Excalibur Exit

    public bool GameMode { get; set; }

    private void OnDestroy()
    {
        playerDic.Clear();
    }

    public void SpawnPlayer(Vector2 spawnPos)
    {
        MyJob = PhotonNetwork.LocalPlayer.GetJob();
        // 플레이어 생성
        switch (MyJob)
        {
            case PlayerJob.Swordsman:
                MyPlayer = PhotonNetwork.Instantiate("Characters/PhotonSwordsman", spawnPos, Quaternion.identity);
                MyWeaponType = WeaponType.Sword;
                break;
            case PlayerJob.Archer:
                MyPlayer = PhotonNetwork.Instantiate("Characters/PhotonArcher", spawnPos, Quaternion.identity);
                MyWeaponType = WeaponType.Bow;
                break;
            case PlayerJob.Priest:
                MyPlayer = PhotonNetwork.Instantiate("Characters/PhotonPriest", spawnPos, Quaternion.identity);
                MyWeaponType = WeaponType.TheCross;
                break;
            case PlayerJob.Wizard:
                MyPlayer = PhotonNetwork.Instantiate("Characters/PhotonWizard", spawnPos, Quaternion.identity);
                MyWeaponType = WeaponType.Wand;
                break;
            default:
                MyPlayer = PhotonNetwork.Instantiate("Characters/PhotonSwordsman", spawnPos, Quaternion.identity);
                MyWeaponType = WeaponType.Sword;
                break;
        }

        MyStats = MyPlayer.GetComponent<PlayerStatsManager>();
        Manager.Event.PlayerSpawned();
    }

    [ContextMenu("Game Phase Start!")]
    public void InitPhaseManager()
    {
        /*
        if (phase != null)
        {
            phase.OnPhaseChanged = null;
            DestroyImmediate(phase); // DontDestroyOnLoad도 쌩깜 짱쎔
        }*/
        if(phase == null)
            phase = gameObject.AddComponent<GamePhaseManager>();
        phase.StartTimer();
        //phase.OnPhaseChanged += MapManager.Instance.PhaseChangeObserver;  // 필요한 곳에서 Manager.Game.PhaseManager.OnPhaseChanged 호출
    }

    public int GetPhaseNumber()
    {
        if (phase == null)
            return 1;

        switch (phase.CurrentPhase)
        {
            case GamePhase.Phase1:
                return 1;
            case GamePhase.Phase2:
                return 2;
            case GamePhase.Phase3:
                return 3;
            case GamePhase.Phase4:
                return 4;
            case GamePhase.Phase5:
                return 5;
            default:
                Debug.LogWarning("INVALID PHASE");
                return -1;
        }
    }

    GameObject playerToSpectate;

    public void SpectateTeammate()
    {
        GameObject[] playersInScene = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in playersInScene)
        {
            if(player.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer.GetTeammate())
            {
                playerToSpectate = player;
                MyPlayer.GetComponent<PhotonPlayerController>().VirtualCamera.m_Follow = player.transform;
                player.GetComponent<PhotonPlayerController>().light2D.gameObject.SetActive(true);
            }
        }
    }

    public void ReturnCameraToSelf()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        MyPlayer.GetComponent<PhotonPlayerController>().VirtualCamera.m_Follow = null;
        MyPlayer.GetComponent<PhotonPlayerController>().VirtualCamera.transform.localPosition = new Vector3(0, 0, -10);
        playerToSpectate.GetComponent<PhotonPlayerController>().light2D.gameObject.SetActive(false);
    }
}

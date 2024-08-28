using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ItemLootSystem;

namespace Tae
{
    public class DebugGameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] bool soloGameMode;
        [SerializeField] MonsterSpawnPoint[] spawnPoints;
        [SerializeField] ExcaliburSpawnTile excaliburSpawnTile;

        private void Awake()
        {
            Manager.Game.PhaseManager = FindObjectOfType<GamePhaseManager>();
        }

        void Start()
        {
            PhotonNetwork.LocalPlayer.NickName = $"DebugPlayer {Random.Range(1000, 10000)}";
            PhotonNetwork.ConnectUsingSettings();
            //PhotonNetwork.JoinRandomOrCreateRoom();
        }

        private void GameStart()
        {
            Manager.Game.GameMode = PhotonNetwork.CurrentRoom.GetGameMode();
            //Manager.Game.PhaseManager.StartTimer();
            //Manager.Game.MyExit = FindObjectOfType<Exit>();
            FindObjectOfType<PlayerSpawnPoints>().SpawnPlayer(PhotonNetwork.LocalPlayer.ActorNumber);
            FindObjectOfType<PlayerUI>().Init();
            FindObjectOfType<SkillsUI>().Init();
            FindObjectOfType<ItemFactory>().DebugSpawnItems();

            if (!PhotonNetwork.IsMasterClient)
                return;

            excaliburSpawnTile.SpawnExcalibur();
            foreach(var spawnPoint in spawnPoints)
            {
                spawnPoint.SpawnMonster();
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master server. Attempting to join a random room...");
            RoomOptions roomOptions = new RoomOptions();
            PhotonNetwork.JoinOrCreateRoom($"HyoPhotonTestRoom{Random.Range(0,10000)}", roomOptions, TypedLobby.Default);
        }

        public override void OnJoinedRoom()
        {
            // Called when successfully joined a room
            Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
            PhotonNetwork.CurrentRoom.SetGameMode(soloGameMode);
            GameStart();
        }

        public override void OnCreatedRoom()
        {
            // Called when a new room is successfully created
            Debug.Log("Created room: " + PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            // Called when disconnected from the server
            Debug.LogWarning($"Disconnected from Photon server: {cause}");
        }
    }
}

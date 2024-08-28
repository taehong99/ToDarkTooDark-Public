using ItemLootSystem;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    /// <summary>
    /// MapGenerator가 맵 생성하고난 이후, 나머지 맵에 수행되는 작업들은 여기서 진행
    ///
    /// By OnPhaseChanged Event
    /// - 페이즈 진행에 따라 비밀방 활성화
    /// 
    /// In AfterGenerateMap()
    /// - 일반방 돌돌이, 먹돌이 스폰  //Master에서만 수행
    /// - 일반방 보물상자 스폰   //Master에서만 수행
    /// 
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        static MapManager instance;
        public static MapManager Instance { get { return instance; } }

        public int seed;

        Dictionary<Vector2, Room> rooms;
        public Dictionary<Vector2, Room> Rooms { get { return rooms; } set { rooms = value; AfterGenerateMap(); } }
        [SerializeField] List<SecretRoom> secretRoomPrefabs;

        //
        Dictionary<Vector3, GameObject> objectDic = new Dictionary<Vector3, GameObject>();
        public Dictionary<Vector3, GameObject> ObjectDic { get { return objectDic; } }

       
        public Dictionary<int, Door> doorDic = new Dictionary<int, Door>();
        public int doorID = 0;


        // 전역변수
        List<SpecialRoom> specialRooms;
        List<NormalRoom> normalRooms;

        Vector3 secretRoomPos = new Vector3(500, 0, 0);
        Vector3 secretRoomPosOffset = new Vector3(0, 50, 0);
        int cnt = 0;

        // 먹돌이, 돌돌이 스폰 수
        const int DolSpawnCount = 15;
        const int GoldenDolSpawnCount = 5;

        private void Awake()
        {
            CreateInstance();
        }

        private void OnDisable()
        {
            Manager.Game.PhaseManager.OnPhaseChanged -= PhaseChangeObserver;
        }
        void CreateInstance()
        {
            if (instance == null)
            {
                instance = this;
                Manager.Game.PhaseManager.OnPhaseChanged += PhaseChangeObserver;
            }
            else
            {
                Destroy(instance);
            }
        }
        /// <summary>
        /// SpecialRoom 구분 후 시드값에 기반해 셔플해 specialRooms에 저장.
        /// </summary>
        void FilteredSpecialRooms()
        {
            specialRooms = new List<SpecialRoom>();

            foreach (var room in rooms.Values)
            {
                if (room.roomType == RoomType.Special)
                {
                    specialRooms.Add((SpecialRoom) room);
                }
            }

            Random.InitState(seed);
            for (int i = specialRooms.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = specialRooms[i];
                specialRooms[i] = specialRooms[j];
                specialRooms[j] = temp;
            }
        }

        void FilteredNormalRooms()
        {
            normalRooms = new List<NormalRoom>();

            foreach (var room in rooms.Values)
            {
                if (room.roomType == RoomType.Normal)
                {
                    normalRooms.Add((NormalRoom) room);
                }
            }

            Random.InitState(seed);
            for (int i = normalRooms.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = normalRooms[i];
                normalRooms[i] = normalRooms[j];
                normalRooms[j] = temp;
            }
        }

        //페이즈 전환 이벤트에서 호출, 현 페이즈에 따라 특수방 활성화
        public void PhaseChangeObserver(GamePhase phase)
        {
            if (phase == GamePhase.Phase2)
            {
                Manager.Sound.PlaySFX(Manager.Sound.SoundSO.SecretRoom1SFX);
                ActivateSpecialRoom();
                ActivateSpecialRoom();
            }

            if (phase == GamePhase.Phase3)
            {
                Manager.Sound.PlaySFX(Manager.Sound.SoundSO.SecretRoom2SFX);
                ActivateSpecialRoom();
                ActivateSpecialRoom();
            }

            if (phase == GamePhase.Phase4)
            {
                Manager.Sound.PlaySFX(Manager.Sound.SoundSO.SecretRoomLastSFX);
                ActivateSpecialRoom();
            }
        }

        //특수방 활성화(비밀방가는 통로생성) 시켜주는 메서드
        [ContextMenu("ActivateSpecialRoom")]
        void ActivateSpecialRoom()
        {
            Debug.Log(specialRooms[cnt].name);
            specialRooms[cnt].ActivateSecretRoom(secretRoomPrefabs[cnt], secretRoomPos);
            secretRoomPos += secretRoomPosOffset;
            cnt++;
        }

        // 맵 생성 이후, Manger에 Rooms 데이터가 세팅되고 나면...
        void AfterGenerateMap()
        {
            Debug.Log("After Generate Map");
            FilteredSpecialRooms();
            FilteredNormalRooms();

            if (!PhotonNetwork.IsMasterClient)
                return;

            SpawnTreasureBoxes();
            MuckdolSpawn();
            DolDolSpawn();
            MerchantSpawn();
        }


        //일반방에서 확률에 맞게 각 등급의 보물상자 스폰 : https://docs.google.com/spreadsheets/d/17T-mBoPHtTC-N7l_Yo0VatIaB-623-_CaHS1GlmLQ_w/edit?gid=417214557#gid=417214557
         void SpawnTreasureBoxes()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            //일반방의 보물상자 스폰포인트 추가
            List<Vector3> treasureSpawnPoints = new List<Vector3>();
            AddTreasureSpawnPoints(treasureSpawnPoints);

            //Debug.Log($"보물상자 스폰장소 : {treasureSpawnPoints.Count}");
            
            InstantiateTreasureBoxes(treasureSpawnPoints);
        }

        void AddTreasureSpawnPoints(List<Vector3> spawnPoints)
        {
            foreach (var room in normalRooms)
            {
                if (room.TreasureSpawnPoints != null) //고정위치 spawnPoitns에 추가
                {
                    foreach (var t in room.TreasureSpawnPoints)
                    {
                        if (t == null) continue;

                        t.gameObject.SetActive(false);
                        spawnPoints.Add(t.position);
                    }
                }
                else // 혹은 랜덤위치 추가
                {
                    List<Vector3> points = room.GetTreasureSpawnPositions();
                    if(points != null)  
                        spawnPoints.AddRange(points);
                }
            }
        }

        //D급 보물상자	C급 보물상자	B급 보물상자	A급 보물상자	S급 보물상자
        //    39%	       30%	            20%	             8%	            3%

        static Dictionary<TreasureRarity, float> TreasureBoxSapwnRate = new Dictionary<TreasureRarity, float>
        { { TreasureRarity.D, 0.39f },
          { TreasureRarity.C , 0.3f },
          { TreasureRarity.B, 0.2f},
          {TreasureRarity.A, 0.08f },
          {TreasureRarity.S, 0.03f }
        };

        void InstantiateTreasureBoxes(List<Vector3> spawnPoints)
        {
            for (int i = 0; i < spawnPoints.Count; i++)
            {
                float value = Random.value;
                TreasureRarity rarity = GetRarity(value);
                string treasureBoxPrefabName = GetTreasureBoxPrefabName(rarity);

                PhotonNetwork.InstantiateRoomObject(treasureBoxPrefabName, spawnPoints[i], Quaternion.identity);
            }
        }

        TreasureRarity GetRarity(float value)
        {
            float cumulative = 0f;
            foreach (var pair in TreasureBoxSapwnRate)
            {
                cumulative += pair.Value;
                if (value < cumulative)
                {
                    return pair.Key;
                }
            }
            return TreasureRarity.D; // Default fallback, should never hit
        }

        public string GetTreasureBoxPrefabName(TreasureRarity rarity)
        {
            switch (rarity)
            {
                case TreasureRarity.D: return "Treasure Box D";
                case TreasureRarity.C: return "Treasure Box C";
                case TreasureRarity.B: return "Treasure Box B";
                case TreasureRarity.A: return "Treasure Box A";
                case TreasureRarity.S: return "Treasure Box S";
                default: return "Treasure Box D";
            }
        }


        // 보물상자 할당 완료 후 호출
        // 먹돌이 스폰 : https://docs.google.com/document/d/17rTr-_mOsnE33sCCkDgXz9n5vQgawG1H0HhaSXD53nM/edit
        // 일반 먹돌이 - 전체 일반맵중 15개
        // 황금 먹돌이 - 전체 일반맵중 5개
         void MuckdolSpawn()
        {
            for (int i = 0; i < DolSpawnCount; i++)
            {
                NormalRoom normalRoom = normalRooms[0];

                Vector3 spawnPoint = normalRoom.DolSpawnPoint();
                if (spawnPoint != Vector3.zero)
                    PhotonNetwork.InstantiateRoomObject("Meokdori", spawnPoint, Quaternion.identity);

                // Debug.Log($" MuckDol {normalRoom.index}");
                normalRooms.RemoveAt(0);
            }

            for (int i = 0; i < GoldenDolSpawnCount; i++)
            {
                NormalRoom normalRoom = normalRooms[0];
                Vector3 spawnPoint = normalRoom.DolSpawnPoint();
                if (spawnPoint != Vector3.zero)
                    PhotonNetwork.InstantiateRoomObject("Golden Meokdori", spawnPoint, Quaternion.identity); //<- 나중에 프리팹 교체할것
                //Debug.Log($"golden MuckDol {normalRoom.index}");

                normalRooms.RemoveAt(0);
            }
        }

        //돌돌이 스폰 : https://docs.google.com/document/d/17QSV9CL-zrgxiPkNKf0UsOePuYK9sH0kB6dAFJBVjcc/edit
        //먹돌이와 동일한 알고리즘, 단 먹돌이가 스폰한 방은 제외하기 위해 먹돌이 스폰 후 호출
         void DolDolSpawn()
        {
            for (int i = 0; i < DolSpawnCount; i++)
            {
                NormalRoom normalRoom = normalRooms[0];
                //Debug.Log($"DolDori {normalRoom.index}");

                Vector3 spawnPoint = normalRoom.DolSpawnPoint();
                if (spawnPoint != Vector3.zero)
                    PhotonNetwork.InstantiateRoomObject("DolDori", spawnPoint, Quaternion.identity);

                normalRooms.RemoveAt(0);
            }
        }

        int merchentSpecies = 4;
        void MerchantSpawn()
        {
            int index = 0;
            for(int i = 0; i< merchentSpecies; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    object[] data = new object[1];
                    data[0] = index;
                    NormalRoom normalRoom = normalRooms[0];

                    Vector3 spawnPoint = normalRoom.DolSpawnPoint();
                    if (spawnPoint != Vector3.zero)
                        PhotonNetwork.InstantiateRoomObject("Merchant", spawnPoint, Quaternion.identity, 0, data);

                    normalRooms.RemoveAt(0);
                }
                index++;
            }
        }
    }
}
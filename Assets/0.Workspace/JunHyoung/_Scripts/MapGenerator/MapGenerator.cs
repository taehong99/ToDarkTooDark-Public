using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using System.Linq;

namespace MapGenerator
{
    public enum Direction
    {
        All,
        Up,
        Down,
        Left,
        Right,
    }

    public enum RoomType
    {
        Empty=0,
        Normal=1,
        Special=2,
        Spawn=3,
        Secret=4,
        Excalibur=5,
    }

    /// <summary>
    /// Random Map 생성 스크립트
    /// </summary>
    /// 


    /*Algorithm
     *  Step 1. 중앙(0,0)에 엑스칼리버 방 생성
     *  
     *  Step 2. 4 방향에  일반방 2개, 특수방 1개 순으로 생성 + 문 및 복도 연결
     *  Step 3. Step2의 특수방 기준으로 일반방 2개, 특수방 1개 4방향 추가 생성 + 문 및 복도 연결 (스폰방 지정)
     *  Step 4. Step3의 스폰방 기준으로 일반방 2개, 특수방 1개 4방향 추가 생성 + 문 및 복도 연결
     *  Step 5. Step4 에서 생성된 특수방의 방향 확인(중앙방 기준)
     *  Step 6. Step5 에서 확인된 특수방의 방향 기준으로 방 추가 생성, 여기서 중앙방에서 더 멀어지는 방향으로는 생성 X
     *          -> Step5,6 통합
     *          
     *  Step 7. 공동에 추가 방 생성, 인접한 방들을 순회하며, 확률에 따라 추가적으로 문과 복도 연결
     *          -> 인접여부 검사 + 완전 인접시 문으로 연결
     *                
     */

    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] int seed;

        [Space(15)]
        [Header("[Settings]")]
        [SerializeField, Tooltip("Default value 5, for small test 3")]
        int generationDepth = 5;
        [SerializeField] int genSpawnPointStep = 2;
        [Range(0, 100)]
        [SerializeField, Tooltip("추가적으로 생성해 연결해줄 방의 갯수")] int addtionalRoomCount = 20;

        [Space(10)]
        [SerializeField] int corridorWidth;
        [SerializeField] float roomDistanceOffset = 5.0f;
        WaitForSeconds wait;
        [SerializeField, Tooltip("방을 배치할 때 거리를 결정하는 방식을 결정합니다." +
            "\nFalse : 절대적인 방의 크기를 기준으로 계산하여 배치합니다." +
            "\nTrue : 방마다 결정되어 있는 문의 위치를 기준으로 계산하여 배치합니다.")]
        bool roomDistanceType = false;
        [Space(10)]
        //[SerializeField, Tooltip("각 Step 별 대기시간")] 
        float waitTime; //각 단계 확인용. 빌드시에는 0으로

        [Space(15)]
        [SerializeField] Transform parent;
        [SerializeField] Transform objects;

        [Space(10)]
        [Header("[Rooms Prefabs]")]
        [SerializeField] Room excaliburRoom;
        [SerializeField] Room spawnRoom;
        [Space(10)]
        [SerializeField] List<Room> normalRooms;
        [SerializeField] List<Room> specialRooms;

        [Header("[Object Prefabs]")]
        [SerializeField] GameObject playerPrefab;
        [SerializeField] Door horizontalDoor;
        [SerializeField] Door verticalDoor;


        const int normalRoomPoolSize = 200;
        const int specialRoomPoolSize = 50;

        /// <summary>
        /// Room Data
        /// </summary>
        List<SpawnRoom> spawnRooms = new List<SpawnRoom>(); // <- 나중에 랜덤하게 스폰포인트 8개중 4개를 지정해주기 위함


        Dictionary<Vector2, Room> map = new Dictionary<Vector2, Room>();
        Dictionary<Vector3, Door> doors = new Dictionary<Vector3, Door>(); // door 중복 검사용
        

        Stack<Room> normalRoomPool;
        Stack<Room> specialRoomPool;

        // 안태홍 추가
        public event System.Action FinishedGeneratingEvent;


        // 임준희 추가
        public List<List<int>> connectedRoomGraph = new List<List<int>>();

        public void GenerateMap()
        {
            wait = new WaitForSeconds(waitTime);
            ClearMap();

            // GetSeed();

            InitRoomPool();

            StartCoroutine(GenerateMapRoutine());
        }

        public void InitSeed()
        {
            // 시드 값 랜덤 생성 및 Photon 커스텀 프로퍼티로 설정은 로비에서 방 만들어졌을 때 미리 정해주자.
            // 굳이 인-게임에서는 할 필요 없을것같음. 대기시간 길어지기만 할듯
            // 여기서 Photon 커스텀 프로퍼티에서 받아오기만
            // 안태홍 추가
            if (PhotonNetwork.IsConnected)
                seed = PhotonNetwork.CurrentRoom.GetSeed();
            else
                seed = Random.Range(0, 10000);

            Debug.Log($"<color=#00FF00>Init Seed : {seed}</color>");
        }

        void InitRoomPool()
        {
            normalRoomPool = CreateRandomRoomPool<Room>(normalRooms, normalRoomPoolSize);
            specialRoomPool = CreateRandomRoomPool<Room>(specialRooms, specialRoomPoolSize);
        }

        IEnumerator GenerateMapRoutine()
        {

            float totalTime = Time.realtimeSinceStartup;
            // Step 1 : 엑스칼리버 방 생성
            Room excaliburRoom = GenerateExcaliburRoom(Vector2.zero);

            yield return wait;


            float startTime = Time.realtimeSinceStartup;
            // Step 2 ~ 6: 방 생성 및 연결
            GenerateRooms(excaliburRoom, Vector2.zero, 1);
            float endTime = Time.realtimeSinceStartup;

            Debug.Log($"[GenerateRooms] work use {endTime - startTime} sec");

            yield return wait;

            startTime = Time.realtimeSinceStartup;
            // Step 7 : 추가 방 생성 및 연결
            AddingMoreRoom();
            endTime = Time.realtimeSinceStartup;
            Debug.Log($"[Adding Rooms] work use {endTime - startTime} sec");

            yield return wait;

            startTime = Time.realtimeSinceStartup;
            //Step 8 : 뚫려있는 곳 벽으로 둘러 싸기 & 그림자 적용
            TileMapManager.Instance.CoverWithWall();
            endTime = Time.realtimeSinceStartup;
            Debug.Log($"[Cover With Wall] work use {endTime - startTime} sec");

            yield return wait;

            /*
            startTime = Time.realtimeSinceStartup;
            //Step 9 : 벽안에 생성된 오브젝트/타일 삭제
            TileMapManager.Instance.DestroyObjectInWall();
            endTime = Time.realtimeSinceStartup;
            Debug.Log($"[Destroy Object In Wall] work use {endTime - startTime} sec");

            yield return wait;*/

            // Step 10 : Player Spawn
            Tae.MathUtilities.ShuffleList(spawnRooms, seed);
            SpawnPlayer();
            endTime = Time.realtimeSinceStartup;

            Debug.Log($"<color=#FAD656>[Generate Map Finish! Seed:{seed})</color>]" +
                $"\nTotal Room Count:{map.Count}" +
                $"\nTotal Objects Count:{MapManager.Instance.ObjectDic.Count}" +
                $"\nTotal Door Count:{MapManager.Instance.doorDic.Count} \n" +
                $"Total Time : {endTime - totalTime} sec");

            // Notify generate complete
            SetMapManager();

            // 임준희 추가
            foreach (List<int> list in connectedRoomGraph)
                list.Sort();

            FinishedGeneratingEvent?.Invoke();
        }

        /**********************************************
        *             Generation  Steps
        ***********************************************/
        #region Steps ---------------------------------------
        /// <summary>
        /// [Step 1] 중앙(0,0)에 엑스칼리버 방 생성
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Room GenerateExcaliburRoom(Vector2 index)
        {
            Room newExcaliburRoom = Instantiate(excaliburRoom, Vector3.zero, Quaternion.identity, parent);

            newExcaliburRoom.DrawRoom();
            map.Add(index, newExcaliburRoom);

            // 임준희 추가 - 방 연결 여부 확인 용 리스트 크기 조절 (복도 제작 전 추가)
            connectedRoomGraph.Add(new List<int>(4));

            return newExcaliburRoom;
        }

        /// <summary>
        /// [Step 2 ~ 6] BaseRoom에서 부터 뻗어 나가 방을 추가적으로 생성하는 메서드, 이후 끝방에서 다시 재귀호출
        /// </summary>
        /// <param name="baseRoom"> 기준점이 되는 방 </param>
        /// <param name="pos"> map 배열에서 해당 방의 curRoomPos </param>
        /// <param name="depth"> 생성 단계 </param>
        void GenerateRooms(Room baseRoom, Vector2 pos, int depth)
        {
            Vector2[] directions = new Vector2[] { Vector3.zero };

            if (depth == generationDepth)
            {
                return;
            }


            // At Last Step
            if (depth == generationDepth - 1)
            {
                // Case Left
                if (pos[0] < 0 && pos.y == 0)
                {
                    directions = new Vector2[]{
                    Vector2.right,
                    Vector2.up,
                    Vector2.down
                    };
                } // Case Right
                else if (pos[0] > 0 && pos[1] == 0)
                {
                    directions = new Vector2[]{
                    Vector2.left,
                    Vector2.up,
                    Vector2.down
                    };
                } //Case Down
                else if (pos[0] == 0 && pos[1] < 0)
                {
                    directions = new Vector2[]{
                    Vector2.left,
                    Vector2.right,
                    Vector2.up,
                    };
                } //Case Up
                else if (pos[0] == 0 && pos[1] > 0)
                {
                    directions = new Vector2[]{
                    Vector2.left,
                    Vector2.right,
                    Vector2.down
                    };
                }
                else
                {
                    directions = new Vector2[]{
                    Vector2.left,
                    Vector2.right,
                    Vector2.up,
                    Vector2.down
                    };
                }
            }
            else
            {
                directions = new Vector2[]{
                Vector2.left,
                Vector2.right,
                Vector2.up,
                Vector2.down
                };
            }


            foreach (Vector2 direction in directions)
            {
                //Debug.Log($"{curRoomPos[0]},{curRoomPos[1]}");
                int nextStep = depth;
                Vector2 newIndex = pos + direction;

                Room normalRoom1 = CreateRoom(baseRoom, direction, SelectRandomRoom(normalRoomPool), newIndex);
                if (normalRoom1 == null) continue;

                newIndex += direction;

                Room normalRoom2 = CreateRoom(normalRoom1, direction, SelectRandomRoom(normalRoomPool), newIndex);
                if (normalRoom2 == null) continue;

                newIndex += direction;
                
                if (depth == genSpawnPointStep
                    || (newIndex == new Vector2(-3,-3)) || (newIndex == new Vector2(-3, 3))
                    || (newIndex == new Vector2(3, -3)) || (newIndex == new Vector2(3, 3)))
                {
                    Room newSpecialRoom = CreateRoom(normalRoom2, direction, spawnRoom, newIndex);
                    if (newSpecialRoom == null) continue;
                    nextStep++;
                    GenerateRooms(newSpecialRoom, newIndex, nextStep);
                }
                else
                {
                    //Room newSpecialRoom = CreateRoom(normalRoom2, direction, specialRoom, newIndex, step);
                    Room newSpecialRoom = CreateRoom(normalRoom2, direction, SelectRandomRoom(specialRoomPool), newIndex);
                    if (newSpecialRoom == null) continue;
                    nextStep++;
                    GenerateRooms(newSpecialRoom, newIndex, nextStep);
                }
            }
        }


        /// <summary>
        /// [Step 7]  추가적인 방 생성 및 연결
        /// </summary>
        void AddingMoreRoom()
        {
            //후보지 리스트, 생성 이전이기 때문에 인접한 두방 Index를 담아서 저장
            List<Vector2[]> candidatePos = new List<Vector2[]>();
            int cnt = 0;

            Vector2[] directions = new Vector2[]{
                    Vector2.up,
                    Vector2.down,
                    Vector2.left,
                    Vector2.right
                    };

            List<Vector2> allRoomPos = new List<Vector2>(map.Keys);

            foreach (Vector2 curRoomPos in allRoomPos)
            {
                cnt = 0;
                List<Vector3> adjacentPos = new List<Vector3>();

                foreach (Vector2 direction in directions)
                {
                    Vector2 neighborPos = curRoomPos + direction;
                    if (!map.ContainsKey(neighborPos))
                    {
                        foreach(Vector2 dir in directions)
                        {
                            if (map.ContainsKey(neighborPos + dir))
                            {
                                cnt++;
                                adjacentPos.Add(neighborPos + dir);   
                            }

                            if (cnt == 2)
                            {
                                Vector3 newPos = new Vector3(map[adjacentPos[0]].position.x, map[adjacentPos[1]].position.y);
                                candidatePos.Add(new Vector2[] { adjacentPos[0], adjacentPos[1] });
                            }
                        }
                        adjacentPos.Clear();
                        cnt = 0;
                    }
                }
                // 인접한 방이 2개일 때 후보지로 추가, 및 위치 지정
            }
            /* Legacy
            // map[][] 순회
            for (int i = 0; i < mapArraySize; i++)
            {
                // 빈곳이고 인접장소에 방이2개 -> 후보지로 선정해 List에 저장
                for (int j = 0; j < mapArraySize; j++)
                {
                    if (map.ContainsKey(new Vector2(i, j))) continue;

                    List<Vector3> adjacentPos = new List<Vector3>();

                    // 상
                    if (i > 0 && map[i - 1][j] != null)
                    {
                        cnt++;
                        adjacentPos.Add(map[i - 1][j].position);
                    }

                    // 하
                    if (i < mapArraySize - 1 && map[i + 1][j] != null)
                    {
                        cnt++;
                        adjacentPos.Add(map[i + 1][j].position);
                    }

                    // 좌
                    if (j > 0 && map[i][j - 1] != null)
                    {
                        cnt++;
                        adjacentPos.Add(map[i][j - 1].position);
                    }

                    // 우
                    if (j < mapArraySize - 1 && map[i][j + 1] != null)
                    {
                        cnt++;
                        adjacentPos.Add(map[i][j + 1].position);
                    }

                    // 인접한 방이 2개일 때 후보지로 추가, 및 위치 지정
                    if (cnt == 2)
                    {
                        Vector3 newPos = new Vector3(adjacentPos[1].x, adjacentPos[0].y);
                        // map[i][j] = Instantiate(normalRooms[0], newPos,Quaternion.identity, newParent.transform);
                        map[i][j] = new Room(); // 생성자 교체할것 // data만 생성 // 그냥 Instantiate 했다가 Destroy 방식 써보자
                                                //map[i][j] = gameObject.AddComponent<Room>();
                        map[i][j].position = newPos;
                        map[i][j].curRoomPos = new Vector2Int(i, j);
                        candidatePos.Add(map[i][j]);
                    }
                    cnt = 0;
                }
            }
            */


            //  Seed 기반으로 셔플 
            //Fisher-Yates Suffle
            Random.InitState(seed);
            for (int i = candidatePos.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var temp = candidatePos[i];
                candidatePos[i] = candidatePos[j];
                candidatePos[j] = temp;
            }
           // Debug.Log(candidatePos.Count); // -> 312 개 나옴

            if (addtionalRoomCount >= candidatePos.Count)
                addtionalRoomCount = candidatePos.Count - 1;

            List<Vector2> indexList = new List<Vector2>(); // 이후 candiateList 를 Instantiate 할 때 정보가 다 덮어씌워지는데, 그 전에 저장해 놓기 위함

            // -> 일정 비율 꺼내서 normal Room으로 변환, 생성
            for (int i = 0; i < addtionalRoomCount; i++)
            {
               
                Vector2 newIndex = new Vector2(candidatePos[i][0].x, candidatePos[i][1].y);

                if (!map.ContainsKey(newIndex))
                {
                    Vector3 newPos = new Vector3(map[candidatePos[i][0]].position.x, map[candidatePos[i][1]].position.y);
                    Room addtionalRoom = Instantiate(SelectRandomRoom(normalRoomPool), newPos, Quaternion.identity, parent);
                    addtionalRoom.DrawRoom();
                    addtionalRoom.index = newIndex;
                    addtionalRoom.name = $"addtionalRoom {i}";
                    map.Add(newIndex, addtionalRoom);
                    indexList.Add(newIndex);

                    // 임준희 추가 - 방 연결 여부 확인 용 리스트 크기 조절 (복도 제작 전 추가)
                    connectedRoomGraph.Add(new List<int>(4)); //for Mini Map
                }
            }

            /* Legacy Connect Room
            foreach (var curRoomPos in indexList)
            {
                map[curRoomPos.x][curRoomPos.y].roomType = RoomType.Normal;
            }
            for (int i = 0; i < addtionalRoomCount; i++)
            {
                if (indexList[i].x > 0 && map[indexList[i].x - 1][indexList[i].y] != null)
                {
                    GenerateCorridor(candidatePos[i], map[indexList[i].x - 1][indexList[i].y], Vector3.left);
                }

                if (indexList[i].x < mapArraySize - 1 && map[indexList[i].x + 1][indexList[i].y] != null)
                {
                    GenerateCorridor(candidatePos[i], map[indexList[i].x + 1][indexList[i].y], Vector3.right);
                }

                if (indexList[i].y > 0 && map[indexList[i].x][indexList[i].y - 1] != null)
                {
                    GenerateCorridor(candidatePos[i], map[indexList[i].x][indexList[i].y - 1], Vector3.down);
                }

                if (indexList[i].y < mapArraySize - 1 && map[indexList[i].x][indexList[i].y + 1] != null)
                {
                    GenerateCorridor(candidatePos[i], map[indexList[i].x][indexList[i].y + 1], Vector3.up);
                }

            }*/

            foreach(var index in indexList)
            {
                foreach(var direction in directions)
                {
                    Vector2 nextIndex = index + direction;
                    if (map.ContainsKey(nextIndex))
                    {
                        GenerateCorridor(map[index], map[nextIndex], direction);
                    }
                }
            }

            candidatePos.Clear();
        }



        public void SpawnPlayer()
        {
            // 안태홍 추가
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log($"Num Spawn rooms : {spawnRooms.Count}");
                Manager.Game.SpawnPlayer(spawnRooms[PhotonNetwork.LocalPlayer.ActorNumber - 1].position);
                Manager.Game.MyExit = spawnRooms[PhotonNetwork.LocalPlayer.ActorNumber + PhotonNetwork.CurrentRoom.PlayerCount - 1].exit;
                Debug.Log($"Actor {PhotonNetwork.LocalPlayer.ActorNumber} spawned in SpawnRoom[{PhotonNetwork.LocalPlayer.ActorNumber - 1}] " +
                    $"with exit in SpawnRoom[{PhotonNetwork.LocalPlayer.ActorNumber + PhotonNetwork.CurrentRoom.PlayerCount - 1}]");

                // 플레이어가 스폰된 방은 탈출구가 없음
                for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                {
                    spawnRooms[i].DisableExit();
                }
            }
            else // 디버그용
                return;
                //Instantiate(playerPrefab, spawnRooms[0].position, Quaternion.identity);
        }

        void SetMapManager()
        {
            MapManager.Instance.seed = seed;
            MapManager.Instance.Rooms = map;
        }

        #endregion



        /*************************************************
         *          Generate Room/Corridor/Door
         *************************************************/
        #region private Methods -----------------------------------
        /// <summary>
        /// RoomPool에서 방 하나를 반환하는 메서드
        /// </summary>
        /// <param name="roomList"></param>
        /// <returns></returns>
        T SelectRandomRoom<T>(Stack<T> roomStack) where T : Room
        {
            if (roomStack.Count == 0)
            {
                Debug.Log("RoomPool is Empty");
                return null;
            }

            return roomStack.Pop();
        }

        /// <summary>
        /// RoomPool 을 생성한 후 Seed 값을 기반으로 랜덤하게 섞는 메서드, 일단은 각 방끼리 동일한 비중으로 들어감
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="roomList"></param>
        /// <returns></returns>
        Stack<T> CreateRandomRoomPool<T>(List<T> roomList, int poolSize) where T : Room
        {
            // int addCount = poolSize / roomList.Count;

            List<T> extendedRoomList = new List<T>();
            while (extendedRoomList.Count < poolSize)
            {
                extendedRoomList.AddRange(roomList);
            }

            extendedRoomList = extendedRoomList.GetRange(0, poolSize);

            //Fisher-Yates Suffle
            Random.InitState(seed);
            for (int i = extendedRoomList.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = extendedRoomList[i];
                extendedRoomList[i] = extendedRoomList[j];
                extendedRoomList[j] = temp;
            }

            Stack<T> roomStack = new Stack<T>();
            foreach (var room in extendedRoomList)
            {
                roomStack.Push(room);
            }

            return roomStack;
        }



        Room CreateRoom(Room baseRoom, Vector3 direction, Room roomPrefab, Vector2 index)
        {
            Vector3 offset;
            Vector3 newPos;

            if (direction.x == 0)
            {
                if (roomDistanceType)
                {
                    offset = direction * roomDistanceOffset + roomPrefab.GetDoorPosition(direction, false);
                    newPos = baseRoom.GetDoorPosition(direction) + offset;
                }
                else
                {
                    offset = direction * (baseRoom.roomSize.y / 2 + roomPrefab.roomSize.y / 2 + roomDistanceOffset);
                    newPos = baseRoom.transform.position + offset;
                }
            }
            else
            {
                if (roomDistanceType)
                {
                    offset = direction * roomDistanceOffset + roomPrefab.GetDoorPosition(direction, false);
                    newPos = baseRoom.GetDoorPosition(direction) + offset;
                }
                else
                {
                    offset = direction * (baseRoom.roomSize.x / 2 + roomPrefab.roomSize.x / 2 + roomDistanceOffset);
                    newPos = baseRoom.transform.position + offset;
                }
            }
            Vector3Int intPos = new Vector3Int((int) newPos.x, (int) newPos.y, 0);

            // Map Dictionary 검사
            // 해당 방향에 방이 없으면 생성, 없으면 return;
            if (!map.ContainsKey(index))
            {
                Room newRoom = Instantiate(roomPrefab, intPos, Quaternion.identity, parent);

                newRoom.DrawRoom();
                newRoom.index = index;

                if (roomPrefab.roomType == RoomType.Spawn)
                {
                    spawnRooms.Add((SpawnRoom) newRoom);
                }

                // Map Dictionary 배열 체크
                map.Add(index, newRoom);

                // 임준희 추가 - 방 연결 여부 확인 용 리스트 크기 조절 (복도 제작 전 추가)
                connectedRoomGraph.Add(new List<int>(4));

                GenerateCorridor(baseRoom, newRoom, direction);
                return newRoom;
            }
            else
            {
                //Debug.Log("Aleady Room Exist");
                GenerateCorridor(baseRoom, map[index], direction);
                return null;
            }
        }

        void GenerateCorridor(Room baseRoom, Room newRoom, Vector3 direction)
        {
            // 문위치가 고정되어있다면, Room에서 문위치 Get 해서 지정

            Vector3 startPos;
            Vector3 endPos;

            startPos = baseRoom.GetDoorPosition(direction);
            endPos = newRoom.GetDoorPosition(direction, false);

            GenerateDoor(startPos, direction);
            GenerateDoor(endPos, direction);

            // 임준희 추가

            //Debug.Log($" Connect Room {map.Values.ToList().IndexOf(baseRoom)} {map.Values.ToList().IndexOf(newRoom)}");

            int baseRoomIndex = map.Values.ToList().IndexOf(baseRoom);
            int newRoomIndex = map.Values.ToList().IndexOf(newRoom);
            if (connectedRoomGraph[baseRoomIndex].Contains(newRoomIndex) == false)
            {
                connectedRoomGraph[map.Values.ToList().IndexOf(baseRoom)].Add(map.Values.ToList().IndexOf(newRoom));
                connectedRoomGraph[map.Values.ToList().IndexOf(newRoom)].Add(map.Values.ToList().IndexOf(baseRoom));
            }

            //overlapOffset 
            //  보정해 수직 수평 복도 생성, 내부 공식 수정할 것.
            // offset ovelap 검사 -> 차이가 너무 큰 경우 ㄹ 복도 생성(Vertical + Horizontal + Vertical)

            if (startPos.x == endPos.x || startPos.y == endPos.y)
            {
                startPos = new Vector3(Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y));
                endPos = new Vector3(Mathf.RoundToInt(endPos.x), Mathf.RoundToInt(endPos.y));

                TileMapManager.Instance.DrawCorridor(startPos, endPos, corridorWidth);
                return;
            }

            bool isHorizontalOverlap = startPos.x < endPos.x || (startPos.x > endPos.y);
            bool isVerticalOverlap = (startPos.y < endPos.y) || (startPos.y > endPos.y);

            if (isVerticalOverlap && !isHorizontalOverlap) // equal direction.x = 1 || -1
            {
                // x 보정
                float offsetX = (startPos.x + endPos.x) * 0.5f;

                TileMapManager.Instance.DrawCorridor(new Vector3(offsetX, startPos.y), new Vector3(offsetX, endPos.y), corridorWidth);
                TileMapManager.Instance.DrawCorridor(new Vector3(endPos.x, endPos.y), new Vector3(offsetX, endPos.y), corridorWidth);
                TileMapManager.Instance.DrawCorridor(new Vector3(offsetX, startPos.y), new Vector3(startPos.x, startPos.y), corridorWidth);

                //Debug.Log("isVertical  Overlap");
            }
            else if (isHorizontalOverlap)
            {

                // y 보정 
                float offsetY = (startPos.y + endPos.y) * 0.5f;

                TileMapManager.Instance.DrawCorridor(new Vector3(startPos.x, offsetY), new Vector3(endPos.x, offsetY), corridorWidth);
                TileMapManager.Instance.DrawCorridor(new Vector3(endPos.x, offsetY), new Vector3(endPos.x, endPos.y), corridorWidth);
                TileMapManager.Instance.DrawCorridor(new Vector3(startPos.x, offsetY), new Vector3(startPos.x, startPos.y), corridorWidth);

                //Debug.Log("isHorizontal  Overlap");
            }
            return;
        }


        void GenerateDoor(Vector3 pos, Vector3 direction)
        {
            pos = pos + TileMapManager.Instance.FloorMap.tileAnchor;

            // 중복 방지
            if (doors.ContainsKey(pos))
                return;

            if (direction == Vector3.left || direction == Vector3.right)
            {
                Door newDoor = Instantiate(verticalDoor, pos, Quaternion.identity, objects);
                newDoor.SetVertical();
                doors.Add(pos, newDoor);

                // 안태홍 추가
                newDoor.id = MapManager.Instance.doorID++;
                MapManager.Instance.doorDic.Add(newDoor.id, newDoor);
                MapManager.Instance.ObjectDic.TryAdd(pos, newDoor.gameObject);
            }
            else
            {
                Door newDoor = Instantiate(horizontalDoor, pos, Quaternion.identity, objects);
                doors.Add(pos, newDoor);

                // 안태홍 추가
                newDoor.id = MapManager.Instance.doorID++;
                MapManager.Instance.doorDic.Add(newDoor.id, newDoor);
                MapManager.Instance.ObjectDic.TryAdd(pos, newDoor.gameObject);
            }
        }
        #endregion

        #region ContextMenu---------------------------------------------

        [ContextMenu("ClearMap")]
        public void ClearMap()
        {

            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in objects)
            {
                Destroy(child.gameObject);
            }
            doors.Clear();
            map.Clear();

            if (normalRoomPool != null)
                normalRoomPool.Clear();

            if (specialRoomPool != null)
                specialRoomPool.Clear();

            TileMapManager.Instance.ClearAll();
        }
        #endregion
    }
}

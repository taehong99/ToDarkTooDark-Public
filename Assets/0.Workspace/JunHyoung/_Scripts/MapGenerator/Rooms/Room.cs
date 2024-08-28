using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Photon.Pun;

namespace MapGenerator
{
    /// <summary>
    /// Script For Describe Room's feature
    /// </summary>
    public class Room : MonoBehaviour
    {
        public RoomType roomType;

        [Space(10)]

        [SerializeField, Tooltip("Position in world \n (*NOT IN TILEMAP, HAVE TO CONVERT IT)")]
        public Vector3 position;
        [SerializeField, Tooltip("Size of Room")]
        public Vector2Int roomSize;
        [SerializeField] public Vector2 index; // index of map[][](in MapGenerator)

        [Space(10)]
        [SerializeField] public Tilemap floorTilemap;
        [SerializeField] Tilemap wallTilemap;
        [SerializeField] Tilemap doorTilemap;
        [SerializeField] Tilemap objectsTilemap;
        
        Vector3[]  doorPos = new Vector3[4];

        // Monster Spawning
        [SerializeField] protected MonsterSpawner spawner;
        private LayerMask playerMask;
        private bool hasSpawned;
        private System.Action allMonstersDied;

        #region Monster Spawning (Tae)
        private void Awake()
        {
            playerMask = 1 << LayerMask.NameToLayer("Player");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!PhotonNetwork.IsMasterClient || hasSpawned)
                return;

            // If player enters room, spawn monsters
            if (playerMask.Contain(collision.gameObject.layer))
            {
                Debug.Log("Collided with player");
                //spawner.SpawnMonsters();
                SpawnMonsters();
            }
        }

        private void SpawnMonsters()
        {
            if (!PhotonNetwork.IsMasterClient || hasSpawned || spawner == null)
                return;

            spawner.SpawnMonsters();
            hasSpawned = true;
        }
        #endregion

        public virtual void DrawRoom()
        {
            // 방마다 override 해서 알아서 그리십쇼
            CalculateRoomSize();
            position = transform.localPosition;


            CalcDoorPosition();
            TileMapManager.Instance.TilemapMigration(floorTilemap, position, TilemapType.floor) ;
            TileMapManager.Instance.TilemapMigration(wallTilemap, position, TilemapType.wall);
            TileMapManager.Instance.TilemapMigration(objectsTilemap, position, TilemapType.obj);
            RegisterRoomObjects();

            floorTilemap.gameObject.SetActive(false);
            doorTilemap.gameObject.SetActive(false);
            wallTilemap.GetComponent<TilemapRenderer>().enabled = false;
            objectsTilemap.GetComponent<TilemapRenderer>().enabled = false; // GetComponet 말고 바꿀것
        }


        /// <summary>
        /// 연결 방향에 따라 적합한 문 위치를 반환
        /// </summary>
        /// <param name="direction">연결 방향</param>
        /// <param name="isStart"> 출발지점인지, 도착지점인지 </param>
        /// <returns></returns>


        public Vector3 GetDoorPosition(Vector3 direction , bool isStart = true)
        {
            Vector3 position = Vector3.zero;

            if (doorPos == null) { return position; }

            if (isStart)
            {
                if (direction == Vector3.up)
                {
                    position = doorPos.OrderByDescending(v => v.y).First();
                }
                else if (direction == Vector3.down)
                {
                    position = doorPos.OrderBy(v => v.y).First();
                }
                else if (direction == Vector3.left)
                {
                    position = doorPos.OrderBy(v => v.x).First();
                }
                else if (direction == Vector3.right)
                {
                    position = doorPos.OrderByDescending(v => v.x).First();
                }
            }
            else
            {
                if (direction == Vector3.up)
                {
                    position = doorPos.OrderBy(v => v.y).First();
                }
                else if (direction == Vector3.down)
                {
                    position = doorPos.OrderByDescending(v => v.y).First();
                }
                else if (direction == Vector3.left)
                {
                    position = doorPos.OrderByDescending(v => v.x).First();
                }
                else if (direction == Vector3.right)
                {
                    position = doorPos.OrderBy(v => v.x).First();
                }
            }
            //position = floorTilemap.WorldToCell(position);
            return position;
        }
        
        private void CalcDoorPosition()
        {
            int cnt = 0;

            foreach (var item in doorTilemap.cellBounds.allPositionsWithin)
            {
                if (doorTilemap.HasTile(item))
                {
                    Vector3 worldPos = doorTilemap.LocalToWorld(item);
                    doorPos[cnt] = worldPos;
                    cnt++;
                }
            }

        }

        /// <summary>
        /// 리스폰(안함) 일단은 Dictonary에 위치를 Key로 추가(벽에 끼인 오브젝트 삭제를 위함), 추가하다 해당위치에 다른 오브젝트 있으면 삭제함
        /// </summary>
        void RegisterRoomObjects()
        {
            foreach(Transform child in objectsTilemap.transform)
            {
                //child.gameObject.transform.parent = TileMapManager.Instance.ObjectMap.transform;
                foreach (Transform obj in child)
                {

                    if (obj.gameObject.layer == LayerMask.NameToLayer("Door"))
                    {
                        int id = MapManager.Instance.doorID++;
                        MapManager.Instance.doorDic.Add(id, obj.GetComponent<Door>());
                    }

                    if (MapManager.Instance.ObjectDic.TryAdd(obj.transform.position, obj.gameObject))
                    {
                        continue;
                    }
                    else
                    {
                        Destroy(obj.gameObject);
                    }
                }
            }
        }



        protected Vector3 GetRandomTilemapPos(Tilemap tilemap)
        {
            List<Vector3> validPos = new List<Vector3>();

            foreach (var item in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(item))
                {
                    validPos.Add(tilemap.LocalToWorld(item) + tilemap.tileAnchor);
                }
            }

            if (validPos.Count == 0)
                return Vector3.zero;

            int randomIndex = Random.Range(0, validPos.Count);
            Vector3 selectedPos = validPos[randomIndex];

            return selectedPos;
        }
        /*************************************************
         *                  Draw Gizmos
         *************************************************/

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            DrawFloorGizmo();
            DrawWallGizmo();
        }

        void DrawFloorGizmo()
        {
            if (floorTilemap == null)
                return;

            // tilemap position
            var tp = floorTilemap.transform.position;

            // bounds + offset
            var tBounds = floorTilemap.cellBounds;

            // corner points
            var c0 = new Vector3(tBounds.min.x, tBounds.min.y) + tp;
            var c1 = new Vector3(tBounds.min.x, tBounds.max.y) + tp;
            var c2 = new Vector3(tBounds.max.x, tBounds.max.y) + tp;
            var c3 = new Vector3(tBounds.max.x, tBounds.min.y) + tp;

            // draw borders
            Debug.DrawLine(c0, c1, Color.red);
            Debug.DrawLine(c1, c2, Color.red);
            Debug.DrawLine(c2, c3, Color.red);
            Debug.DrawLine(c3, c0, Color.red);

            // draw origin cross
            Debug.DrawLine(new Vector3(tp.x, tBounds.min.y + tp.y), new Vector3(tp.x, tBounds.max.y + tp.y), Color.green);
            Debug.DrawLine(new Vector3(tBounds.min.x + tp.x, tp.y), new Vector3(tBounds.max.x + tp.x, tp.y), Color.green);
        }

        void DrawWallGizmo()
        {
            if (wallTilemap == null)
                return;

            // tilemap position
            var tp = wallTilemap.transform.position;

            // bounds + offset
            var tBounds = wallTilemap.cellBounds;

            // corner points
            var c0 = new Vector3(tBounds.min.x, tBounds.min.y) + tp;
            var c1 = new Vector3(tBounds.min.x, tBounds.max.y) + tp;
            var c2 = new Vector3(tBounds.max.x, tBounds.max.y) + tp;
            var c3 = new Vector3(tBounds.max.x, tBounds.min.y) + tp;

            // draw borders
            Debug.DrawLine(c0, c1, Color.blue);
            Debug.DrawLine(c1, c2, Color.blue);
            Debug.DrawLine(c2, c3, Color.blue);
            Debug.DrawLine(c3, c0, Color.blue);

            // draw origin cross
            Debug.DrawLine(new Vector3(tp.x, tBounds.min.y + tp.y), new Vector3(tp.x, tBounds.max.y + tp.y), Color.cyan);
            Debug.DrawLine(new Vector3(tBounds.min.x + tp.x, tp.y), new Vector3(tBounds.max.x + tp.x, tp.y), Color.cyan);
        }

#endif
        /*************************************************
         *          Context Menu for Debug
         *************************************************/

        [ContextMenu("Cell Bounds")]
        public void DebuggingCellbounds()
        {
            Debug.Log($"CellBound {floorTilemap.cellBounds} , Local Bound {floorTilemap.localBounds} ");
            Debug.Log($"{floorTilemap.cellBounds.center}");

            foreach(var item in floorTilemap.cellBounds.allPositionsWithin)
            {
                Debug.Log(item);
            }
        }

        [ContextMenu("Debugging Objects Pos")]
        public void DebuggingCellboundsObjects()
        {
            Debug.Log($"CellBound {doorTilemap.cellBounds} , Local Bound {doorTilemap.localBounds} ");

            foreach (var item in doorTilemap.cellBounds.allPositionsWithin)
            {
                if (doorTilemap.HasTile(item))
                {
                    Vector3 worldPos = doorTilemap.LocalToWorld(item);
                    Debug.Log($"local Pos {item}, world Pos {worldPos}");
                }
            }
        }

        [ContextMenu("Calculate RoomSize")]
        public void CalculateRoomSize()
        {
            if (wallTilemap == null) return;

            wallTilemap.CompressBounds(); // 이거 안해주면 한칸씩 여백 포함해서 들어감
            BoundsInt bounds = wallTilemap.cellBounds;
            roomSize = new Vector2Int(bounds.size.x, bounds.size.y);
        }

        [ContextMenu("Fix Pivot")]
        public virtual void CenterTilemapAndObjects()
        {
            // 타일맵의 Bounds 가져오기
            BoundsInt bounds = wallTilemap.cellBounds;
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            // 타일맵의 중앙 계산
            Vector3 center = (min + max) * 0.5f;

            // 현재 타일맵의 중앙을 원점으로 이동시키기 위해 필요 이동량 계산
            Vector3 offset = wallTilemap.CellToWorld(Vector3Int.FloorToInt(center));

            // 타일맵 이동
            wallTilemap.transform.position -= offset;
            floorTilemap.transform.position -= offset;
            doorTilemap.transform.position -= offset;
            objectsTilemap.transform.position -= offset;

            // 오브젝트 이동
            foreach (Transform child in objectsTilemap.transform)
            {
                child.position -= offset;
            }
        }
    }
}
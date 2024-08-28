using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace MapGenerator
{
    public enum TilemapType
    {
        floor = 0,
        wall = 1,
        obj =2
    }

    /// <summary>
    /// TileMap 그려주는 매니저
    /// 벽과 바닥, 문 등을 각각 올바른 TilemapGrid에 찍어주는게 목적 - 문은 오브젝트라 따로 둬야 할듯?
    /// </summary>
    public class TileMapManager : MonoBehaviour
    {
        private static TileMapManager instance;
        public static TileMapManager Instance { get { return instance; } }

        [SerializeField]  Tilemap floorMap;
        public Tilemap FloorMap { get { return floorMap; } }
        [SerializeField] Tilemap wallMap;
        public Tilemap WallMap { get { return wallMap; } }

        [SerializeField] Tilemap objectMap;
        public Tilemap ObjectMap { get { return objectMap; } }

        [Space(10)]
        [SerializeField] TileBase wallTile;
        [SerializeField] TileBase corridorTile;

        private void Awake()
        {
            CreateInstance();
        }

        void CreateInstance()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
            }
        }

        private void Update()
        {
            DebuggingTilePos();
        }

        void DebuggingTilePos()
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int gridPos = floorMap.WorldToCell(mousePos);

                //Debug.Log(gridPos);
            }
        }

        public void TilemapMigration(Tilemap map, Vector3 position, TilemapType type)
        {
            Tilemap target;
            switch (type)
            {
                case TilemapType.floor:
                    target = floorMap;
                    map.CompressBounds();
                    break;
                case TilemapType.wall: 
                    target = wallMap;
                    map.CompressBounds();
                    break;
                case TilemapType.obj:
                    target = objectMap;
                    break;
                default:
                    target = floorMap;
                    break;
            }

            BoundsInt bounds = map.cellBounds;
            Vector3Int positionOffset = new Vector3Int((int) position.x, (int) position.y);

            for (int x = bounds.position.x; x < bounds.size.x; x++)
            {
                for (int y = bounds.position.y; y < bounds.size.y; y++)
                {
                    for (int z = 0; z < bounds.size.z; z++)
                    {
                        TileBase tile = map.GetTile(new Vector3Int(x, y, z));
                        if (tile != null)
                        {
                            // 타일을 타겟 Tilemap에 복사
                            Vector3Int targetPosition = new Vector3Int(x, y, z) + positionOffset;
                            target.SetTile(targetPosition, tile);
                        }
                    }
                }
            }

        }

        //Legacy TilemapMigration
        /*
        public void TilemapMigrationFloor(Tilemap floorTilemap, Vector3 position)
        {
            floorTilemap.CompressBounds();
            BoundsInt bounds = floorTilemap.cellBounds;
            Vector3Int positionOffset = new Vector3Int((int) position.x, (int) position.y);

            for (int x = bounds.position.x; x < bounds.size.x; x++)
            {
                for (int y = bounds.position.y; y < bounds.size.y; y++)
                {
                    for (int z = 0; z < bounds.size.z; z++)
                    {
                        TileBase tile = floorTilemap.GetTile(new Vector3Int(x, y, z));
                        if (tile != null)
                        {
                            // 타일을 타겟 Tilemap에 복사
                            Vector3Int targetPosition = new Vector3Int(x, y, z) + positionOffset;
                            floorMap.SetTile(targetPosition, tile);
                        }
                    }
                }
            }
        }

        public void TilemapMigrationWall(Tilemap wallTilemap, Vector3 position)
        {
            wallTilemap.CompressBounds();
            BoundsInt bounds = wallTilemap.cellBounds;
            Vector3Int positionOffset = new Vector3Int((int) position.x, (int) position.y);

            for (int x = bounds.position.x; x < bounds.size.x; x++)
            {
                for (int y = bounds.position.y; y < bounds.size.y; y++)
                {
                    for (int z = 0; z < bounds.size.z; z++)
                    {
                        TileBase tile = wallTilemap.GetTile(new Vector3Int(x, y, z));
                        if (tile != null)
                        {
                            // 타일을 타겟 Tilemap에 복사
                            Vector3Int targetPosition = new Vector3Int(x, y, z)+ positionOffset;
                            wallMap.SetTile(targetPosition, tile);
                        }
                    }
                }
            }
        }

     
        public void TilemapMigrationObjects(Tilemap objectTileMap, Vector3 position)
        {
            //objectTileMap.CompressBounds();
            BoundsInt bounds = objectTileMap.cellBounds;
            TileBase[] allTiles = objectTileMap.GetTilesBlock(bounds);
            Vector3Int positionOffset = new Vector3Int((int) (position.x), (int) (position.y), 0);


            for (int x = bounds.position.x; x < bounds.size.x; x++)
            {
                for (int y = bounds.position.y; y < bounds.size.y; y++)
                {
                    for (int z = 0; z < bounds.size.z; z++)
                    {
                        TileBase tile = objectTileMap.GetTile(new Vector3Int(x, y, z));
                        if (tile != null)
                        {
                            // 타일을 타겟 Tilemap에 복사
                            Vector3Int targetPosition = new Vector3Int(x, y, z) + positionOffset;
                            objectMap.SetTile(targetPosition, tile);
                        }
                    }
                }
            }
        }
        */

        /// <summary>
        /// 맵 생성 이후, 벽안에 끼어서 생성된 오브젝트/타일들을 삭제해주는 메서드
        /// </summary>
        private void DestroyObjectInWall()
        {
            foreach (var item in wallMap.cellBounds.allPositionsWithin)
            {
                if (wallMap.HasTile(item))
                {
                    //Tilemap에 있는 오브젝트 삭제
                    objectMap.SetTile(item, null);

                    // WorldPos에 있는 오브젝트 삭제
                    Vector3 worldPos = item + objectMap.tileAnchor;
                    if (MapManager.Instance.ObjectDic.ContainsKey(worldPos))
                    {
                        //Debug.Log($"중복 오브젝트 {objectDic[worldPos].name} 삭제");
                        Destroy(MapManager.Instance.ObjectDic[worldPos]);
                    }
                }
            }
        }


        /// <summary>
        /// 복도 바닥을 그려주는 메서드
        /// </summary>
        /// <param name="startPos">시작 지점(=시작 지점 문 위치)</param>
        /// <param name="endPos">도착 지점(=도착 지점 문 위치)</param>
        /// <param name="corridorWidth">복도 두께</param>
        public void DrawCorridor(Vector3 startPos, Vector3 endPos , int corridorWidth =1)
        {
            Vector3Int startCellPos = floorMap.WorldToCell(startPos);
            Vector3Int endCellPos = floorMap.WorldToCell(endPos);
            

            // case Horizontal
            if (startCellPos.y == endCellPos.y)
            {
                if (startCellPos.x > endCellPos.x)
                {
                    var temp = startCellPos;
                    startCellPos = endCellPos;
                    endCellPos = temp;
                }

                for (int x = startCellPos.x; x <= endCellPos.x + 1; x++)
                {
                    for (int offset = -corridorWidth; offset <= corridorWidth; offset++)
                    {
                        floorMap.SetTile(new Vector3Int(x, startCellPos.y + offset, 0), corridorTile);
                    }
                }
            } // case Vertical
            else if (startCellPos.x == endCellPos.x)
            {
                if (startCellPos.y > endCellPos.y)
                {
                    var temp = startCellPos;
                    startCellPos = endCellPos;
                    endCellPos = temp;
                }


                for (int y = startCellPos.y; y <= endCellPos.y; y++)
                {
                    for (int offset = -corridorWidth; offset <= corridorWidth; offset++)
                    {
                        floorMap.SetTile(new Vector3Int(startCellPos.x + offset, y, 0), corridorTile);
                    }
                }
            }
        }


        /// <summary>
        /// 모든 단계가 끝난 후, FloorMap을 기반으로 WallMap에 벽을 감싸 그려주는 메서드 
        /// </summary>
        public void CoverWithWall()
        {
            BoundsInt bounds = wallMap.cellBounds;

            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
            Queue<Vector3Int> queue = new Queue<Vector3Int>();

            // Initialize the queue with floorMap
            for (int x = bounds.position.x; x <= bounds.size.x; x++)
            {
                for (int y = bounds.position.y; y <= bounds.size.y; y++)
                {
                    for (int z = 0; z < bounds.size.z; z++)
                    {
                        Vector3Int position = new Vector3Int(x, y, z);
                        if (floorMap.HasTile(position) && !visited.Contains(position))
                        {
                            queue.Enqueue(position);
                            visited.Add(position);
                        }
                    }
                }
            }

            // Directions
            Vector3Int[] directions = new Vector3Int[]
            {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            };

            // BFS to find the boundary
            while (queue.Count > 0)
            {
                Vector3Int current = queue.Dequeue();

                foreach (var dir in directions)
                {
                    Vector3Int neighbor = current + dir;

                    if (!bounds.Contains(neighbor) || visited.Contains(neighbor))
                     {
                        continue;
                    }

                    if (floorMap.HasTile(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                    else
                    {
                        if(!wallMap.HasTile(neighbor))
                        {
                            wallMap.SetTile(neighbor, wallTile);
                        }
                    }
                }
            }

            DestroyObjectInWall();
            GenerateShadowCaster();
        }

        /// <summary>
        /// ShadowCaster 생성하는 메서드 ,맵 다 그려진 후 호출
        /// </summary>
        public void GenerateShadowCaster()
        {
            AutoShadowCasterTilemap autoCaster = wallMap.GetComponent<AutoShadowCasterTilemap>();
            if (autoCaster != null)
                autoCaster.Generate();
        }


        public void ClearAll()
        {
            floorMap.ClearAllTiles();
            wallMap.ClearAllTiles();
            objectMap.ClearAllTiles();

            MapManager.Instance.ObjectDic.Clear();
        }

        // GameObject를 Tilemap 의 Position으로 보정해주는 메서드
        public void SetObjectToTilemapPos(GameObject gameObject)
        {
            Vector3 worldPosition = gameObject.transform.position;

            Vector3Int cellPosition = floorMap.WorldToCell(worldPosition);

            Vector3 newWorldPosition = floorMap.CellToWorld(cellPosition);

            //Debug.Log($"worldPos:{worldPosition}, cellPos{cellPosition},newWorldPos{newWorldPosition}");
            gameObject.transform.position = cellPosition;
        }

        /*************************************************
        *                  Legacy Methods
        *************************************************/
        #region Legacy--------------------------------------
        // 방 위치와 Size를 입력 받아서, 상하좌우 4방향으로 벽 생성
        //Room Size가 홀수 * 홀수 여야 깔끔하게 벽이 생성 됨 (...)
        public void DrawWall(TileBase tile, Vector2 size, Vector3 pos)
        {
            Vector3Int centerCellPos = wallMap.WorldToCell(pos);

            for (int x = 0; x < size.x + 1; x++)
            {
                Vector3Int currentCellPos = new Vector3Int(centerCellPos.x - (int) (size.x / 2) + x - 1, centerCellPos.y - (int) (size.y / 2) - 1, centerCellPos.z);
                wallMap.SetTile(currentCellPos, tile);
            }

            for (int x = 0; x < size.x + 1; x++)
            {
                Vector3Int currentCellPos = new Vector3Int(centerCellPos.x - (int) (size.x / 2) + x - 1, centerCellPos.y + (int) (size.y / 2), centerCellPos.z);
                wallMap.SetTile(currentCellPos, tile);
            }

            for (int y = 0; y < size.y + 1; y++)
            {
                Vector3Int currentCellPos = new Vector3Int(centerCellPos.x - (int) (size.x / 2) - 1, centerCellPos.y - (int) (size.y / 2) - 1 + y, centerCellPos.z);
                wallMap.SetTile(currentCellPos, tile);
            }

            for (int y = 0; y < size.y + 1; y++)
            {
                Vector3Int currentCellPos = new Vector3Int(centerCellPos.x + (int) (size.x / 2) + 1, centerCellPos.y - (int) (size.y / 2) - 1 + y, centerCellPos.z);
                wallMap.SetTile(currentCellPos, tile);
            }
        }

        public void DrawFloor(TileBase tile, Vector2 size, Vector3 pos)
        {
            //Debug.Log(pos);
            // pos 을 floorMap.LocalToCell 을 통해 Vector3Int 인 cell postiton 으로 변환, 
            Vector3Int centerCellPos = floorMap.WorldToCell(pos);
            //Debug.Log(centerCellPos);

            // size 만큼 반복하여 타일을 배치
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector3Int currentCellPos = new Vector3Int(centerCellPos.x - (int) (size.x / 2) + x, centerCellPos.y - (int) (size.y / 2) + y, centerCellPos.z);
                    floorMap.SetTile(currentCellPos, tile);
                }
            }
        }
        #endregion
    }
}
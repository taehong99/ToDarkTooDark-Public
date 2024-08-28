using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Tilemaps;
using ItemLootSystem;

namespace MapGenerator
{
    [AddComponentMenu("Dungeon Rooms/NormalRoom")]
    public class NormalRoom :Room
    {
        [Space(10)]
        [SerializeField] Tilemap doldolSpawnPoints;

        //일반방 보물상자 스폰용 https://docs.google.com/spreadsheets/d/17T-mBoPHtTC-N7l_Yo0VatIaB-623-_CaHS1GlmLQ_w/edit?gid=417214557#gid=417214557 
        //방에서는 스폰위치만 가지고 있고, MapManger에서 일괄적으로 모든 일반방 스폰포인트 읽어들인 후 스폰
        [Space(10),SerializeField] public List<Transform> TreasureSpawnPoints; //고정(확정스폰)
        [SerializeField] Tilemap treasureSpawnPoints; //랜덤(밑에 cnt수 만큼만)
        [SerializeField] int treasureSpawnCnt;

        public override void DrawRoom()
        {
            base.DrawRoom();

            if (doldolSpawnPoints == null)
                return;
            doldolSpawnPoints.GetComponent<TilemapRenderer>().enabled = false;
            if (treasureSpawnPoints == null)
                return;
            treasureSpawnPoints.GetComponent<TilemapRenderer>().enabled = false;
        }

        //임시 , 나중에 Photon Instantiate로 교체할것.
        public void SpawnDol(GameObject dol)
        {
            // 돌돌이 or 먹돌이 스폰 위치 지정 
            Vector3 spawnPoint = GetRandomTilemapPos(doldolSpawnPoints);

            // 돌돌이 or 먹돌이 스폰
            Instantiate(dol, spawnPoint, Quaternion.identity , gameObject.transform);
        }

        public Vector3 DolSpawnPoint()
        {
            if(doldolSpawnPoints == null)
                return Vector3.zero;
            return GetRandomTilemapPos(doldolSpawnPoints);
        }

        public List<Vector3> GetTreasureSpawnPositions()
        {
            if (treasureSpawnPoints == null)
                return null;

            List<Vector3> selectedPositions = new List<Vector3>();
            HashSet<Vector3> uniquePositions = new HashSet<Vector3>(); //중복 방지용 HashSet

            while (selectedPositions.Count < treasureSpawnCnt && uniquePositions.Count < treasureSpawnPoints.cellBounds.size.x * treasureSpawnPoints.cellBounds.size.y)
            {
                Vector3 pos = GetRandomTilemapPos(treasureSpawnPoints);
                if (uniquePositions.Add(pos))
                {
                    selectedPositions.Add(pos);
                }
            }

            return selectedPositions;
        }
    }
}
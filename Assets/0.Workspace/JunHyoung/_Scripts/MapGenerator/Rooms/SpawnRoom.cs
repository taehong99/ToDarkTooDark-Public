using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tae;

namespace MapGenerator
{
    [AddComponentMenu("Dungeon Rooms/SpawnRoom")]
    public class SpawnRoom : Room
    {
        public Exit exit; // 탈출구

        public void DisableExit() // 스폰방중에 플레이어가 스폰이 안된 방들만 탈출구가 있음
        {
            exit.gameObject.SetActive(false);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerator
{
    public class TileData : TileBase
    {
        public TileBase tile;
        public TileType type;
        
        public bool isActivate;
        public bool isStatic; // 정적인거면 그냥 Instantiate, 동적인거면 동기화 해야하니깐 Photon Instantiate
    }


    public enum TileType
    {
        Floor = 0,
        Wall = 1,
        Door = 2,
        Box = 3,
        Trap1 = 3, 
        Trap2 = 4, // etc... 
    }
}
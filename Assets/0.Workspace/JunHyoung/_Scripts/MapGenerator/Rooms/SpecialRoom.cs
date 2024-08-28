using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapGenerator
{
    [AddComponentMenu("Dungeon Rooms/SpecialRoom")]
    public class SpecialRoom : Room
    {
        [Space(15),Header("SpecialRoom Feature")]
        [SerializeField] Tilemap entranceMap;

        [Space(10)]
        [SerializeField] SecretRoomEntrance entrance; // 계단이던 포탈이던, 비밀방으로 이동시킬 trigger Object

        public override void DrawRoom()
        {
            base.DrawRoom();

            if(entranceMap != null)
                 entranceMap.gameObject.SetActive(false);
        }


        public void ActivateSecretRoom(SecretRoom secretRoomPrefab, Vector3 secretRoomPos)
        {
            // 비밀방 생성 // 좌표는 MapManager에서 지정해서 호출해줌

            //if (!PhotonNetwork.IsMasterClient)
            //    return;

            if (!PhotonNetwork.IsConnected)
            {
                SecretRoom secretRoom = Instantiate(secretRoomPrefab, secretRoomPos, Quaternion.identity);
                secretRoom.DrawRoom();
                ActivateRandomEntrance(secretRoom);
            }
            else
            {
                SecretRoom secretRoom = Instantiate(secretRoomPrefab, secretRoomPos, Quaternion.identity);
                secretRoom.DrawRoom();
                ActivateRandomEntrance(secretRoom);
            }
        }

        ///미리 찍혀진 EntrancePoint중 랜덤으로 하나 활성화
        void ActivateRandomEntrance(SecretRoom secretRoom)
        {
            Vector3 selectedPos = GetRandomTilemapPos(entranceMap);
            secretRoom.exit.endPos = selectedPos; //비밀방 출구 좌표 지정

            // 해당 좌표에  비밀방 입구 오브젝트 생성 
            if(!PhotonNetwork.IsConnected)
            {
               SecretRoomEntrance newEntrance = Instantiate(entrance, selectedPos, Quaternion.identity);
                // 입구 좌표 및 출구좌표 지정
               newEntrance.endPos = secretRoom.entrancePoint.position;
               newEntrance.transform.parent = TileMapManager.Instance.ObjectMap.transform; //해도 좋고 안해도 됨
            }
            else if(PhotonNetwork.IsMasterClient)
            {
                secretRoom.InstantiateExit(selectedPos);
                object[] data = new object[] { secretRoom.entrancePoint.position.x, secretRoom.entrancePoint.position.y, secretRoom.entrancePoint.position.z };
                PhotonNetwork.InstantiateRoomObject(entrance.name, selectedPos, Quaternion.identity, 0, data);
            }  
        }
    }
}

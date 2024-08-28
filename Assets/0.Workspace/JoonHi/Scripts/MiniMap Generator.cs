using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MapGenerator;
using System;
using System.Linq;
using static MinimapRooms;

public class MiniMapGenerator : MonoBehaviour
{
    [Header("MapManager")]
    [SerializeField] MapGenerator.MapGenerator mapGenerator;
    [SerializeField] MapManager mapManager;

    [Header("MiniMap Room Prefab")]
    [SerializeField] GameObject minimapRoomPrefab;

    // Actions
    private Action generate;                                        // MapGenerator 호출 탐지
    public event System.Action FinishedGeneratingMiniMapEvent;      // 미니맵 생성 완료 호출

    private Dictionary<Room, MinimapRooms> minimapRooms = new Dictionary<Room, MinimapRooms>();


    private void Start()
    {
        generate = Generate;
        mapGenerator.FinishedGeneratingEvent += generate;
    }

    private void Generate()
    {
        Debug.Log("Mini Map Generator Action Activate");

        // Corridor Check
        int buf = 0;
        foreach (List<int> list in mapGenerator.connectedRoomGraph)
        {
            //Debug.Log($"Room{buf} (Count {list.Count}) : {string.Join(",", list)}");
            buf++;
        }

        // 미니맵 방 제작
        for (int i = 0; i < mapManager.Rooms.Count; i++)
        {
            CreateMinimapRoom(i, mapManager.Rooms.Values.ToList()[i]);
        }

        Debug.Log("MiniMap Generate Finish");
        FinishedGeneratingMiniMapEvent?.Invoke();
    }


    /// <summary> 특정 방에대한 미니맵 방 생성 </summary>
    /// <param name="num"> 방 번호 및 이름 뒤에 붙을 번호</param>
    /// <param name="room"> room은 Room 속성을 가진 방. room의 크기를 받아 미니맵의 크기를 정의 및 위치값을 반영</param>
    public void CreateMinimapRoom(int num, Room room)
    {
        GameObject minimapRoom = Instantiate(minimapRoomPrefab);
        minimapRoom.name = $"Minimap Room {num}";
        MinimapRooms minimapScript = minimapRoom.GetComponent<MinimapRooms>();
        minimapScript.minimapGen = this;

        minimapScript.SetMinimapRoomSize(room);

        // 위치조정
        var tp = room.floorTilemap.transform.position;
        var tBounds = room.floorTilemap.cellBounds;

        var c0 = new Vector3(tBounds.min.x, tBounds.min.y) + tp;
        var c2 = new Vector3(tBounds.max.x, tBounds.max.y) + tp;

        Vector3 center = Vector3.Lerp(c0, c2, 0.5f);    // 두 꼭지점의 중앙지점
        minimapRoom.transform.position = center;
        minimapRoom.transform.SetParent(this.transform);

        minimapRooms.Add(room, minimapScript);
    }

    /// <summary> 맵의 상태를 업데이트하는 함수. </summary>
    /// <param name="room"> room은 현제 플레이어가 출입한 Room 속성 방</param>
    public void UpdateMap(Room room)
    {
        minimapRooms.TryGetValue(room, out MinimapRooms updateRoom);
        updateRoom.SetVisibility(MinimapVisible.Show);
        foreach(int index in mapGenerator.connectedRoomGraph[minimapRooms.Keys.ToList().IndexOf(room)])
        {
            MinimapRooms temp = minimapRooms.Values.ToList()[index];
            // 연결된 방이 아직 표시되지 않은경우 이를 표시
            if (temp.curVisibility == MinimapVisible.Hide)
                temp.SetVisibility(MinimapVisible.Visible);
            // 만약 방이 완전히 표시되지 않았다면 복도가 표시되지 않았다는 뜻임으로 복도를 그린다.
            /*
             * if(temp.curVisibility != MinimapVisible.Show)
                updateRoom.DrawCorridor(minimapRooms.Keys.ToList()[index].gameObject.transform.position);
            */
            if (temp.curVisibility != MinimapVisible.Show)
                updateRoom.DrawCorridor(minimapRooms.Keys.ToList()[index]);
        }
    }

    /// <summary> 엑스칼리버 룸을 미니맵에 표시하는 함수 </summary>
    public void ShowExcaliburRoom()
    {
        minimapRooms.Values.ToList()[0].SetVisibility(MinimapVisible.Show);
    }

#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] MinimapRooms.MinimapVisible Visibility;
    [SerializeField] int SelectRoom;

    [ContextMenu("Debug All Visibility Setting")]
    private void DebugShowAllMinimap()
    {
        foreach(MinimapRooms room in minimapRooms.Values)
        {
            room.SetVisibility(Visibility);
        }
    }

    [ContextMenu("Debug Selected Visibility Setting")]
    private void DebugShowMinimapRoom()
    {
        if(minimapRooms.Values.ToList().Count > SelectRoom || SelectRoom > 0)
            minimapRooms.Values.ToList()[SelectRoom].SetVisibility(Visibility);
        else
            Debug.Log("Room Num No MATCH");
    }

    [ContextMenu("Debug Show Excaliver Room Minimap")]
    private void DebugShowExcaliver()
    {
        ShowExcaliburRoom();
    }
    #endif
}

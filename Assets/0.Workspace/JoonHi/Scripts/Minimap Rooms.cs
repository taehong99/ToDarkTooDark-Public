using DG.Tweening;
using MapGenerator;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class MinimapRooms : MonoBehaviour
{
    public enum MinimapVisible { Hide, Visible, Show,}

    public MiniMapGenerator minimapGen { get; set; }

    [Header("Minimap Sprites GameObject")]
    [SerializeField] GameObject Boundary;
    [SerializeField] GameObject Room;
    [SerializeField] GameObject BackGround;

    [Header("Minimap Icon")]
    [SerializeField] SpriteRenderer Icon;

    [Header("Corridors")]
    [SerializeField] GameObject corridorBundle;
    [SerializeField] GameObject corridorPrefab;

    [Header("Components")]
    [SerializeField] Collider2D minimapCollider;

    public MinimapVisible curVisibility { get; private set; }

    private SpriteRenderer _Boundary;
    private SpriteRenderer _Room;
    private SpriteRenderer _BackGround;
    private BoxCollider2D minimapcollider;

    private Room room;

    private void Awake()
    {
        minimapcollider = gameObject.GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        _Boundary = Boundary.GetComponent<SpriteRenderer>();
        _Room = Room.GetComponent<SpriteRenderer>();
        _BackGround = BackGround.GetComponent<SpriteRenderer>();
        Icon.gameObject.SetActive(false);

        SetVisibility(MinimapVisible.Hide);
    }

    public void SetMinimapRoomSize(Room room)
    {
        Boundary.transform.localScale = new Vector2(room.roomSize.x - 3, room.roomSize.y - 3);
        BackGround.transform.localScale = new Vector2(room.roomSize.x + 1, room.roomSize.y + 1);
        Room.transform.localScale = new Vector2(room.roomSize.x - 2, room.roomSize.y - 2);
        minimapcollider.size = new Vector2(room.roomSize.x - 2, room.roomSize.y - 2);

        // 룸타입에 따라 아이콘 변경
        // Icon.sprite = 아이콘 리스트[room.roomType];

        this.room = room;
    }

    public void SetIcon(Sprite iconSprite)
    {
        Icon.sprite = iconSprite;
    }

    public void SetVisibility(MinimapVisible visibility)
    {
        switch(visibility)
        {
            case MinimapVisible.Hide:
                _Boundary.color = new Color(1, 1, 1, 0);
                _BackGround.color = new Color(_BackGround.color.r, _BackGround.color.g, _BackGround.color.b, 0);
                _Room.color = new Color(_Room.color.r, _Room.color.g, _Room.color.b, 0);
                break;
            case MinimapVisible.Visible:
                _Boundary.color = new Color(1, 1, 1, 1f);
                _BackGround.color = new Color(0.45f, 0.45f, 0.45f, 1f);
                _Room.color = new Color(_Room.color.r, _Room.color.g, _Room.color.b, 0.5f);
                break;
            case MinimapVisible.Show:
                _Boundary.color = new Color(1, 1, 1, 1);
                _BackGround.color = new Color(0.22f, 0.22f, 0.22f, 1);
                _Room.color = new Color(_Room.color.r, _Room.color.g, _Room.color.b, 1);
                if(Icon != null)
                    Icon.gameObject.SetActive(true);
                break;
        }
        curVisibility = visibility;
    }

    public void DrawCorridor(Vector2 TargetRoom)
    {
        LineRenderer corridor = Instantiate(corridorPrefab).GetComponent<LineRenderer>();
        corridor.gameObject.transform.SetParent(corridorBundle.transform);
        corridor.SetPosition(0, transform.position);
        corridor.SetPosition(1, TargetRoom);
        corridor.widthMultiplier = 5;
    }

    public void DrawCorridor(Room TargetRoom)
    {
        LineRenderer corridor = Instantiate(corridorPrefab).GetComponent<LineRenderer>();
        corridor.gameObject.transform.SetParent(corridorBundle.transform);
        corridor.widthMultiplier = 7;

        float up = Vector2.Distance(TargetRoom.position, room.DoorPosition(Vector3.up));
        float down = Vector2.Distance(TargetRoom.position, room.DoorPosition(Vector3.down));
        float left = Vector2.Distance(TargetRoom.position, room.DoorPosition(Vector3.left));
        float right = Vector2.Distance(TargetRoom.position, room.DoorPosition(Vector3.right));

        float min = Mathf.Min(up, down, left, right);
        if (min == up)
        {
            corridor.SetPosition(0, room.DoorPosition(Vector2.up));
            corridor.SetPosition(1, TargetRoom.DoorPosition(Vector2.down));
        }
        else if (min == down)
        {
            corridor.SetPosition(0, room.DoorPosition(Vector2.down));
            corridor.SetPosition(1, TargetRoom.DoorPosition(Vector2.up));
        }
        else if (min == left)
        {
            corridor.SetPosition(0, room.DoorPosition(Vector2.left));
            corridor.SetPosition(1, TargetRoom.DoorPosition(Vector2.right));
        }
        else
        {
            corridor.SetPosition(0, room.DoorPosition(Vector2.right));
            corridor.SetPosition(1, TargetRoom.DoorPosition(Vector2.left));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && collision.gameObject.GetComponent<PhotonView>().IsMine)
        {
            Debug.Log("update Minimap");
            minimapGen.UpdateMap(room);
            minimapcollider.enabled = false;
        }
    }
}

using Cinemachine;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Rendering.Universal;

public class PhotonPlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] PlayerStates state;

    [SerializeField] Animator animator;

    [SerializeField] PlayerData data;
    [SerializeField] AudioClip sound;
    public PlayerData Data { get { return data; } }
    [SerializeField] PlayerStatsManager stat;
    // 나중에 네트워크에서 내 캐릭이 아니면 prioriy를 줄여주자
    [SerializeField] GameObject clientOnlyGameObjects;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    public CinemachineVirtualCamera VirtualCamera { get {  return virtualCamera; } }
	public Light2D light2D;
    [SerializeField] SpriteRenderer playerRendere;
    Vector3 moveDir;
    Vector3 mouseDir;
    float speed;
    [SerializeField] float verticalWallCheckOffset;
    [SerializeField] float horizontalWallCheckOffset;

    public bool canFlip = true;
    public event Action<bool> playerFlipped;
    // 횡 스크롤 아이템의 위아래 가는 방향 제한 두기윈한 변수(이효석)
    bool isUpDown= true;
    public bool IsUpDown {  get { return isUpDown; } set { isUpDown = value; } }
    public bool IsFacingLeft
    {
        get
        {
            return playerRendere.flipX;
        }
        set
        {
            if(playerRendere.flipX != value)
            {
                playerFlipped?.Invoke(value);
            }
            playerRendere.flipX = value;
        }
    }

    private void Awake()
    {
        if (!PhotonNetwork.InRoom)
            return;

        if (!PhotonNetwork.IsConnected || photonView.IsMine)
        {
            light2D.gameObject.SetActive(true);
            clientOnlyGameObjects.SetActive(true);
        }
    }

    public void SetupTutorialPlayer()
    {
        light2D.gameObject.SetActive(true);
        clientOnlyGameObjects.SetActive(true);
    }

    //public void Start()
    //{
    //    if (!PhotonNetwork.IsConnected)
    //    {
    //        light2D.gameObject.SetActive(true);
    //        clientOnlyGameObjects.SetActive(true);
    //    }
    //}

    private void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            return;
        Debug.DrawRay(transform.position, Vector3.right * horizontalWallCheckOffset, Color.green);
        Debug.DrawRay(transform.position, Vector3.up * verticalWallCheckOffset, Color.green);

        Move();
        Flip();
    }

    void FixedUpdate()
    {
        
    }

    private void OnMove(InputValue inputValue)
    {
        speed = stat.FinalStat(StatType.Speed);
        Vector2 input = inputValue.Get<Vector2>();
        moveDir.x = input.x;
        if(isUpDown) // 아니면 y축 입력값자체를 안받는걸로
        moveDir.y = input.y;
        Manager.Sound.WalkSFXPlay(sound);
    }
    Vector3 move;
    private void Move()
    {
        if (state.IsMove)
        {
            float xMove = moveDir.x *speed  * Time.deltaTime;
            float yMove = moveDir.y * speed * Time.deltaTime;
            move = new Vector3(xMove, yMove, 0);
            transform.Translate(MoveCheck(move.normalized));
            // 애니메이션
            if (moveDir == Vector3.zero)
            {
                animator.SetBool("Move", false);
                Manager.Sound.WalkSfxStop();

            }
            else
            {
                animator.SetBool("Move", true);
            }
        }

    }
    // 마우스 방향에 따라 Flip 반전
    private void Flip()
    {
        if (!state.IsDie && canFlip)
        {
            mouseDir = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            mouseDir = Camera.main.ScreenToWorldPoint(mouseDir);
            Vector2 direction = (mouseDir - transform.position).normalized;

            if (direction.x < 0)
            {
                IsFacingLeft = true;
            }
            else if (direction.x > 0)
            {
                IsFacingLeft = false;
            }
        }
    }
    [SerializeField] LayerMask rayMask;
    private Vector2 MoveCheck(Vector2 direction)
    {
        RaycastHit2D hit;
        if (direction.y > 0)
        {
            hit = Physics2D.Raycast(transform.position, direction, verticalWallCheckOffset, rayMask);
        }
        else
        {
            hit = Physics2D.Raycast(transform.position, direction, horizontalWallCheckOffset, rayMask);
        }

        if (hit.collider != null)
        {
            return Vector3.zero;
        }
        return move;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Synced values:
        // - SpriteRenderer.flipX

        if (stream.IsWriting)
        {
            stream.SendNext(IsFacingLeft);
        }
        if (stream.IsReading)
        {
            IsFacingLeft = (bool)stream.ReceiveNext();
        }
    }
}

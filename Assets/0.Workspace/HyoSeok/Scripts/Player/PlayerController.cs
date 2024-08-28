using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerStates newStates;

    [SerializeField] Animator animator;

    public Animator animator2 { get { return animator; } }
    [SerializeField] PlayerData data;

    public PlayerData Data { get { return data; } }
    [SerializeField] PlayerStatsManager stat;
    // 나중에 네트워크에서 내 캐릭이 아니면 prioriy를 줄여주자
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] SpriteRenderer playerRendere;
    Vector3 moveDir;
    Vector3 mouseDir;
    private void Update()
    {
        Move();
        Flip();
    }
    private void OnMove(InputValue inputValue)
    {
        Vector2 input = inputValue.Get<Vector2>();
        moveDir.x = input.x;
        moveDir.y = input.y;
    }
    Vector3 move;
    private void Move()
    {
        if (newStates.IsMove)
        {
            //state.ChangeState(PlayerState.Move);
           // float xMove = moveDir.x * stat.curSpeed * Time.deltaTime;
            //float yMove = moveDir.y * stat.curSpeed * Time.deltaTime;
            move = moveDir * stat.curSpeed * Time.deltaTime;
           // move = new Vector3(xMove, yMove, 0);
            transform.Translate(MoveCheck(move.normalized));
            // 애니메이션
            if (moveDir == Vector3.zero)
            {
                animator.SetBool("Move", false);
                //state.ChangeState(PlayerState.Idle);
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
        if (!newStates.IsDie)
        {
            mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseDir - transform.position).normalized;

            if (direction.x < 0)
            {
                playerRendere.flipX = true;
            }
            else if (direction.x > 0)
            {
                playerRendere.flipX = false;
            }
        }
    }
    [SerializeField] LayerMask rayMask;
    private Vector2 MoveCheck(Vector2 direction)
    {
        RaycastHit2D hit;
        if (direction.y > 0)
        {
            hit = Physics2D.Raycast(transform.position, direction, 1f, rayMask);
        }
        else
        {
            hit = Physics2D.Raycast(transform.position, direction, 0.5f, rayMask);
        }

        if (hit.collider != null)
        {
            return Vector3.zero;
        }
        return move;
    }
}

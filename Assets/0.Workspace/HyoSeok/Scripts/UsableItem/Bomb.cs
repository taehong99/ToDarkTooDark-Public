using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bomb : MonoBehaviour
{
    Collider2D[] colliders = new Collider2D[10];
    [SerializeField] LayerMask mask;
    [SerializeField] GameObject damageRadius;
    // 사거리 : 플레이어와의 거리에서의 제한을 둬야됨.... 그럼 그 플레이어는 어떻게 찾아야될까?
    [SerializeField] GameObject m_player;
    [SerializeField] GameObject effectAni;
    Animator animator;
    EffectTimer effectTimer;
    private void Awake()
    {
        // 나중에 바꿔줘야됨
        m_player = Manager.Game.MyPlayer;
        mouseFollower = StartCoroutine(MouseFollower());
        GameObject aa = Instantiate(effectAni, transform);
        animator = aa.GetComponent<Animator>();
        effectTimer = aa.GetComponent<EffectTimer>();

    }
    private void OnEnable()
    {
        m_player.GetComponent<PlayerStates>().MoveAttack();
    }
     private void OnDisable()
    {
    }
    private void OnMouseDown()
    {
        if ((m_player.transform.position - transform.position).sqrMagnitude < 50)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.position, 1, mask);
            Debug.Log(hit.collider);
            if (hit.collider == null)
            {
                StopCoroutine(mouseFollower);
                StartCoroutine(Timer());
            }
        }

    }
    Coroutine mouseFollower;
    Vector3 mousePosition;
    IEnumerator MouseFollower()
    {
        while (true)
        {
            mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10) ;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = mousePosition;
            yield return new WaitForSeconds(0.1f);

        }
    }
    IEnumerator Timer()
    {
        m_player.GetComponent<PlayerStates>().AttackNomal();
        damageRadius.SetActive(true);
        effectTimer.effectDamageEvent += BombDamage;
        yield return new WaitForSeconds(2f);
        animator.SetTrigger("Bomb");
        damageRadius.SetActive(false);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;

    }
    Collider2D[] collider = new Collider2D[10];
    private void BombDamage()
    {
        // 데미지
        int count = Physics2D.OverlapBoxNonAlloc(transform.position, new Vector2(3, 3), 0, collider);
        for (int i = 0; i < count; i++)
        {
            if (collider[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(40);
            }
        }

    }

}
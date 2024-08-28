using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mallet : MonoBehaviour
{
    [SerializeField] LineRenderer renderer;
    [SerializeField] GameObject m_player;
    [SerializeField] GameObject effectAni;
    [SerializeField] SpriteRenderer spriteRenderer;
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
        renderer.gameObject.SetActive(true);
        spriteRenderer.flipY = false;

    }
    private void OnEnable()
    {
        m_player.GetComponent<PlayerStates>().MoveAttack();
    }
    private void OnDisable()
    {
        m_player.GetComponent<PlayerStates>().AttackNomal();
    }
    private void OnMouseDown()
    {
        if ((m_player.transform.position - transform.position).sqrMagnitude < 50)
        {
            spriteRenderer.flipY = true;
            StopCoroutine(mouseFollower);
            MalletDamage();
            renderer.gameObject.SetActive(false);
            StartCoroutine(Malletoff());
        }
    }
    IEnumerator Malletoff()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
    Coroutine mouseFollower;
    Vector3 mousePosition;
    IEnumerator MouseFollower()
    {
        while (true)
        {
            mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = mousePosition;
            RangeRenderer(mousePosition);
            yield return new WaitForSeconds(0.1f);
        }
    } 
    Collider2D[] collider = new Collider2D[10];
    private void MalletDamage()
    {
        // 데미지
        int count = Physics2D.OverlapBoxNonAlloc(transform.position, new Vector2(3, 2), 0, collider);
        for (int i = 0; i < count; i++)
        {
            if (collider[i].gameObject == m_player.gameObject)
                return;
            if (collider[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(40);
                Crush(collider[i]);
            }
        }
    }
    private void Crush(Collider2D collider)
    {
        if (collider.TryGetComponent(out GameObject enemyObject))
        {
            StartCoroutine(DurationTime(enemyObject));
        }
        // 플레이어만 가능
        if (collider.TryGetComponent(out StatusEffectManager enemy))
        {
            DeBuff deBuff = new DeBuff("디버프", 10, StatType.Speed, 0.1f, true);
            enemy.AddEffect(deBuff);
        }
    }
    IEnumerator DurationTime(GameObject enemyObject)
    {
        enemyObject.transform.localScale = new Vector3(1, 0.2f, 1);
        yield return new WaitForSeconds(10f);
        enemyObject.transform.localScale = new Vector3(1, 1, 1);
    }
    IEnumerator MalletMotion()
    {
        yield return null;
    }
    private void RangeRenderer(Vector3 mousePosition)
    {
        renderer.SetPosition(0, new Vector3(mousePosition.x -1.5f, mousePosition.y, 0));
        renderer.SetPosition(1, new Vector3(mousePosition.x +1.5f, mousePosition.y, 0));
    }
  /*  private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(3, 2, 0));

    }*/
}

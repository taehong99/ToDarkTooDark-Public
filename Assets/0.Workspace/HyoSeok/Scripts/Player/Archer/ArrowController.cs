using System.Collections;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ArrowController : PooledObject
{
    public Player OwnerPhotonPlayer { get; private set; }
    public GameObject Owner { get; private set; }
    [SerializeField] SpriteRenderer sprite;
    public float damage;
    float speed = 10.0f;
    string aniName;
    int hitsound;

    private void OnEnable()
    {
        if(sprite == null)
            sprite = GetComponentInChildren<SpriteRenderer>(true);
    }


    public void StartFireArrow(Vector2 startPos, Vector2 endPos, Player player, GameObject owner , string aniName, int hitsound)
    {
        OwnerPhotonPlayer = player;
        Owner = owner;
        StartCoroutine(FireArrow(startPos, endPos));
        this.aniName = aniName;
        this.hitsound = hitsound;   
    }
    private IEnumerator FireArrow(Vector2 startPos, Vector2 endPos)
    {
        transform.position = startPos;
        while ((Vector2) transform.position != endPos)
        {
            transform.position = Vector2.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            yield return null;
        }
        Release();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Release();
            return;
        }

        // 플레이어랑 충돌
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PhotonView photonView))
        {
            // 팀전
            if (PhotonNetwork.CurrentRoom.GetGameMode() == false)
            {
                if (photonView.Owner.GetTeam() == OwnerPhotonPlayer.GetTeam())
                    return;
            }
            else // 1:1
            {
                if (OwnerPhotonPlayer == photonView.Owner)
                    return;
            }
        }
        
        if (collision.TryGetComponent(out IDamageable damageable))
        {
            Debug.Log($"Arrow Damage : {(int) damage}");
            // Only Master Client deals damage
            if (PhotonNetwork.IsMasterClient)
            {
                damageable.TakeDamage((int) damage);
                damageable.LastHitter = Owner;
            }
            if( Owner.TryGetComponent(out ArcherAttack archer))
            {
                archer.HitAniPlay(aniName, transform.position);
                archer.SFXPlay(hitsound);
            }
            Release();
        }
    }

    Vector3[] m_points = new Vector3[4];

    private float m_timerMax = 2f;
    private float m_timerCurrent = 0;
    private float m_speed;

    public void Init(Vector2 _startPos, Vector2 _endTr, float _speed, float _newPointDistanceFromStartTr, float _newPointDistanceFromEndTr, Player Player, int seed, GameObject Owner, string aniName)
    {
        OwnerPhotonPlayer = Player;
        this.Owner = Owner;
        this.aniName = aniName;
        Random.InitState(seed);

        m_speed = _speed;

        // 끝에 도착할 시간을 랜덤으로 줌.
        m_timerMax = Random.Range(0.8f, 1.0f);

        // 시작 지점.
        m_points[0] = _startPos;

        // 시작 지점을 기준으로 랜덤 포인트 지정.
        m_points[1] = _startPos +
            (Vector2.right * Random.Range(-1.0f, 1.0f) * _newPointDistanceFromStartTr) +
            (_newPointDistanceFromStartTr * Random.Range(-0.15f, 1.0f) * Vector2.down);

        // 도착 지점을 기준으로 랜덤 포인트 지정.
        m_points[2] = _endTr +
            (Vector2.right * Random.Range(-1.0f, 1.0f) * _newPointDistanceFromEndTr) +
            (_newPointDistanceFromEndTr * Random.Range(-0.15f, 1.0f) * Vector2.down);

        // 도착 지점.
        m_points[3] = _endTr;

        transform.position = _startPos;
        StartCoroutine(SkillLerp());
    }

    IEnumerator SkillLerp()
    {
        while (m_timerCurrent < m_timerMax)
        {
            // 경과 시간 계산.
            m_timerCurrent += Time.deltaTime * m_speed;

            // 베지어 곡선으로 X,Y 좌표 얻기.
            transform.position = new Vector3(
                CubicBezierCurve(m_points[0].x, m_points[1].x, m_points[2].x, m_points[3].x),
                CubicBezierCurve(m_points[0].y, m_points[1].y, m_points[2].y, m_points[3].y),
                0
            );
            yield return null;
        }

        m_timerCurrent = 0;
        Release();
    }
    /// <summary>
    /// 3차 베지어 곡선.
    /// </summary>
    /// <param name="a">시작 위치</param>
    /// <param name="b">시작 위치에서 얼마나 꺾일 지 정하는 위치</param>
    /// <param name="c">도착 위치에서 얼마나 꺾일 지 정하는 위치</param>
    /// <param name="d">도착 위치</param>
    /// <returns></returns>
    private float CubicBezierCurve(float a, float b, float c, float d)
    {
        // (0~1)의 값에 따라 베지어 곡선 값을 구하기 때문에, 비율에 따른 시간을 구했다.
        float t = m_timerCurrent / m_timerMax; // (현재 경과 시간 / 최대 시간)

        // 방정식.

        return Mathf.Pow((1 - t), 3) * a
            + Mathf.Pow((1 - t), 2) * 3 * t * b
            + Mathf.Pow(t, 2) * 3 * (1 - t) * c
            + Mathf.Pow(t, 3) * d;
    }
}

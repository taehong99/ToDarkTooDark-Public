using Photon.Pun;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class PlayerUtils
{
    public static float Critical(float baseDamage, float criticalChance, float criticalMultiplier)
    {
        float random = Random.Range(0, 1f);
        if (random < criticalChance)
        {
            return baseDamage * criticalMultiplier;
        }
        else
        {
            return baseDamage;
        }
    }
    public static Vector2 CheckForCollision(Vector3 m_position, Vector2 direction, float Range, LayerMask layer)
    {
        RaycastHit2D hit = Physics2D.Raycast(m_position, direction, Range, layer);
        if (hit.collider != null)
        {
            return hit.point - direction * 0.5f;
        }
        return (Vector2) m_position + direction * Range;
    }
    public static int countEnemy;
    public static Collider2D[] AttackCheck(float Angle, float damage, float range, float criticalChance, float multiplier, float Stunduration, GameObject m_Object, Collider2D[] colliders)
    {
        countEnemy = 0;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - m_Object.transform.position).normalized;


        // 오버랩을 통해 근처 콜라이더 다 검사
        int count = Physics2D.OverlapCircleNonAlloc(m_Object.transform.position, range, colliders);
        for (int i = 0; i < count; i++)
        {
            Vector2 directionToEnemy = (colliders[i].transform.position - m_Object.transform.position).normalized;

            // 플레이어랑 충돌
            if (colliders[i].CompareTag("Player") && colliders[i].TryGetComponent(out PhotonView photonView))
            {
                if (!PhotonNetwork.IsConnected)
                    continue;

                // 팀전
                if (PhotonNetwork.CurrentRoom.GetGameMode() == false)
                {
                    Team myTeam = m_Object.GetComponent<PhotonView>().Owner.GetTeam();
                    if (photonView.Owner.GetTeam() == myTeam)
                        continue;
                }
                else // 1:1
                {
                    if (colliders[i].gameObject == m_Object)
                        continue;
                }
            }

            // Legacy
            //if (colliders[i].gameObject == m_Object)
            //    continue;
            float angleToEnemy = Vector2.Angle(direction, directionToEnemy);
            if (angleToEnemy <= Angle / 2)
            {
                if (colliders[i].TryGetComponent(out IDamageable damageable))
                {
                    damageable.LastHitter = m_Object;
                    damageable.TakeDamage((int) Critical(damage, criticalChance, multiplier));
                    damageable.ApplyStun(Stunduration);
                    countEnemy++;
                    
                }
            }
        }
        return colliders;
    }
}

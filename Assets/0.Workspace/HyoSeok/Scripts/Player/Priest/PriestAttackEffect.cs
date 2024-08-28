using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PriestAttackEffect : PooledObject
{
    public Player OwnerPhotonPlayer { get; private set; }
    public GameObject Owner { get; private set; }

    public float damage;
    float speed = 5f;
    string aniName;
    public void StartFire(Vector2 startPos, Vector2 endPos, Player player, GameObject owner, string aniName)
    {
        this.Owner = owner;
        OwnerPhotonPlayer = player;
        this.aniName = aniName;
        StartCoroutine(Fire(startPos, endPos));
    }

    private IEnumerator Fire(Vector2 startPos, Vector2 endPos)
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
        if (collision.gameObject.layer == 21)
        {
            if (Owner.TryGetComponent(out PriestAttack archer))
            {
                archer.HitAniPlay("PriestSkill2", collision.transform.position);
                archer.HitSoundPlay(2);
            }
            Destroy(collision.gameObject);
            return;
        }
        // 주인한테 데미지 안들어감
        // 같은팀이면 데미지 안들어감 (본인 포함)
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PhotonView photonView))
        {
            if (photonView.Owner.GetTeam() == OwnerPhotonPlayer.GetTeam())
                return;
        }

        if (collision.TryGetComponent(out IDamageable damageable))
        {
            // Only Master Client deals damage
            if (PhotonNetwork.IsMasterClient)
            {
                damageable.TakeDamage((int) damage);
                damageable.LastHitter = Owner;
            }
            if (Owner.TryGetComponent(out PriestAttack archer))
            {
                archer.HitAniPlay(aniName, collision.transform.position);
                archer.HitSoundPlay(3);

            }

        }
    }

}

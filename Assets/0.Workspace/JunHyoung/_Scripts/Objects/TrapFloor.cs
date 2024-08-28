using System.Collections;
using Tae;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap), typeof(TilemapCollider2D))]
public class TrapFloor : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Tilemap trapFloor;
    [SerializeField] Collider2D trapCollider;

    [Space(10), Header("TileSet")]
    [SerializeField] Tile activeTrap;
    [SerializeField] Tile inactiveTrap;

    [Space(10), Header("Settings")]
    [SerializeField] bool activeState = false;

    [Space(10), Header("Spec")]
    [SerializeField] int damageAmount;

    private PlayerHealth player;
    private IEnumerator onthespike;

    private bool deactivate;

    private void Awake()
    {
        deactivate = false;
    }

    public void Deactivate()
    {
        ActiveFloor(true);
        trapCollider.enabled = false;
        deactivate = true;
    }

    public void ActiveFloor(bool state)
    {
        if (deactivate == false)
        {
            activeState = state;
            // 타일맵 찍는 메서드
            ChangeTilemap();
            // Trigercolider 활성화 하는 메서드
            trapCollider.enabled = activeState;
        }
    }

    [ContextMenu("ActiveFloor")]
    void ActiveFloor()
    {
        activeState = !activeState;
        ChangeTilemap();
        trapCollider.enabled = activeState;
    }

    void ChangeTilemap()
    {
        foreach (var item in trapFloor.cellBounds.allPositionsWithin)
        {
            if (trapFloor.HasTile(item))
            {
                trapFloor.SetTile(item, activeState ? activeTrap : inactiveTrap);
            }
        }
    }


    // 가시 함정 닿을 때 행위 정의
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PlayerHealth>().photonView.IsMine)
        {
            // 데미지 
            player = collision.GetComponent<PlayerHealth>();
            player.TakeFixedDamage(damageAmount);
            onthespike = OnSpike();
            StartCoroutine(onthespike);
            // 넉백 등
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PlayerHealth>().photonView.IsMine)
        {
            // 데미지 
            player = null;
            StopCoroutine(onthespike);
            // 넉백 등
        }
    }

    IEnumerator OnSpike()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (player == null)
                break;
            player.TakeFixedDamage(damageAmount);
        }
    }
}

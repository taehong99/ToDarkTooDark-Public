using ItemLootSystem;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BreakableObject : MonoBehaviour, IDamageable
{
    [SerializeField] SpriteRenderer render;

    [SerializeField] ObjectDropTable dropTable; // 얘는 포톤뷰 붙일수 없으니깐, 박스랑은 다르게 Itemfactory 통해서 아이템 드랍 해줘야할듯
    [SerializeField] AudioClip breakSFX;

    int health = 1;
    public int Health { get => health; set => Debug.Log("뭐쓰지"); }
    public int MaxHealth { get => health; set => Debug.Log("뭐쓰지2"); }
    public GameObject LastHitter { get => null; set => Debug.Log("nothing"); }

    public event Action<int> healthChangedEvent;
    public event Action diedEvent;

    bool destroyed;

    public void ApplyStun(float duration)
    {
        if (duration == 0)
            return;
        Debug.Log("스턴이 걸리겠냐");
    }

    public void Heal(float amount, bool isPercent)
    {
        Debug.Log("힐을 왜 함");
    }

    public void TakeDamage(int amount)
    {
        if (destroyed)
            return;

        if (PhotonNetwork.IsConnected)
            Manager.Event.ObjectBreak(transform.position);
        else
            SyncBreak();
    }

    [ContextMenu("Break")]
    public void SyncBreak()
    {
        Debug.Log("부셔짐 동기화");

        if (breakSFX != null)
            Manager.Sound.PlaySFX(breakSFX);

        DropItem();
        StartCoroutine(FadeOutRoutine());
        destroyed = true;
    }

    float fadeOutTime = 0.75f;

    IEnumerator FadeOutRoutine()
    {
        render?.DOFade(0, fadeOutTime).SetUpdate(true);
        yield return YieldCache.WaitForSecondsRealTime(fadeOutTime);
        diedEvent?.Invoke();
        Destroy(gameObject);
    }

    void DropItem()
    {
        if (dropTable == null)
            return;
        List<BaseItemData> pickedItems = dropTable.PickedItem();

        if (pickedItems == null)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var item in pickedItems)
            {
                ItemFactory.Instance.SpawnItem(item, transform.position);
            }
        }
    }

    public void TakeFixedDamage(int amount)
    {
        Debug.Log("이건 언제 쓰는거임");
    }
}

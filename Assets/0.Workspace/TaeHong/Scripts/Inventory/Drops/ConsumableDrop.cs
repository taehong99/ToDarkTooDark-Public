using ItemLootSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tae.Inventory
{
    /// <summary>
    /// 이 클래스는 장비를 제외한 아이템에 붙인다 :
    ///     -포션 아이템
    ///     -사용형 아이템
    ///     -즉발형 아이템
    ///     -기타 아이템
    /// ConsumableDrop은 플레이어가 일정 사거리 안에 들어오면 아이템이 플레이어의
    /// 위치로 빨려들어가고 접촉하면 플레이어가 이 아이템을 획득한다.
    /// </summary>
    public class ConsumableDrop : MonoBehaviour, ICollectable
    {
        [SerializeField] BaseItemData item;
        [SerializeField] float bezierOffset = 0.5f;
        private LayerMask interactorLayer;
        private SpriteRenderer spriteRenderer;
        private bool pullStarted;
        private bool canBePulled;
        [SerializeField] private int count = 1;
        public int Count { get { return count; } set { count = value; } }
        private GameObject previousOwner;
        const float PREVENT_PICKUP_TIME = 10;

        private void Start()
        {
            interactorLayer = 1 << LayerMask.NameToLayer("Interactor");

            canBePulled = true;
            /*if (item != null)
            {
                InitDrop(item, 1, null);
            }*/
        }

        public void InitDrop(BaseItemData item, int count, GameObject owner)
        {
            this.item = item;
            this.count = count;
            previousOwner = owner;

            if (item.isPotion)
            {
                spriteRenderer.sprite = PotionManager.Instance.unknownPotionDic[item.ID].sprite;
            }
            else
            {
                spriteRenderer.sprite = item.icon;
            }

            PreventPickUp();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!canBePulled || pullStarted || !interactorLayer.Contain(collision.gameObject.layer))
                return;

            if (collision.gameObject == previousOwner)
                return;

            // 플레이어가 근처에 오면 플레이어로 빨려들어가기
            if (collision.TryGetComponent(out Interactor interactor))
            {
                StartCoroutine(GetPulledRoutine(interactor.collector));
                pullStarted = true;
            }
        }

        // 베지에 곡선을 이용한 자연스러운 빨려들어가기 효과
        private IEnumerator GetPulledRoutine(Collector collector)
        {
            float t = 0;
            Vector2 p0 = transform.position;
            Vector2 p3 = collector.transform.position;
            Vector2 p1 = new Vector2(p0.x + Random.Range(0f, bezierOffset), p0.y + Random.Range(0f, bezierOffset));
            Vector2 p2 = new Vector2(p3.x + Random.Range(0f, bezierOffset), p3.y + Random.Range(0f, bezierOffset));

            while ((collector.transform.position - transform.position).sqrMagnitude >= 0.01f)
            {
                p3 = collector.transform.position;
                transform.position = (Vector3) MathUtilities.CubicBezierCurve(p0, p1, p2, p3, MathUtilities.EaseInQuad(t));
                t += Time.deltaTime;
                yield return null;
            }

            OnCollect(collector);
        }

        // 플레이어랑 접촉시 플레이어에게 ItemData 정보 전송
        public void OnCollect(Collector collector)
        {
            if (InventoryManager.Instance.IsConsumableInventoryFull)
            {
                StopAllCoroutines();
                previousOwner = collector.gameObject;
                PreventPickUp();
                return;
            }

            for (int i = 0; i < count; i++)
            {
                collector.CollectItem(item);
            }

            Destroy(gameObject);
        }

        // 이 아이템을 버린 플레이어는 10초간 재획득이 불가능
        private void PreventPickUp()
        {
            StartCoroutine(PreventPickUpRoutine());
        }

        private IEnumerator PreventPickUpRoutine()
        {
            yield return new WaitForSeconds(PREVENT_PICKUP_TIME);
            previousOwner = null;
        }
    }
}
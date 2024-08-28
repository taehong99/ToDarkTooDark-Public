using Photon.Pun;
using System.Collections;
using Tae;
using Tae.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ItemLootSystem
{
    public class DropedItem : MonoBehaviourPun, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, // PointerHandler
        IInteractable,// ICollectable,   // User Define Interface
        IPunObservable, IPunInstantiateMagicCallback // Photon Interface
    {
        [SerializeField] BaseItemData itemData;
        [SerializeField] int dropCount = 1;
        [SerializeField] float expirationTime;
        public int DropCount { get => DropCount; set => dropCount = value; }
        [Space(10)]
        [SerializeField] TextMeshPro nameText;
        [SerializeField] SpriteRenderer spriteRenderer;

        [SerializeField] LayerMask layerMask;
        
        //for Control TrailRenderer
        [SerializeField] TrailRenderer trailRenderer;

        [Space(10)]
        [SerializeField] float checkInterval = 0.2f;

        // 아이템 줍기
        private PotionManager PotionManager => PotionManager.Instance;
        [SerializeField] float pickUpDelayTime = 1f;
        [SerializeField] float pickUpBezierOffset = 0.5f;
        private bool canPickUp;
        private LayerMask interactorLayer;
        private Collider2D myCollider;
        [SerializeField] GameObject interactionUI;

        private Vector3 lastPosition;
        private float timeSinceLastMove;

        public BaseItemData ItemData
        {
            get { return itemData; }
            set { itemData = value; SetFromItemData(); }
        }

        /**********************************************
        *                 Unity Event
        ***********************************************/

        private void Awake()
        {
            interactorLayer = 1 << LayerMask.NameToLayer("Interactor");
            myCollider = GetComponent<Collider2D>();
        }

        void OnEnable()
        {
            lastPosition = transform.position;
            timeSinceLastMove = 0.0f;
            //layerMask 로 교체할것.
        }

        void Update()
        {
            ControlTrailRenderer();
            StartCoroutine(ExpirationPoll());
        }

        /**********************************************
        *                 Methods
        ***********************************************/

        public void Drop(float dropRange, float dropSpeed)
        {
            transform.BounceDrop(this, LayerMask.GetMask("Wall"), true, dropRange, dropSpeed);
        }

        IEnumerator ExpirationPoll()
        {
            // 아무도 expirationTime초 동안 안먹으면 제거
            while (true)
            {
                if (timeSinceLastMove >= expirationTime)
                    if (PhotonNetwork.IsConnected)
                        photonView.RPC("Destroy", photonView.Owner);
                    else
                        Destroy(gameObject);
                yield return YieldCache.WaitForSeconds(1);
            }
        }

        void ControlTrailRenderer()
        {
            if (transform.position != lastPosition)
            {
                lastPosition = transform.position;
                timeSinceLastMove = 0.0f;
            }
            else
            {
                timeSinceLastMove += Time.deltaTime;
            }

            if (timeSinceLastMove >= checkInterval)
            {
                if (trailRenderer != null)
                {
                    trailRenderer.enabled = false;
                }
            }
        }

        void SetFromItemData()
        {
            if (itemData.isPotion)
            {
                UnknownPotionData data = PotionManager.PotionBottleData(itemData.ID);
                nameText.text = data.potionName;
                spriteRenderer.sprite = data.sprite;
                StartCoroutine(PickUpDelay(false));
                return;
            }

            spriteRenderer.sprite = itemData.icon;
            SetColorByItemRarity();
            ShowItemName();
            StartCoroutine(PickUpDelay(true));
        }

        private IEnumerator PickUpDelay(bool isEquipment)
        {
            canPickUp = false;
            myCollider.enabled = false;
            yield return YieldCache.WaitForSeconds(pickUpDelayTime);
            myCollider.enabled = true;
            canPickUp = true;
        }


        void ShowItemName()
        {
            if (itemData is EquipItemData)
            {
                EquipItemData equipItemData = itemData as EquipItemData;
                int enhanceLevel = equipItemData.enhanceLevel;
                string enhanceText = enhanceLevel > 0 ? $" +{enhanceLevel}" : string.Empty;
                nameText.text = equipItemData.GetItemNameWithNewLines() + enhanceText;
            }
            else
            {
                nameText.text = itemData.Name;
            }
        }

        void SetColorByItemRarity()
        {
            Color rarityColor = RarityDrawer.GetColorForRarity(itemData.rarity);

            trailRenderer.startColor = rarityColor;
            trailRenderer.endColor = rarityColor;
            nameText.color = rarityColor;
        }


        /**********************************************
        *                 Interactions
        ***********************************************/
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (itemData is EquipItemData)
                return;
             
            if (!canPickUp || !interactorLayer.Contain(collision.gameObject.layer))
                return;

            /*if (collision.gameObject == previousOwner)
                return;*/

            // 플레이어가 근처에 오면 플레이어로 빨려들어가기
            if (collision.TryGetComponent(out Interactor interactor))
            {
                StartCoroutine(GetPulledRoutine(interactor.collector));
            }
        }

        // 베지에 곡선을 이용한 자연스러운 빨려들어가기 효과
        private IEnumerator GetPulledRoutine(Collector collector)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                
            canPickUp = false;
            float t = 0;
            /*Vector2 p0 = transform.position;
            Vector2 p3 = collector.transform.position;
            Vector2 p1 = new Vector2(p0.x + Random.Range(0f, pickUpBezierOffset), p0.y + Random.Range(0f, pickUpBezierOffset));
            Vector2 p2 = new Vector2(p3.x + Random.Range(0f, pickUpBezierOffset), p3.y + Random.Range(0f, pickUpBezierOffset));*/

            Vector2 directionToPlayer = (collector.transform.position - transform.position).normalized;
            while ((collector.transform.position - transform.position).sqrMagnitude >= 0.01f)
            {
                directionToPlayer = (collector.transform.position - transform.position).normalized;
                //p3 = collector.transform.position;
                //transform.position = (Vector3) MathUtilities.CubicBezierCurve(p0, p1, p2, p3, MathUtilities.EaseInQuad(t));
                transform.Translate(directionToPlayer * Time.deltaTime * 6);
                t += Time.deltaTime;
                yield return null;
            }

            OnCollect(collector);
        }


        /**********************************************
        *             PointHandler Interface
        ***********************************************/

        public void OnPointerExit(PointerEventData eventData)
        {
            nameText.fontStyle = FontStyles.Normal;
            nameText.color = RarityDrawer.GetColorForRarity(itemData.rarity);

            HideUI();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            nameText.fontStyle = FontStyles.Underline;
            nameText.color = Color.white;

            // Player와 물체까지 거리계산, 일정 범위 이하일 때만 ShowUI() 할 것
            ShowUI();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            photonView.RPC("Destroy", photonView.Owner);
        }

        [PunRPC]
        void Destroy()
        {
            PhotonNetwork.Destroy(gameObject);
        }

        /**********************************************
        *             User Interface
        ***********************************************/

        public void OnCollect(Collector collector)
        {
            if (!InventoryManager.Instance.CanPickUp(itemData))
                return;

            //뭐 데이터는 로컬에서 주워먹음 되지 않을까, 인벤토리나 스탯관리에서 동기화 하고있겠지
            for (int i = 0; i < dropCount; i++)
            {
                collector.CollectItem(itemData);
            }

            // RoomObject라서 아이템 삭제 요청은 마스터에게 보내야함
            if (PhotonNetwork.IsConnected)
                //PhotonNetwork.Destroy(gameObject);
                photonView.RPC("Destroy", photonView.Owner);
            else
                Destroy(gameObject);
        }

        public void OnInteract(Interactor interactor)
        {
            if (!canPickUp || itemData is not EquipItemData)
                return;
            OnCollect(interactor.collector);
        }

        public void ShowUI()
        {
            if (!canPickUp || itemData is not EquipItemData)
                return;
            interactionUI.SetActive(true);
        }

        public void HideUI()
        {
            if (!canPickUp || itemData is not EquipItemData)
                return;
            interactionUI.SetActive(false);
        }


       /**********************************************
       *             Photon Interface
       ***********************************************/

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            object[] instantiationData = info.photonView.InstantiationData;
            ItemData = ItemDataManager.Instance.GetItemData((byte[]) instantiationData[0]);
            DropCount = (int) instantiationData[1];
        }


        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(canPickUp);
            }
            else
            {
                canPickUp = (bool) stream.ReceiveNext();
            }
        }
    }
}
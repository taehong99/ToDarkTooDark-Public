using System.Collections.Generic;
using Tae;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;
using System;
using Tae.Inventory;

namespace ItemLootSystem
{

    public class BaseBox : MonoBehaviourPun, IInteractable
    {
        //[SerializeField] string boxName;
        //[SerializeField] int id;

        [SerializeField] bool requireKey;
        //[SerializeField] int roomsAvailableList;

        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Sprite openSprite;
        [SerializeField] Sprite closeSprite;

        //for test
        [SerializeField] DropedItem itemPrefab;
        [SerializeField] ItemDropTable table;
        public ItemDropTable Table { set { table = value; } }
        [SerializeField] float dropRange;
        [Range(0,5)]
        [SerializeField] float dropSpeed;

        [SerializeField] AudioClip openSound;

        public event Action OnBoxOpen;

        bool isOpened = false; //상자의 열리고 닫힌 상태
        bool openFlag = false; // 상자에서 아이템이 이미 드랍되었는지 체크

        [PunRPC]
        public void PickItemFromTable(WeaponType type = WeaponType.NULL)
        {
            Debug.Log("PickItemFromTable called");
            List<BaseItemData> dropedItems = table.DropedItems(type);

            foreach (BaseItemData item in dropedItems)
            {
                if(!PhotonNetwork.IsConnected)
                    LocalItemDrop(item);
                else
                    NetworkItemDrop(item);
            }
        }

        /// <summary>
        /// 로컬에서 아이템 드랍
        /// </summary>
        /// <param name="item"></param>
        public void LocalItemDrop(BaseItemData item)
        {
            DropedItem dropedItem = Instantiate(itemPrefab, transform.position, Quaternion.identity, gameObject.transform);
            dropedItem.ItemData = item;
            dropedItem.Drop(dropRange, dropSpeed);
        }

        /// <summary>
        /// 네트워크에 아이템 드랍
        /// </summary>
        /// <param name="itemData"></param>
        public void NetworkItemDrop(BaseItemData itemData)
        {
            object[] data = ItemDataManager.Instance.ConvertToObjectArray(itemData);

            DropedItem dropedItem = PhotonNetwork.InstantiateRoomObject
                ("DroptedItem", transform.position, Quaternion.identity,0,data).GetComponent<DropedItem>();
            dropedItem.ItemData = itemData;
            dropedItem.Drop(dropRange, dropSpeed);           
        }

        /* Legacy
        [PunRPC]
        public void RPC_ItemDrop(byte[] serializedData, int photonViewID)
        {
            EquipItemData itemData = ItemDataManager.Instance.GetEquipData(serializedData);

            // 포톤 view의 ID 를 기반으로 객체를 찾아서 data 주입.
            PhotonView targetPhotonView = PhotonView.Find(photonViewID);
            if (targetPhotonView != null)
            {
                Debug.Log("Photon View Find");
                DropedItem dropedItem = targetPhotonView.GetComponent<DropedItem>();
                dropedItem.ItemData = itemData;
            }
        }*/


        [PunRPC]
        public void OpenBox()
        {
            if (openSprite != null)
            {
                spriteRenderer.sprite = openSprite;
            }

            if (openSound != null)
            {
                Manager.Sound.PlaySFX(openSound);
            }

            isOpened = true;

            openFlag = true;
        }

        [PunRPC]
        public void CloseBox()
        {
            Debug.Log("CloseBox called");
            if (closeSprite != null)
                spriteRenderer.sprite = closeSprite;
            isOpened = false;
        }

        void ClearItems()
        {
            foreach (Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        }

        public virtual void OnInteract(Interactor interactor)
        {
            if (PhotonNetwork.IsConnected)
            {
                if (!isOpened)
                {
                    if (!PhotonNetwork.IsMasterClient && openFlag ==false)
                    {
                        photonView.RPC("PickItemFromTable", RpcTarget.MasterClient, Manager.Game.MyWeaponType);
                        openFlag = true;
                        return;
                    }
                    else if(openFlag == false)
                    {
                        PickItemFromTable(Manager.Game.MyWeaponType);
                        openFlag = true;
                    }
                    InventoryManager.Instance.ObtainGold(table.DropGold());
                    photonView.RPC("OpenBox", RpcTarget.All); // interactor의 무기정보 받아오는거 추가할것.
                }
                else
                {
                    photonView.RPC("CloseBox", RpcTarget.All);
                }
            }
            else
            {
                if (!isOpened)
                {
                    OpenBox();
                    PickItemFromTable();
                    OnBoxOpen?.Invoke();
                }
                else
                    CloseBox();
            }
        }



        public virtual void ShowUI()
        {
            Debug.Log($"{gameObject.name} Show UI");
        }

        public virtual void HideUI()
        {
            Debug.Log($"{gameObject.name} Hide UI");
        }
    }
}

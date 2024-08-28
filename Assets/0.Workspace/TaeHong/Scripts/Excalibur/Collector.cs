using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ItemLootSystem;

namespace Tae.Inventory
{
    public class Collector : MonoBehaviour
    {
        [SerializeField] ExcaliburOwner playerExcaliburComponent;

        // ICollectable 엑스칼리버가 호출하는 엑칼 줍기 함수
        public void CollectExcalibur()
        {
            Debug.Log("Collected : Excalibur");
            Manager.Event.ExcaliburPickedUp(GetComponentInParent<PhotonView>().ViewID);
            playerExcaliburComponent.enabled = true;
        }

        // ICollectable 아이템이 호출하는 아이템 줍기 함수
        public void CollectItem(BaseItemData item)
        {
            Debug.Log($"Collected : {item.Name}");
            InventoryManager.Instance.ObtainItem(item);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemLootSystem;
using Photon.Pun;
using Tae.Inventory;

public class DebugHacks : MonoBehaviourPun
{
    [SerializeField] BaseItemData gemItem;
    PlayerStatsManager stats;
    ItemFactory itemFactory;

    Vector3 excaliburPoint = new Vector3(-1, -1, 0);


    private void Start()
    {
        itemFactory = FindObjectOfType<ItemFactory>();
        stats = GetComponent<PlayerStatsManager>();
    }

    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            TeleportToExcalibur();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            LevelUp();
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            InventoryManager.Instance.ObtainGold(3000);
        }
    }

    private void TeleportToExcalibur()
    {
        transform.position = excaliburPoint;
        Debug.Log(gemItem.GetInstanceID());
        itemFactory.DropItem(gemItem, excaliburPoint, 3);
    }

    private void LevelUp()
    {
        if(stats.CurLevel < 10)
        {
            stats.CurEXP = stats.MaxEXP;
        }
    }
}

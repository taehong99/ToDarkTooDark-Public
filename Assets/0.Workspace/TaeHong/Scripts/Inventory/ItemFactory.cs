using ExitGames.Client.Photon;
using ItemLootSystem;
using Photon.Pun;
using UnityEngine;

public class ItemFactory : MonoBehaviourPun
{
    public static ItemFactory Instance { get; private set; }

    [SerializeField] DropedItem itemPrefab;
    [SerializeField] float dropRange;
    [Range(0, 5)]
    [SerializeField] float dropSpeed;

    [SerializeField] BaseItemData[] testItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PhotonPeer.RegisterType(typeof(BaseItemData), (byte) 'B', BaseItemData.Serialize, BaseItemData.Deserialize);
        PhotonPeer.RegisterType(typeof(EquipItemData), (byte) 'E', EquipItemData.Serialize, EquipItemData.Deserialize);
    }

    public void DebugSpawnItems()
    {
        foreach (var item in testItem)
        {
            SpawnItem(item, new Vector2(-17, 0));
        }
    }

    public void SpawnEquipment(EquipItemData equipItemData, Vector2 spawnPos)
    {
        if (PhotonNetwork.IsConnected)
        {
            object[] data = ItemDataManager.Instance.ConvertToObjectArray(equipItemData);
            DropedItem dropedItem = PhotonNetwork.InstantiateRoomObject
                ("TaeDroppedItem", spawnPos, Quaternion.identity, 0, data).GetComponent<DropedItem>();
            //dropedItem.ItemData = equipItemData;
            dropedItem.Drop(dropRange, dropSpeed);
        }
        else
        {
            DropedItem dropedItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
            dropedItem.ItemData = equipItemData;
            dropedItem.Drop(dropRange, dropSpeed);
        }
    }

    public void SpawnItem(BaseItemData itemData, Vector2 spawnPos, int count = 1)
    {
        if (PhotonNetwork.IsConnected)
        {
            object[] data = ItemDataManager.Instance.ConvertToObjectArray(itemData, count);
            DropedItem dropedItem = PhotonNetwork.InstantiateRoomObject
                ("TaeDroppedItem", spawnPos, Quaternion.identity, 0, data).GetComponent<DropedItem>();
            //dropedItem.ItemData = itemData;
            //dropedItem.DropCount = count;
            dropedItem.Drop(dropRange, dropSpeed);
        }
        else
        {
            DropedItem dropedItem = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
            dropedItem.ItemData = itemData;
            dropedItem.DropCount = count;
            dropedItem.Drop(dropRange, dropSpeed);
        }
    }

    // Used for Player drop item
    public void DropItem(BaseItemData itemData, Vector2 dropPos, int count = 1)
    {
        if (PhotonNetwork.IsConnected)
        {
            byte[] serializedData;
            if (itemData is EquipItemData)
                serializedData = EquipItemData.Serialize(itemData);
            else
                serializedData = BaseItemData.Serialize(itemData);

            photonView.RPC("RequestSpawnItem", RpcTarget.MasterClient, serializedData, dropPos, System.Convert.ToByte(count));
        }
        else
        {
            DropedItem dropedItem = Instantiate(itemPrefab, dropPos, Quaternion.identity);
            dropedItem.ItemData = itemData;
            dropedItem.DropCount = count;
            dropedItem.Drop(dropRange, dropSpeed);
        }
    }

    [PunRPC]
    public void RequestSpawnItem(byte[] serializedData, Vector2 spawnPos, byte count)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            BaseItemData itemData = ItemDataManager.Instance.GetItemData(serializedData);
            object[] data = ItemDataManager.Instance.ConvertToObjectArray(itemData, count);
            DropedItem dropedItem = PhotonNetwork.InstantiateRoomObject
                ("TaeDroppedItem", spawnPos, Quaternion.identity, 0, data).GetComponent<DropedItem>();
            //dropedItem.ItemData = itemData;
            //dropedItem.DropCount = count;
            dropedItem.Drop(dropRange, dropSpeed);
        }
    }

    public void SpawnNetworkEquipment(EquipItemData equipItem, Vector2 spawnPos)
    {

    }

    public void SpawnNetworkItem(BaseItemData itemData, Vector2 spawnPos)
    {
        object[] data = ItemDataManager.Instance.ConvertToObjectArray(itemData);

        DropedItem dropedItem = PhotonNetwork.InstantiateRoomObject
            ("DroptedItem", transform.position, Quaternion.identity, 0, data).GetComponent<DropedItem>();
        dropedItem.ItemData = itemData;
        dropedItem.Drop(dropRange, dropSpeed);
    }
}

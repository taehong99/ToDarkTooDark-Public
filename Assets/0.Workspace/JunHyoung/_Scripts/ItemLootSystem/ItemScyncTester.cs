using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ItemLootSystem
{
    /// <summary>
    /// 아이템 드랍(생성) 데이터 동기화 테스트용 스크립트
    /// </summary>
    public class ItemScyncTester : MonoBehaviourPunCallbacks
    {
        // BaseItemData, EquipItemData 의 Serialize / Deserialize 메서드를 작성하고 송수신 하는 방식은 데이터양이 불필요하게 많음 
        // RaiseEvent로 Item ID + (랜덤하게 붙는 값인)강화 레벨, 수식어만 전달?  <- 나머지 데이터는 각각 로컬에서 모두 동일하니깐.

        // 이후 모-든 아이템을 가지고 있는 Manager 에서 ID를 가지고 검색해서 Instantiate, 강화 레벨, 수식어 적용해서 생성
        // 어짜피 매니저 만들꺼니깐, 매니저에 아이템 생성 이벤트 요청, 처리도 하자

        // 수식어도 데이터양 불필요하게 많은데 ... 이름 보내고 이름 기준으로 검색?  X  수식어에도 ID 붙여놓자

        [SerializeField] Image connetionImage;

        [SerializeField] Sprite disconnectImage;
        [SerializeField] Sprite connectImage;

        [SerializeField] TextMeshProUGUI isMasterText;
        void Start()
        {
            PhotonNetwork.LocalPlayer.NickName = $"DebugPlayer {Random.Range(1000, 10000)}";
            PhotonNetwork.ConnectUsingSettings();
            //PhotonNetwork.JoinRandomOrCreateRoom();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master server. Attempting to join a random room...");
            RoomOptions roomOptions = new RoomOptions();
            PhotonNetwork.JoinOrCreateRoom("ItemScyncTestRoom", roomOptions, TypedLobby.Default);
            
        }

        public override void OnJoinedRoom()
        {
            if(connetionImage != null)
                 connetionImage.sprite = connectImage;
            // Called when successfully joined a room
            PhotonNetwork.AddCallbackTarget(this);
            Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
            int seed = Random.Range(0, 1000);
            PhotonNetwork.CurrentRoom.SetSeed(seed);

            if (PhotonNetwork.IsMasterClient)
            {
                //isMasterText.text = "Master";
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);

            if (PhotonNetwork.IsMasterClient)
            {
                isMasterText.text = "Master";
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            //connetionImage.sprite = disconnectImage;
        }

        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

    }
}
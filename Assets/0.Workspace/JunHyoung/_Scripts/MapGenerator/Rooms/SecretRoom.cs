using ItemLootSystem;
using Photon.Pun;
using UnityEngine;
namespace MapGenerator
{
    public class SecretRoom : Room
    {
        [Space(10)]
        // 입구 위치,
        [SerializeField] public Transform entrancePoint;
        // 출구, 입장했던 특수방 입구와 연결됨
        [SerializeField] public SecretRoomEntrance exit;
        

        // 입장시 몬스터 스폰
        //[SerializeField] MonsterSpawner spawner;

        [SerializeField] Transform rewardPoint;

        TreasureBox box;

        private void Start()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            spawner.SpawnMonsters(); //스폰도 마스터에서만하고, 감지도 마스터에서만 하니깐 문제 없을..듯?
            box = Manager.DropTable.GetTresureBoxSecretRoom();
            spawner.allMonsterDied += DropRewards; // 근데 이러면 중간에 마스터 나갔을때 이벤트 발생 안할것...같은데
        }

        void DropRewards()
        {
            DropTreasureBox();
            DropVIPTicket();
        }

        void DropTreasureBox()
        {
            if (!PhotonNetwork.IsConnected)
            {
                TreasureBox treasureBox = Instantiate(box, rewardPoint == null ? rewardPoint.position : transform.position, Quaternion.identity);
                treasureBox.gameObject.transform.JellyPingPongTransform(treasureBox, 0.5f, 3, 3f, 0.8f);
                // box.gameObject.transform.JellyPingPongTransform(box, 0.5f, 3, 3f,0.8f); <- 하면 box가 씬에 생성된 객체가 아니라 코루틴 실행 X
            }
            else
            {
                string prefabName = MapManager.Instance.GetTreasureBoxPrefabName(box.rarity);
                TreasureBox treasureBox = PhotonNetwork.InstantiateRoomObject(prefabName, rewardPoint == null ? rewardPoint.position : transform.position, Quaternion.identity).GetComponent<TreasureBox>();
                treasureBox.gameObject.transform.JellyPingPongTransform(treasureBox, 0.5f, 3, 3f, 0.8f);
            }
        }


        void DropVIPTicket()
        {
            // 이건 엘리트 몬스터 드랍테이블에 들어가 있어야할듯?
        }

        void OnDisable()
        {
            spawner.allMonsterDied -= DropRewards;
        }

        public void InstantiateExit(Vector3 exitPoint)
        {
            exit.enabled = false;
            object[] data = new object[] { exitPoint.x, exitPoint.y, exitPoint.z };
            PhotonNetwork.InstantiateRoomObject(exit.name, exit.transform.position, Quaternion.identity, 0, data);
        }

        public override void DrawRoom()
        {
            base.DrawRoom();
            //TileMapManager.Instance.GenerateShadowCaster();
        }
    }
}

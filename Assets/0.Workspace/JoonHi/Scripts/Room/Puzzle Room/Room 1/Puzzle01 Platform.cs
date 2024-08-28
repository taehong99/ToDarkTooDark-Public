using System.Collections;
using UnityEngine;

namespace JH
{
    public class Puzzle01Platform : MonoBehaviour
    {
        [Header("GameObjects")]
        [SerializeField] PuzzleRoom01Buttons pressurePlate1;
        [SerializeField] PuzzleRoom01Buttons pressurePlate2;
        [SerializeField] PuzzleRoom01 PuzzleRoom;


        [Header("Prefabs")]
        [SerializeField] GameObject treasurePrefab;
        [SerializeField] Transform treasureSpawnPoint;

        [Header("LayerMask")]
        [SerializeField] LayerMask layermask;

        [Header("Stat")]
        [SerializeField] int bridgeTime;

        private bool isTreasureGen;
        public bool platLeft { get; set; }
        public bool isDeactivate { get; private set; }
        private IEnumerator bridgetimer;

        private void Awake()
        {
            platLeft = true;
            isDeactivate = false;
            isTreasureGen = false;
        }

        private void Start()
        {
            if (gameObject.transform.childCount < 3)
            {
                Debug.LogWarning("Not enough Pressure Plate for Puzzle. Check the GameObject");
                gameObject.SetActive(false);
            }
            pressurePlate1 = gameObject.transform.GetChild(0).GetComponent<PuzzleRoom01Buttons>();
            pressurePlate2 = gameObject.transform.GetChild(1).GetComponent<PuzzleRoom01Buttons>();
            treasureSpawnPoint = gameObject.transform.GetChild(2).transform;
            PuzzleRoom = gameObject.transform.parent.GetComponent<PuzzleRoom01>();
            PuzzleRoom.BridgeUndeploy();
        }

        public void PlateActivate()
        {
            if (pressurePlate1.state != PuzzleRoom01Buttons.ButtonState.NotPressed && pressurePlate2.state != PuzzleRoom01Buttons.ButtonState.NotPressed)
            {
                pressurePlate1.Deactivate();
                pressurePlate2.Deactivate();
                if (isTreasureGen == false)
                {
                    isTreasureGen = true;
                    TreasureTemp treasureInst = Instantiate(treasurePrefab, treasureSpawnPoint).GetComponent<TreasureTemp>();
                    treasureInst.Opened.AddListener(Deactivate);
                    treasureInst.Opened.AddListener(PuzzleRoom.Deactivate);
                }
                PuzzleRoom.BridgeDeploy();
                if (PuzzleRoom.isDeactivate != true)
                {
                    bridgetimer = BridgeTimer(bridgeTime);
                    StartCoroutine(bridgetimer);
                }
            }
        }

        public void Deactivate()
        {
            if (PuzzleRoom.isDeactivate != true)
            {
                if (bridgetimer != null)
                {
                    StopCoroutine(bridgetimer);
                    PuzzleRoom.activeCoroutineNum -= 1;
                }
                PuzzleRoom.BridgeDeploy();

                Debug.Log($"{gameObject.name} Diactivate");
            }
            pressurePlate1.Deactivate();
            pressurePlate2.Deactivate();
            isDeactivate = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (layermask.Contain(collision.gameObject.layer))
            {
                GameObject buf = collision.gameObject;
                switch (platLeft)
                {
                    case true:
                        if (PuzzleRoom.plate1Spawn.Contains(buf))
                            break;
                        if (PuzzleRoom.plate2Spawn.Contains(buf))
                            PuzzleRoom.plate2Spawn.Remove(buf);
                        PuzzleRoom.plate1Spawn.Add(collision.gameObject);
                        break;
                    case false:
                        if (PuzzleRoom.plate2Spawn.Contains(buf))
                            break;
                        if (PuzzleRoom.plate1Spawn.Contains(buf))
                            PuzzleRoom.plate1Spawn.Remove(buf);
                        PuzzleRoom.plate2Spawn.Add(collision.gameObject);
                        break;
                }
            }
        }

        private IEnumerator BridgeTimer(int time)
        {
            PuzzleRoom.activeCoroutineNum += 1;
            int timeleft = 0;
            while (timeleft < bridgeTime)
            {
                timeleft += 1;
                yield return new WaitForSeconds(1);
            }
            pressurePlate1.Activate();
            pressurePlate2.Activate();
            PuzzleRoom.activeCoroutineNum -= 1;
            if (PuzzleRoom.isDeactivate != true && PuzzleRoom.activeCoroutineNum == 0)
                PuzzleRoom.BridgeUndeploy();
        }
    }
}
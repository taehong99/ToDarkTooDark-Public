using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace JH
{
    public class PuzzleRoom03 : MonoBehaviourPunCallbacks
    {
        [Header("Pillars")]
        [SerializeField] PuzzleRoom03Pillar upperLeftPillar;
        [SerializeField] PuzzleRoom03Pillar upperRightPillar;
        [SerializeField] PuzzleRoom03Pillar bottomLeftPillar;
        [SerializeField] PuzzleRoom03Pillar bottomRightPillar;

        [Header("Hint Point")]
        [SerializeField] SpriteRenderer HintPoint1;
        [SerializeField] SpriteRenderer HintPoint2;
        [SerializeField] List<Sprite> HintSprite = new List<Sprite>();

        [Header("Prefabs")]
        [SerializeField] GameObject treasurePrefab;

        public bool Setting { get; private set; }

        private List<PuzzleRoom03Pillar.Element> answer = new List<PuzzleRoom03Pillar.Element>();
        private List<PuzzleRoom03Pillar.Element> submit = new List<PuzzleRoom03Pillar.Element>();

        private IEnumerator Restart;

        private void Start()
        {
            upperLeftPillar = gameObject.transform.GetChild(0).GetComponent<PuzzleRoom03Pillar>();
            upperRightPillar = gameObject.transform.GetChild(1).GetComponent<PuzzleRoom03Pillar>();
            bottomLeftPillar = gameObject.transform.GetChild(2).GetComponent<PuzzleRoom03Pillar>();
            bottomRightPillar = gameObject.transform.GetChild(3).GetComponent<PuzzleRoom03Pillar>();

            upperLeftPillar.element = PuzzleRoom03Pillar.Element.Water;
            upperRightPillar.element = PuzzleRoom03Pillar.Element.Wind;
            bottomLeftPillar.element = PuzzleRoom03Pillar.Element.Fire;
            bottomRightPillar.element = PuzzleRoom03Pillar.Element.Grass;

            HintPoint1 = gameObject.transform.GetChild(4).GetChild(0).GetComponent<SpriteRenderer>();
            HintPoint2 = gameObject.transform.GetChild(4).GetChild(1).GetComponent<SpriteRenderer>();

            GenerateAnswer();
            //GenerateHint();
        }

        public override void OnJoinedRoom()
        {
            GenerateAnswer();
        }

        [PunRPC]
        public void InitPuzzle(byte element1, byte element2)
        {
            answer.Add((PuzzleRoom03Pillar.Element) (int) element1);
            answer.Add((PuzzleRoom03Pillar.Element) (int) element2);
            GenerateHint();
        }

        private void GenerateAnswer()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            answer.Add((PuzzleRoom03Pillar.Element) (UnityEngine.Random.Range(1, 120) % 4));
            while (answer.Count < 2)
            {
                int randnum = UnityEngine.Random.Range(1, 120) % 4;
                if (!answer.Contains((PuzzleRoom03Pillar.Element) randnum))
                {
                    answer.Add(((PuzzleRoom03Pillar.Element) randnum));
                }
            }
            photonView.RPC("InitPuzzle", RpcTarget.OthersBuffered, System.Convert.ToByte(answer[0]), System.Convert.ToByte(answer[1]));
            GenerateHint();
        }

        private void GenerateHint()
        {
            HintPoint1.sprite = HintSprite[(int) answer[0]];
            HintPoint2.sprite = HintSprite[(int) answer[1]];
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Setting == false)
            {
                upperLeftPillar.Activate();
                upperRightPillar.Activate();
                bottomLeftPillar.Activate();
                bottomRightPillar.Activate();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            upperLeftPillar.Deactivate();
            upperRightPillar.Deactivate();
            bottomLeftPillar.Deactivate();
            bottomRightPillar.Deactivate();
        }

        public void inputIn(PuzzleRoom03Pillar.Element element, bool ONOFF)
        {
            if (ONOFF == true)
                submit.Add(element);
            else
                submit.Remove(element);
            if (submit.Count == 2)
            {
                Setting = true;
                if (answer.Contains(submit[0]) && answer.Contains(submit[1]))
                {
                    Deactivate();
                    return;
                }
                submit.Clear();
                Restart = restart();
                StartCoroutine(Restart);
            }
        }

        private void Deactivate()
        {
            upperLeftPillar.StopPillar();
            upperRightPillar.StopPillar();
            bottomLeftPillar.StopPillar();
            bottomRightPillar.StopPillar();

            Instantiate(treasurePrefab, gameObject.transform);
        }

        private IEnumerator restart()
        {
            yield return new WaitForSeconds(1);
            upperLeftPillar.OFF();
            upperRightPillar.OFF();
            bottomLeftPillar.OFF();
            bottomRightPillar.OFF();
            yield return new WaitForSeconds(1);
            upperLeftPillar.Deactivate();
            upperRightPillar.Deactivate();
            bottomLeftPillar.Deactivate();
            bottomRightPillar.Deactivate();
            yield return new WaitForSeconds(2);
            upperLeftPillar.Activate();
            upperRightPillar.Activate();
            bottomLeftPillar.Activate();
            bottomRightPillar.Activate();
            Setting = false;
        }
    }
}
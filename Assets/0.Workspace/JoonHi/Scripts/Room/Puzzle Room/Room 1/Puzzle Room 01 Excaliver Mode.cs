using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRoom01ExcaliverMode : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] PuzzleRoom01 PuzzleRoom;

    [Header("LayerMask")]
    [SerializeField] LayerMask layermask;

    private List<GameObject> gameObjects;

    private int Timer;
    private IEnumerator bridgetimer;
    private IEnumerator bridgeRefillTimer;
    private bool isRefilling;

    private void Awake()
    {
        gameObjects = new List<GameObject>();
        isRefilling = false;
    }

    private void Start()
    {
        PuzzleRoom = gameObject.transform.parent.GetComponent<PuzzleRoom01>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRefilling == false && layermask.Contain(collision.gameObject.layer))
        {
            gameObjects.Add(collision.gameObject);
            if (bridgetimer == null)
            {
                bridgetimer = BridgeTimer();
                StartCoroutine(bridgetimer);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isRefilling == false && layermask.Contain(collision.gameObject.layer))
        {
            gameObjects.Remove(collision.gameObject);
            if (gameObjects.Count == 0 && Timer > 1)
            {
                StopCoroutine(bridgetimer);
                bridgetimer = null;
                bridgeRefillTimer = BridgeVanishTimer();
                StartCoroutine(bridgeRefillTimer);
            }
        }
    }

    private IEnumerator BridgeTimer()
    {
        while (true)
        {
            Timer += 1;
            Debug.Log(Timer);
            yield return new WaitForSeconds(1);
        }
    }
    private IEnumerator BridgeVanishTimer()
    {
        isRefilling = true;
        PuzzleRoom.BridgeUndeploy();
        while (Timer > 0)
        {
            yield return new WaitForSeconds(1);
            Timer -= 1;
        }
        PuzzleRoom.BridgeDeploy();
        isRefilling = false;
    }
}

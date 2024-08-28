using JH;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleRoom01 : MonoBehaviour
{
    [Header("Tiles")]
    [SerializeField] GameObject bridgeDeploy;
    [SerializeField] GameObject bridgeUndeploy;

    [Header("Platforms")]
    [SerializeField] Puzzle01Platform plat1;
    [SerializeField] Puzzle01Platform plat2;

    [Header("GameObject")]
    [SerializeField] GameObject excaliverMode;

    [Header("LayerMask")]
    [SerializeField] LayerMask layermask;

    public List<GameObject> plate1Spawn { get; set; }
    public List<GameObject> plate2Spawn { get; set; }

    public bool isDeactivate { get; private set; }
    public int activeCoroutineNum { get; set; }

    private void Awake()
    {
        plate1Spawn = new List<GameObject>();
        plate2Spawn = new List<GameObject>();
    }

    private void Start()
    {
        isDeactivate = false;
        activeCoroutineNum = 0;

        plat1 = gameObject.transform.GetChild(0).GetComponent<Puzzle01Platform>();
        plat2 = gameObject.transform.GetChild(1).GetComponent<Puzzle01Platform>();
        bridgeDeploy = gameObject.transform.GetChild(2).GetChild(0).gameObject;
        bridgeUndeploy = gameObject.transform.GetChild(2).GetChild(1).gameObject;
        excaliverMode = gameObject.transform.GetChild(3).gameObject;

        plat1.platLeft = true;
        plat2.platLeft = false;

        bridgeDeploy.SetActive(false);
        bridgeUndeploy.SetActive(true);

        excaliverMode.SetActive(false);
    }

    public void BridgeDeploy()
    {
        bridgeDeploy.SetActive(true);
        bridgeUndeploy.SetActive(false);
    }

    public void BridgeUndeploy()
    {
        bridgeDeploy.SetActive(false);
        Collider2D hit = Physics2D.OverlapBox(transform.position, new Vector2(3, 4), 0, layermask);
        if (hit != null)
        {
            if (plate1Spawn.Contains(hit.gameObject))
                hit.gameObject.transform.position = new Vector3(plat1.transform.position.x, plat1.transform.position.y, hit.gameObject.transform.position.z);
            else
                hit.gameObject.transform.position = new Vector3(plat2.transform.position.x, plat2.transform.position.y, hit.gameObject.transform.position.z);
        }
        bridgeUndeploy.SetActive(true);
    }

    public void Deactivate()
    {
        BridgeDeploy();
        isDeactivate = true;
    }

    public void ExcaliverMode()
    {
        Deactivate();
        plat1.Deactivate();
        plat2.Deactivate();
        BridgeDeploy();
        excaliverMode.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (layermask.Contain(collision.gameObject.layer))
        {
            plate1Spawn.Remove(collision.gameObject);
            plate2Spawn.Remove(collision.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector2(3,4));
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Excaliver Mode")]
    private void DebugExcaliverPulled()
    {
        ExcaliverMode();
    }
#endif
}

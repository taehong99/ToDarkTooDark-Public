using UnityEngine;

public class FieldOFView : MonoBehaviour
{
    [SerializeField] float viewRadius = 10;
    [SerializeField] float viewLength = 10;
    [SerializeField] LayerMask layerMask;
    [SerializeField] LayerMask layerMask2;
    Collider2D [] playerInRadius;
    Collider [] colliders = new Collider [20];
    [SerializeField] bool debug;
    private GameObject DebugRayingObject;
    private GameObject Debugray;

    private void Update()
    {
        FindVidiblePlayer();
    }

    void FindVidiblePlayer()
    {
        int size = Physics.OverlapSphereNonAlloc(transform.position, viewRadius, colliders, layerMask);

        if (size == 0)
        {
            DebugRayingObject = null;
            Debugray = null;
        }

        for ( int i = 0; i < size; i++ )
        {
            DebugRayingObject = colliders[i].gameObject;
            Vector3 dirToTarget = ( colliders [i].transform.position - transform.position ).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, viewLength, layerMask2);
            if (hit != null)
            {
                Debugray = hit.collider.gameObject;
                Debug.DrawRay(transform.position, dirToTarget * Vector3.Distance(transform.position, hit.collider.gameObject.transform.position), Color.red);
            }
            else
                Debugray = null;
            if ( hit.collider == colliders [i] )
            {
                if(colliders [i].gameObject.GetComponent<LightSource>().lightGameObject.activeSelf == false)
                    colliders [i].gameObject.GetComponent<LightSource>().lightGameObject.SetActive(true);
            }
            else
            {
                colliders [i].gameObject.GetComponent<LightSource>().lightGameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if ( debug != true )
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }

}

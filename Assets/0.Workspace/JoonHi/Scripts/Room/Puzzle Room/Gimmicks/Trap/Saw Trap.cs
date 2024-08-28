using Photon.Pun;
using System.Collections;
using UnityEngine;

public class SawTrap : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator animator;

    [Header("Saw Point")]
    [SerializeField] Transform sawPointLeft;
    [SerializeField] Transform sawPointRight;

    [Header("Spec")]
    [SerializeField] float sawSpeed;
    [SerializeField] bool sawToRight;
    [SerializeField]
    [Range(0, 1)] float DamageRate;


    public Transform syncPosition;
    private IEnumerator sawRouter;
    private bool turning;

    private void Start()
    {
        transform.position = sawToRight ? sawPointLeft.position : sawPointRight.position;
        // syncPosition = sawToRight ? sawPointLeft : sawPointRight;
        sawRouter = SawRouter();
    }

    public void SawStart()
    {
        if (turning == false)
        {
            animator.SetTrigger("StartSaw");
            turning = true;
            // transform.position = syncPosition.position;
            StartCoroutine(sawRouter);
        }
    }

    public void SawStop()
    {
        if (turning == true)
        {
            StopCoroutine(sawRouter);
            animator.SetTrigger("EndSaw");
            turning = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out PhotonView photonView))
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            damageable.TakeDamage((int) Mathf.Floor(damageable.MaxHealth * DamageRate));
        }
    }

    private IEnumerator SawRouter()
    {
        while (true)
        {
            if (sawToRight)
                transform.position = new Vector2(transform.position.x + (sawSpeed * Time.deltaTime), transform.position.y);
            else
                transform.position = new Vector2(transform.position.x - (sawSpeed * Time.deltaTime), transform.position.y);
            if (transform.position.x >= sawPointRight.position.x)
            {
                transform.position = sawPointRight.position;
                sawToRight = false;
            }
            else if (transform.position.x <= sawPointLeft.position.x)
            {
                transform.position = sawPointLeft.position;
                sawToRight = true;
            }
            yield return new WaitForEndOfFrame();
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Start")]
    private void DebugStart()
    {
        SawStart();
    }

    [ContextMenu("Debug Stop")]
    private void DebugStop()
    {
        SawStop();
    }
#endif
}

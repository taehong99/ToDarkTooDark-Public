using System.Collections;
using System.Collections.Generic;
using Tae;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public enum SpikeType { Immediate, Deactivate, Timer };

    [Header("Components")]
    [SerializeField] SpriteRenderer spriterenderer;
    [SerializeField] Animator animator;

    [Header("Spec")]
    [SerializeField] SpikeType type;
    [SerializeField] int time;
    [SerializeField] int damageAmount;

    private bool activate;
    private bool wating;
    private GameObject player;

    private IEnumerator timer;

    private void Start()
    {
        switch (type)
        {
            case SpikeType.Immediate:
                activate = false;
                break;
            case SpikeType.Deactivate:
                animator.SetTrigger("SpikeOut");
                activate = true;
                break;
            case SpikeType.Timer:
                activate = false;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PlayerHealth>().photonView.IsMine)
        {
            if (player == null)
            {
                player = collision.gameObject;
                switch (type)
                {
                    case SpikeType.Immediate:
                        animator.SetTrigger("SpikeOut");
                        collision.GetComponent<PlayerHealth>().TakeFixedDamage(damageAmount);
                        break;
                    case SpikeType.Deactivate:
                        if (activate)
                            collision.GetComponent<PlayerHealth>().TakeFixedDamage(damageAmount);
                        break;
                    case SpikeType.Timer:
                        if (wating != true)
                        {
                            wating = true;
                            timer = TimerforTimerSpike(time);
                            StartCoroutine(timer);
                        }
                        break;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (type)
        {
            case SpikeType.Immediate:
                /*
                if (activate)
                    collision.GetComponent<PlayerHealth>().TakeFixedDamage(damageAmount);
                */
                break;
            case SpikeType.Deactivate:
                /*
                if(activate)
                    collision.GetComponent<PlayerHealth>().TakeFixedDamage(damageAmount);
                */
                break;
            case SpikeType.Timer:
                if (wating != true)
                {
                    wating = true;
                    timer = TimerforTimerSpike(time);
                    StartCoroutine(timer);
                }
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<PlayerHealth>().photonView.IsMine)
        {
            if (player != null)
            {
                player = null;
                switch (type)
                {
                    case SpikeType.Immediate:
                        animator.SetTrigger("SpikeIn");
                        break;
                    case SpikeType.Deactivate:
                        break;
                    case SpikeType.Timer:
                        break;
                }
            }
        }
    }

    public void Deactivate()
    {
        if (type == SpikeType.Deactivate && activate == true)
        {
            activate = false;
            animator.SetTrigger("SpikeIn");
            timer = TimerforDeactivateSpike(time);
            StartCoroutine(timer);
        }
    }

    IEnumerator TimerforTimerSpike(int time)
    {
        animator.SetBool("Wait", true);
        animator.SetTrigger("SpikeWait");
        yield return new WaitForSeconds(time);
        animator.SetBool("Wait", false);
        if (player != null)
            player.GetComponent<PlayerHealth>().TakeFixedDamage(damageAmount);
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("SpikeIn");
        wating = false;
    }

    IEnumerator TimerforDeactivateSpike(int time)
    {
        yield return new WaitForSeconds(time);
        activate = true;
        animator.SetTrigger("SpikeOut");
    }

#if UNITY_EDITOR
    [ContextMenu("Debug Deactivate")]
    private void DebugDeactivate()
    {
        Deactivate();
    }
#endif
}
